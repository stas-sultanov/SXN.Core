# Enable -Verbose option
[CmdletBinding()]

#param([String] $nuget, [String] $apiKey, [String] $workingDir)

param([String] $apiKey, [String] $workingDir)

$nuget = 'NuGet.exe'

# Check if path to nuget.exe is specified
if (-not $nuget)
{
	Write-Error ("The path to NuGet.exe is not specified.")

	exit 1
}

# Check if working directory is specified
if (-not $workingDir)
{
	Write-Error ("The output directory is not specified.")

	exit 1
}

# Set api key
Invoke-Expression "$nuget setApiKey $apiKey"

# Enumerate nuspec files in build output directory
$nuspecFiles = Get-ChildItem -Path $workingDir -Recurse -File -Include '*.nuspec'

foreach ($nuspecFile in $nuspecFiles)
{
	# Pack NuGet package
	Invoke-Expression "$nuget pack $nuspecFile -Verbosity detailed -Symbols -OutputDirectory $workingDir"
}

# Remove not nupkg files and folders
Get-ChildItem -Path $workingDir -Recurse -Exclude '*.nupkg' | Remove-Item -Force -Recurse

# Enumerate packages files
$packageFiles = Get-ChildItem -Path $workingDir -Recurse -File -Include '*.nupkg' -Exclude '*.symbols.nupkg'

foreach ($file in $packageFiles)
{
	# Push symbols files
	Invoke-Expression "$nuget push $file"

	# Delete file
	# Remove-Item $file -Force
}
