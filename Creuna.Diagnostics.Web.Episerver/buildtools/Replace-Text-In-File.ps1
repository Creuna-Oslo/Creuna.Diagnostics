param (
	[string]$srcFile,
	[string]$destFile,
	[string]$oldString,
	[string]$newString
)

Write-Host "$srcFile $destFile $oldString $newString"
Get-Content $srcFile | Foreach-Object {$_ -replace([regex]::escape("$oldString"), "$newString")} | Set-Content $destFile
