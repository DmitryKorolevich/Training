Param(
	[string]$RootDeploy,
	[string]$Src
)

. ".\functions.ps1"
. ".\exclude_projects.ps1"

if ($RootDeploy.Equals("")) {
	$RootDeploy = "\deploy_temp\"
}
if ($Src.Equals("")) {
	$Src = "..\.."
}
$RootBuild = "${env:TEMP}\vc\build\"
robocopy "${Src}" "${RootBuild}" /xd "artifacts" "bin" "obj" ".git" ".vs" /e /purge /nfl /ndl /njh /r:2 /w:1
kvm upgrade -r coreclr -amd64
kpm restore "${RootBuild}"
kvm upgrade -r clr -amd64
ls -Path "${RootBuild}\src" | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo")) {
		$projectName = $_.Name
		if (!$exclude.Any($_.Equals($projectName))) {
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