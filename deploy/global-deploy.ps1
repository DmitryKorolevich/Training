Param(
	[string]$RootDeploy,
	[string]$Src
)

. ".\functions.ps1"
. ".\exclude_projects.ps1"

if ($RootDeploy.Equals("")) {
	$RootDeploy = "c:\inetpub\wwwroot\vitalchoice_new"
}
if ($Src.Equals("")) {
	$Src = ".."
}
$RootBuild = "C:\Temp\vc"
robocopy "${Src}" "${RootBuild}" /xd "artifacts" "bin" "obj" ".git" ".vs" /e /purge /nfl /ndl /njh /r:2 /w:1
dnu restore "${RootBuild}"
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
			}
		}
	}
}