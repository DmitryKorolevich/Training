Param(
	[string]$Src
)

. ".\functions.ps1"

if ($Src.Equals("")) {
	$Src = ".."
}
$RootBuild = "C:\Temp\vc"
robocopy "${Src}" "${RootBuild}" /xd "artifacts" "bin" "obj" ".git" ".vs" /mir /is /it /nfl /ndl /njh /r:2 /w:1
dnu restore "${RootBuild}\" --parallel >> restore.log
ls -Path "${RootBuild}\src" | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo")) {
		$projectName = $_.Name
		$isExclude = Any -name $projectName -inlist $exclude
		if (!$isExclude) {
			$project = $RootBuild + "\src\" + $projectName
			$deployScript = $project + "\deploy\deploy.ps1"
			if (test-path $deployScript) {
				Push-Location $project
				if (test-path "project.json") {
					dnu build
				}
				Pop-Location
			}
		}
	}
}