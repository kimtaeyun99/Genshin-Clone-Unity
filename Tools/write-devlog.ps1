# ==========================================
# Basic Settings
# ==========================================

$documents = [Environment]::GetFolderPath("MyDocuments")

$vault = Join-Path $documents "Obsidian Vault\DevLog"

$project = "Genshin-Clone-Unity"

$date = Get-Date -Format "yyyy-MM-dd"
$time = Get-Date -Format "HH:mm"


# ==========================================
# Output Path
# ==========================================

$outputDir = Join-Path $vault "Projects"
$outputDir = Join-Path $outputDir $project

$outputFile = Join-Path $outputDir "$date.md"


if (!(Test-Path -LiteralPath $outputDir)) {
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null
}


# ==========================================
# Git Information
# ==========================================

$commit = git log -1 --pretty=format:"%s"

$hash = git log -1 --pretty=format:"%h"

$branch = git branch --show-current

$files = git diff-tree `
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
        [System.Text.UTF8Encoding]::new($false)
    )
}


# ==========================================
# Changed Files
# ==========================================

$fileList = ""

foreach ($file in $files) {
    $fileList += "- $file`r`n"
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
    [System.Text.UTF8Encoding]::new($false)
)


Write-Host ""
Write-Host "Dev log created successfully."
Write-Host "File: $outputFile"