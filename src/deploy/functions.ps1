function GetTargets() {
	[string[]] $targetNames = @()
	ls | `
	foreach{
		if ($_.GetType().Name.Equals("DirectoryInfo")) {
			$target = $_.Name
			$targetNames += $target
		}
	}
	return $targetNames
}
function CopyTarget($targetName) {
	robocopy "${targetName}\" ".." /e /ndl /nfl /njh
}
function KpmAll($deployPath) {
	kpm restore ..\project.json
	kpm build ..\project.json
	kpm bundle ..\project.json -o "${deployPath}"
}
function GruntTask($taskName) {
	grunt -b ".." --gruntfile "..\gruntfile.js" ${taskName}
}
function BuildDbProject() {
	
}