# Windows Documents 폴더 자동 탐색
$documents = [Environment]::GetFolderPath("MyDocuments")

# Obsidian Vault 경로
$vault = Join-Path $documents "Obsidian Vault\DevLog"

# 프로젝트 이름
$project = "Genshin-Clone-Unity"

# 현재 날짜 / 시간
$date = Get-Date -Format "yyyy-MM-dd"
$time = Get-Date -Format "HH:mm"

# 저장 경로
# DevLog\Projects\Genshin-Clone-Unity
$outputDir = Join-Path $vault "Projects"
$outputDir = Join-Path $outputDir $project

# 오늘 날짜의 Markdown 파일
$outputFile = Join-Path $outputDir "$date.md"


# ==========================================
# 폴더 생성
# ==========================================

if (!(Test-Path -LiteralPath $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}


# ==========================================
# Git 정보 가져오기
# ==========================================

# 마지막 Commit 메시지
$commit = git log -1 --pretty=format:"%s"

# 마지막 Commit Hash
$hash = git log -1 --pretty=format:"%h"

# 현재 Branch
$branch = git branch --show-current

# 마지막 Commit에서 변경된 파일
$files = git diff-tree --no-commit-id --name-only -r HEAD


# ==========================================
# 오늘의 개발일지가 없으면 생성
# ==========================================

if (!(Test-Path -LiteralPath $outputFile)) {

    $header = @"
# $date Dev Log

"@

    [System.IO.File]::WriteAllText(
        $outputFile,
        $header,
        [System.Text.UTF8Encoding]::new($false)
    )
}


# ==========================================
# 변경 파일 목록
# ==========================================

$fileList = $files -join "`r`n"


# ==========================================
# 개발일지 내용
# ==========================================

$content = @"

## $time

### Commit
$commit

### Commit Hash
$hash

### Branch
$branch

### Changed Files
$fileList

---

"@


# ==========================================
# Markdown 파일에 추가
# ==========================================

[System.IO.File]::AppendAllText(
    $outputFile,
    $content,
    [System.Text.UTF8Encoding]::new($false)
)


# ==========================================
# 완료 메시지
# ==========================================

Write-Host ""
Write-Host "Dev log created successfully."
Write-Host "File: $outputFile"