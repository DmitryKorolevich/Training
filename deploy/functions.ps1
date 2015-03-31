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
function DnuAll($deployPath) {
	dnu restore ..\project.json
	dnu publish ..\project.json -o "${deployPath}"
}
function GruntTask($taskName) {
	grunt -b ".." --gruntfile "..\gruntfile.js" ${taskName}
}
function BuildDbProject() {
	
}
function Any($name, $inlist) {
	foreach($item in $inlist) {
		if ($name.Equals($item)) { 
			return true;
		}
	}
	return false;
}