param($target = "C:\Dev\Github\WireMock.Net")

$header = "// Copyright © WireMock.Net
"

function Write-Header ($file)
{
    $content = Get-Content $file
    $filename = Split-Path -Leaf $file
    $fileheader = $header
    Set-Content $file $fileheader
    Add-Content $file $content
}

Get-ChildItem $target -Recurse | ? { $_.Extension -like ".cs" } | % `
{
    Write-Header $_.PSPath.Split(":", 3)[2]
}