# ==========================================
# Genshin Clone - Daily Dev Log AI Summary
# ==========================================

$ErrorActionPreference = "Continue"


# ==========================================
# UTF-8 Encoding
# ==========================================

$utf8 = [System.Text.UTF8Encoding]::new($false)

[Console]::InputEncoding = $utf8
[Console]::OutputEncoding = $utf8
$OutputEncoding = $utf8

$env:LANG = "ko_KR.UTF-8"
$env:LC_ALL = "ko_KR.UTF-8"


# ==========================================
# Settings
# ==========================================

$documents = [Environment]::GetFolderPath("MyDocuments")

$vault = Join-Path $documents "Obsidian Vault\DevLog"

$project = "Genshin-Clone-Unity"

$date = Get-Date -Format "yyyy-MM-dd"

$outputDir = Join-Path $vault "Projects"
$outputDir = Join-Path $outputDir $project

$outputFile = Join-Path $outputDir "$date.md"

$model = "gpt-5.6-luna"

# AI에 전달할 최대 Diff 글자 수
$maxDiffLength = 60000


# ==========================================
# API Key Check
# ==========================================

if ([string]::IsNullOrWhiteSpace($env:OPENAI_API_KEY)) {

    Write-Host ""
    Write-Host "OPENAI_API_KEY not found."
    Write-Host "PowerShell을 새로 실행했는지 확인하세요."

    exit
}


# ==========================================
# Git Repository Check
# ==========================================

$isGitRepo = git rev-parse --is-inside-work-tree 2>$null


if ($LASTEXITCODE -ne 0 -or $isGitRepo -ne "true") {

    Write-Host ""
    Write-Host "현재 위치가 Git Repository가 아닙니다."

    exit
}


# ==========================================
# Collect Today's Commits
# ==========================================

Write-Host ""
Write-Host "Collecting today's commits..."


$commitLines = git `
    -c i18n.logOutputEncoding=utf-8 `
    log `
    --since="$date 00:00:00" `
    --until="$date 23:59:59" `
    --pretty=format:"%h | %ad | %s" `
    --date=format:"%H:%M"


if ([string]::IsNullOrWhiteSpace(($commitLines -join ""))) {

    Write-Host ""
    Write-Host "오늘 생성된 Commit이 없습니다."

    exit
}


$commitList = $commitLines -join "`r`n"


# ==========================================
# Today's Commit Hashes
# ==========================================

$todayHashes = git log `
    --since="$date 00:00:00" `
    --until="$date 23:59:59" `
    --pretty=format:"%H"


$todayHashes = @($todayHashes)


if ($todayHashes.Count -eq 0) {

    Write-Host ""
    Write-Host "오늘 Commit Hash를 찾을 수 없습니다."

    exit
}


# git log = 최신 -> 과거
$firstCommit = $todayHashes[-1]
$lastCommit = $todayHashes[0]


# ==========================================
# Today's Changed Files
# ==========================================

Write-Host "Collecting changed files..."


$changedFiles = git `
    -c core.quotepath=false `
    log `
    --since="$date 00:00:00" `
    --until="$date 23:59:59" `
    --name-only `
    --pretty=format:""


# 필요 없는 Unity / Binary 파일 제외
$changedFiles = $changedFiles |
    Where-Object {

        $_ -and

        $_ -notmatch "\.meta$" -and

        $_ -notmatch "^Library/" -and
        $_ -notmatch "^Temp/" -and
        $_ -notmatch "^Logs/" -and
        $_ -notmatch "^UserSettings/" -and
        $_ -notmatch "^obj/" -and

        $_ -notmatch "\.(png|jpg|jpeg|gif|bmp|tga|psd)$" -and
        $_ -notmatch "\.(fbx|blend|obj)$" -and
        $_ -notmatch "\.(wav|mp3|ogg|mp4|mov)$" -and
        $_ -notmatch "\.(dll|exe|pdb|zip|7z)$"

    } |
    Sort-Object -Unique


if ($changedFiles.Count -gt 0) {

    $changedFileList = ($changedFiles | ForEach-Object {

        "- $_"

    }) -join "`r`n"

}
else {

    $changedFileList = "- 분석 가능한 변경 파일 없음"
}


# ==========================================
# Collect Today's Code Diff
# ==========================================

Write-Host "Collecting code changes..."


# Git Empty Tree Hash
$emptyTree = "4b825dc642cb6eb9a060e54bf8d69288fbee4904"


# 오늘 첫 Commit의 부모 존재 여부 확인
git cat-file -e "$firstCommit^" 2>$null

$hasParent = ($LASTEXITCODE -eq 0)


if ($hasParent) {

    # 오늘 첫 Commit 이전 상태
    # ↓
    # 오늘 마지막 Commit 상태

    $firstParent = git rev-parse "$firstCommit^" 2>$null


    $diff = git `
        -c core.quotepath=false `
        diff `
        $firstParent `
        $lastCommit `
        -- `
        "*.cs" `
        "*.json" `
        "*.asmdef" `
        "*.shader" `
        "*.hlsl" `
        "*.cginc" `
        "*.uxml" `
        "*.uss"

}
else {

    # Repository 최초 Commit인 경우
    # Empty Tree와 오늘 마지막 Commit 비교

    $diff = git `
        -c core.quotepath=false `
        diff `
        $emptyTree `
        $lastCommit `
        -- `
        "*.cs" `
        "*.json" `
        "*.asmdef" `
        "*.shader" `
        "*.hlsl" `
        "*.cginc" `
        "*.uxml" `
        "*.uss"
}


$diffText = $diff -join "`r`n"


# ==========================================
# Empty Diff
# ==========================================

if ([string]::IsNullOrWhiteSpace($diffText)) {

    $diffText = @"
오늘 Commit에서 분석 가능한 코드 Diff를 찾지 못했습니다.
Commit 메시지와 변경 파일 목록을 중심으로 분석하세요.
"@
}


# ==========================================
# Diff Size Limit
# ==========================================

if ($diffText.Length -gt $maxDiffLength) {

    $originalLength = $diffText.Length

    $diffText = $diffText.Substring(
        0,
        $maxDiffLength
    )


    $diffText += @"

==================================================
NOTICE
==================================================

전체 Diff 길이: $originalLength characters

비용 및 요청 크기 제한을 위해
앞부분 $maxDiffLength characters만 제공되었습니다.

제공되지 않은 코드의 구현 내용을 추측하지 마세요.

"@
}


# ==========================================
# AI Prompt
# ==========================================

$prompt = @"
당신은 Unity 게임 개발 프로젝트의 기술 개발일지를 작성하는 개발 문서 작성자입니다.

아래에는 개발자가 오늘 하루 동안 수행한 Git Commit 기록,
변경된 파일 목록,
그리고 실제 코드 변경 Diff가 제공됩니다.

이 정보를 분석하여 오늘 하루 동안 무엇을 구현했는지
게임 개발자의 기술 개발일지 형태로 작성하세요.

이 문서는 추후 게임 개발 포트폴리오와
프로젝트 기술 문서 작성에도 활용됩니다.


==================================================
작성 원칙
==================================================

1. 반드시 한국어로 작성합니다.

2. Markdown 형식으로 작성합니다.

3. Commit 메시지를 단순히 나열하지 않습니다.

4. 실제 코드 Diff를 우선적으로 분석합니다.

5. Diff에서 확인할 수 없는 구현 내용을 임의로 만들어내지 않습니다.

6. 클래스명, 메서드명, 시스템명 등 중요한 기술 요소는
   가능한 경우 구체적으로 언급합니다.

7. 단순히 "파일을 수정했다"라고 작성하지 말고,
   어떤 기능을 구현하거나 개선했는지 설명합니다.

8. 리팩터링이 확인된다면
   기존 구조에서 무엇이 어떻게 개선되었는지 설명합니다.

9. 버그 수정이 확인된다면
   원인과 수정 방식을 확인 가능한 범위에서 설명합니다.

10. 책임 분리, 데이터 관리, 이벤트 구조,
    상태 관리, 객체지향 설계, 확장성, 재사용성 등
    기술적으로 의미 있는 설계가 확인된다면 설명합니다.

11. 코드에서 확인할 수 없는 설계 의도는 추측하지 않습니다.

12. Git Commit 목록을 그대로 다시 출력하지 않습니다.

13. '# $date Dev Log' 제목은 작성하지 않습니다.

14. 게임 개발 포트폴리오에 활용할 수 있도록
    명확하고 전문적으로 작성하되 과장하지 않습니다.


==================================================
출력 형식
==================================================

## AI Daily Summary

### 오늘의 작업 요약

오늘 수행한 핵심 개발 작업을
2~4문장으로 요약합니다.


### 주요 구현 내용

기능 또는 시스템 단위로 핵심 구현 내용을
Bullet Point 형태로 정리합니다.


### 설계 및 기술 포인트

오늘 변경된 코드에서 확인되는

- 코드 구조
- 책임 분리
- 데이터 흐름
- 시스템 간 연결
- 객체지향 설계
- 확장성
- 재사용성

등 기술적으로 의미 있는 내용을 작성합니다.

특별한 내용이 없다면 간단하게 작성합니다.


### 문제 해결 및 개선

오늘 작업에서 확인되는

- Bug Fix
- Refactoring
- 구조 개선
- 기존 구현 개선

내용을 작성합니다.

확인되는 내용이 없다면

- 특이사항 없음

이라고 작성합니다.


### 오늘 변경한 주요 파일

중요한 파일만 선택하여

- 파일명 : 역할 및 주요 변경 내용

형태로 작성합니다.


==================================================
Today's Git Commits
==================================================

$commitList


==================================================
Today's Changed Files
==================================================

$changedFileList


==================================================
Today's Code Diff
==================================================

$diffText

"@


# ==========================================
# OpenAI Request
# ==========================================

Write-Host ""
Write-Host "Analyzing today's work with AI..."
Write-Host "Model: $model"
Write-Host "Commits: $($todayHashes.Count)"
Write-Host "Diff characters: $($diffText.Length)"


$headers = @{

    "Authorization" = "Bearer $($env:OPENAI_API_KEY)"

    "Content-Type" = "application/json"
}


$bodyObject = @{

    model = $model

    reasoning = @{

        effort = "low"
    }

    input = $prompt

    max_output_tokens = 2500
}


$body = $bodyObject |
    ConvertTo-Json -Depth 10


# ==========================================
# Call OpenAI API
# ==========================================

$response = $null


try {

    $response = Invoke-RestMethod `
        -Uri "https://api.openai.com/v1/responses" `
        -Method Post `
        -Headers $headers `
        -Body $body `
        -ContentType "application/json; charset=utf-8"

}
catch {

    Write-Host ""
    Write-Host "========================================"
    Write-Host "AI summary failed."
    Write-Host "========================================"
    Write-Host ""

    Write-Host $_.Exception.Message


    if ($_.Exception.Response) {

        try {

            $stream = $_.Exception.Response.GetResponseStream()

            if ($stream) {

                $reader = New-Object System.IO.StreamReader(
                    $stream
                )

                $errorBody = $reader.ReadToEnd()

                $reader.Close()


                if (-not [string]::IsNullOrWhiteSpace($errorBody)) {

                    Write-Host ""
                    Write-Host "API Error:"
                    Write-Host $errorBody
                }
            }
        }
        catch {
        }
    }


    Write-Host ""
    Write-Host "기존 Git 개발일지는 그대로 유지됩니다."
    Write-Host ""

    exit
}


# ==========================================
# Extract AI Response
# ==========================================

$aiSummary = ""


if ($response -and $response.output) {

    foreach ($output in $response.output) {

        if ($output.type -eq "message") {

            foreach ($content in $output.content) {

                if ($content.type -eq "output_text") {

                    $aiSummary += $content.text
                }
            }
        }
    }
}


# ==========================================
# Empty Response Check
# ==========================================

if ([string]::IsNullOrWhiteSpace($aiSummary)) {

    Write-Host ""
    Write-Host "========================================"
    Write-Host "AI response is empty."
    Write-Host "========================================"

    Write-Host ""
    Write-Host "AI 응답에서 텍스트를 찾지 못했습니다."
    Write-Host "기존 개발일지는 그대로 유지됩니다."
    Write-Host ""

    exit
}


# ==========================================
# Create Output Directory
# ==========================================

if (!(Test-Path -LiteralPath $outputDir)) {

    New-Item `
        -ItemType Directory `
        -Force `
        -Path $outputDir |
        Out-Null
}


# ==========================================
# Create Dev Log If Missing
# ==========================================

if (!(Test-Path -LiteralPath $outputFile)) {

    $header = @"
# $date Dev Log

"@

    [System.IO.File]::WriteAllText(
        $outputFile,
        $header,
        $utf8
    )
}


# ==========================================
# Read Existing Dev Log
# ==========================================

$existingContent = [System.IO.File]::ReadAllText(
    $outputFile,
    [System.Text.Encoding]::UTF8
)


# ==========================================
# AI Summary Markers
# ==========================================

$startMarker = "<!-- AI_SUMMARY_START -->"

$endMarker = "<!-- AI_SUMMARY_END -->"


# ==========================================
# Remove Previous AI Summary
# ==========================================

$pattern = "(?s)" +
    [regex]::Escape($startMarker) +
    ".*?" +
    [regex]::Escape($endMarker) +
    "\s*(?:---\s*)?"


$existingContent = [regex]::Replace(
    $existingContent,
    $pattern,
    ""
)


# ==========================================
# Prepare AI Section
# ==========================================

$aiSection = @"
$startMarker

$aiSummary

$endMarker

---

"@


# ==========================================
# Insert AI Summary Under Main Header
# ==========================================

$headerPattern = "(?m)^(# .+?\r?\n)"


if ($existingContent -match $headerPattern) {

    $newContent = [regex]::Replace(
        $existingContent,
        $headerPattern,
        "`$1`r`n$aiSection",
        1
    )

}
else {

    $newContent = @"
# $date Dev Log

$aiSection
$existingContent
"@
}


# ==========================================
# Save Markdown UTF-8
# ==========================================

[System.IO.File]::WriteAllText(
    $outputFile,
    $newContent,
    $utf8
)


# ==========================================
# Complete
# ==========================================

Write-Host ""
Write-Host "========================================"
Write-Host "AI Daily Summary created successfully."
Write-Host "========================================"

Write-Host ""
Write-Host "Date: $date"
Write-Host "Commits analyzed: $($todayHashes.Count)"
Write-Host "Model: $model"
Write-Host "File: $outputFile"
Write-Host ""