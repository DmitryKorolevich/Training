Param(
	[string]$RootDeploy,
	[string]$Src,
	[string]$RootBuild
)

. ".\functions.ps1"
. ".\exclude_projects.ps1"

if ($RootDeploy.Equals("")) {
	$RootDeploy = "c:\inetpub\wwwroot\vitalchoice_rc2"
}
if ($Src.Equals("")) {
	$Src = ".."
}
$RootBuild = (Get-Item -Path ".\${Src}" -Verbose).FullName
$filesLinkSource = "$RootDeploy" + "\..\vitalchoice_new\files"
$designLinkSource = "$RootDeploy" + "\..\vitalchoice_new\design"
echo "Clean deploy directory..."
cmd /c rmdir "${RootDeploy}\blue\wwwroot\files"
cmd /c rmdir "${RootDeploy}\public\wwwroot\files"
cmd /c rmdir "${RootDeploy}\blue\wwwroot\design"
cmd /c rmdir "${RootDeploy}\public\wwwroot\design"
robocopy "empty\" "${RootDeploy}\" /xd "logs" "files" "design" /xf "nlog.config" /mir /nfl /ndl /njh > clean.log
ni -itemtype directory -path "${RootDeploy}\logs\" -Force
#cp "${RootBuild}\src\nlog.config" "${RootDeploy}\nlog.config"
Push-Location "${RootBuild}"
echo "Working directory: ${RootBuild}"
echo "Restoring packages..."
dotnet restore -v Warning
if(-Not $?)
{
	exit $LASTEXITCODE
}
Pop-Location
ls -Path "${RootBuild}\src" | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo")) {
		$projectName = $_.Name
		$isExclude = Any -name $projectName -inlist $exclude
		if (!$isExclude) {
			$project = $RootBuild + "\src\" + $projectName + "\deploy\"
			$deployScript = $project + "deploy.ps1"
			if (test-path "${deployScript}") {
				Push-Location "${project}"
				& powershell -File """${deployScript}""" -RootDeploy ${RootDeploy}
				Pop-Location
				$deployProjectPath = $RootBuild + "\src\" + $projectName + "\deploy"
				ls -Path "${deployProjectPath}" | `
				foreach{
					if ($_.GetType().Name.Equals("DirectoryInfo")) {
						$targetName = $_.Name
						$destinationPath = $RootDeploy + "\" + $targetName + "\wwwroot\files"
						cmd /c mklink /D $destinationPath "${filesLinkSource}"
						$destinationPath = $RootDeploy + "\" + $targetName + "\wwwroot\design"
						cmd /c mklink /D $destinationPath "${designLinkSource}"
						$destinationPath = $RootDeploy + "\" + $targetName
						cp libuv.dll $destinationPath -Force
					}
				}
			}
		}
	}
}
cmd /c "%windir%\system32\inetsrv\appcmd" start apppool /apppool.name:admin_dev
cmd /c "%windir%\system32\inetsrv\appcmd" start apppool /apppool.name:public_dev