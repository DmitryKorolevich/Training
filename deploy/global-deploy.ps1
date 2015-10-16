Param(
	[string]$RootDeploy,
	[string]$Src,
	[string]$RootBuild
)

. ".\functions.ps1"
. ".\exclude_projects.ps1"

if ($RootDeploy.Equals("")) {
	$RootDeploy = "c:\inetpub\wwwroot\vitalchoice_new"
}
if ($Src.Equals("")) {
	$Src = ".."
}
if ($RootBuild.Equals("")) {
	$RootBuild = "C:\Temp\vc"
}
$filesLinkSource = "$RootDeploy" + "\files"
$designLinkSource = "$RootDeploy" + "\design"
ni -itemtype directory -path "empty" -Force
echo "Clean temp..."
robocopy "empty\" "${RootBuild}\" /mir /nfl /ndl /njh > clean.log
echo "Clean deploy directory..."
cmd /c rmdir "${RootDeploy}\blue\wwwroot\files"
cmd /c rmdir "${RootDeploy}\public\wwwroot\files"
cmd /c rmdir "${RootDeploy}\blue\wwwroot\design"
cmd /c rmdir "${RootDeploy}\public\wwwroot\design"
robocopy "empty\" "${RootDeploy}\" /xd "logs" "files" "design" /mir /nfl /ndl /njh > clean.log
echo "Copy checkout files to temp..."
robocopy "${Src}" "${RootBuild}" /xd "artifacts" "bin" "obj" ".git" ".vs" /xf "project.lock.json" /mir /nfl /ndl /njh /is /it /r:2 /w:1 > copy.log
ni -itemtype directory -path "${RootDeploy}\logs\" -Force
cp "${RootBuild}\src\nlog.config" "${RootDeploy}\nlog.config"
if (-Not(test-path "${RootDeploy}\logs\Logs.mdf")) {
	cp "${RootBuild}\src\Logs_log.ldf" "${RootDeploy}\logs\Logs_log.ldf"
	cp "${RootBuild}\src\Logs.mdf" "${RootDeploy}\logs\Logs.mdf"
}
Push-Location "${RootBuild}"
echo "Restoring packages..."
dnu restore --parallel >> restore.log
Pop-Location
ls -Path "${RootBuild}\src" | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo")) {
		$projectName = $_.Name
		$isExclude = Any -name $projectName -inlist $exclude
		if (!$isExclude) {
			$project = $RootBuild + "\src\" + $projectName + "\deploy\"
			$deployScript = $project + "deploy.ps1"
			if (test-path $deployScript) {
				Push-Location ${project}
				iex "${deployScript} -RootDeploy ${RootDeploy}"
				Pop-Location
				$deployProjectPath = $RootBuild + "\src\" + $projectName + "\deploy"
				ls -Path "${deployProjectPath}" | `
				foreach{
					if ($_.GetType().Name.Equals("DirectoryInfo")) {
						$targetName = $_.Name
						$destinationPath = $RootDeploy + "\" + $targetName + "\wwwroot\files"
						cmd /c mklink /D $destinationPath $filesLinkSource
						$destinationPath = $RootDeploy + "\" + $targetName + "\wwwroot\design"
						cmd /c mklink /D $destinationPath $designLinkSource
					}
				}
			}
		}
	}
}