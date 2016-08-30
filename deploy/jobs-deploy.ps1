Param(
	[string]$RootDeploy,
	[string]$Src,
	[string]$RootBuild,
	[string]$JobsFolder
)

. ".\functions.ps1"

if ($RootDeploy.Equals("")) {
	$RootDeploy = "c:\WindowsService\temp"
}
if ($Src.Equals("")) {
	$Src = ".."
}
if ($JobsFolder.Equals("")) {
	$JobsFolder = "c:\WindowsService\VitalChoice.Jobs"
}

echo "Jobs stop..."
net stop jobsService

$RootBuild = (Get-Item -Path ".\${Src}" -Verbose).FullName
echo "Clean deploy directory..."
ls -Path ${RootDeploy} -Include * | remove-Item -recurse 
cp "${RootBuild}\src\VitalChoice.Jobs\nlog.config" "${RootDeploy}\nlog.config"
cp "${RootBuild}\src\VitalChoice.Jobs\5083717e58d4513a8e8ca2992840b54ee728d674-privatekey.p12" "${RootDeploy}\5083717e58d4513a8e8ca2992840b54ee728d674-privatekey.p12"

Push-Location "${RootBuild}"
echo "Working directory: ${RootBuild}"
echo "Restoring packages..."
dotnet restore -v Warning
if(-Not $?)
{
	exit $LASTEXITCODE
}
Pop-Location

Push-Location "${RootBuild}\src\VitalChoice.Jobs\Jobs"
BuildAll -deployPath "${RootDeploy}"
Pop-Location

$process = Get-Process "VitalChoice.Jobs" -ErrorAction SilentlyContinue
while ($process) {
	Sleep 1
	$process = Get-Process "VitalChoice.Jobs" -ErrorAction SilentlyContinue
}

echo "Clean jobs directory..."
ls -Path ${JobsFolder} -Include * | remove-Item -recurse 
echo "Copy to jobs directory..."
cp "$RootDeploy\*" $JobsFolder -recurse

echo "Jobs start..."
net start jobsService