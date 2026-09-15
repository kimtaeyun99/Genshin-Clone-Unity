# ==========================================
# Git Commit -> Obsidian Dev Log
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
# Basic Settings
# ==========================================

$documents = [Environment]::GetFolderPath("MyDocuments")

$vault = Join-Path $documents "Obsidian Vault\DevLog"

$project = "Genshin-Clone-Unity"


# ==========================================
# Development Day
#
# 06:00 ~ 다음날 05:59를
# 하나의 개발일로 처리
# ==========================================

$now = Get-Date

if ($now.Hour -lt 6) {

    # 새벽 0시 ~ 5시 59분이면 전날 개발일
    $devDate = $now.AddDays(-1)

}
else {

    $devDate = $now
}


$date = $devDate.ToString("yyyy-MM-dd")

# 실제 Commit 시간은 현재 시간 그대로 기록
$time = $now.ToString("HH:mm")


# ==========================================
# Output Path
# ==========================================

$outputDir = Join-Path $vault "Projects"
$outputDir = Join-Path $outputDir $project

$outputFile = Join-Path $outputDir "$date.md"


if (!(Test-Path -LiteralPath $outputDir)) {

    New-Item `
        -ItemType Directory `
        -Force `
        -Path $outputDir |
        Out-Null
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
# Git Information
# ==========================================

$commit = git `
    -c i18n.logOutputEncoding=utf-8 `
    log -1 `
    --pretty=format:"%s"


$hash = git log -1 --pretty=format:"%h"


$branch = git branch --show-current


$files = git `
    -c core.quotepath=false `
    diff-tree `
    --no-commit-id `
    --name-only `
    -r HEAD


# ==========================================
# Create Daily Log
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
# Changed Files
# ==========================================

$fileList = ""


foreach ($file in $files) {

    if (-not [string]::IsNullOrWhiteSpace($file)) {

        $fileList += "- $file`r`n"
    }
}


if ([string]::IsNullOrWhiteSpace($fileList)) {

    $fileList = "- 변경 파일 없음`r`n"
}


# ==========================================
# Commit Log
# ==========================================

$content = @"

## $time

### Commit
$commit

### Commit Hash
``$hash``

### Branch
``$branch``

### Changed Files
$fileList

---

"@


# ==========================================
# Save
# ==========================================

[System.IO.File]::AppendAllText(
    $outputFile,
    $content,
    $utf8
)


# ==========================================
# Complete
# ==========================================

Write-Host ""
Write-Host "Dev log created successfully."
Write-Host "Development Day: $date"
Write-Host "File: $outputFile"