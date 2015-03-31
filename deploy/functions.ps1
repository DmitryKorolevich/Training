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
	Push-Location ".."
	dnu restore --parallel
	dnu publish -o "${deployPath}"
	Pop-Location
}
function GruntTask($taskName) {
	grunt -b ".." --gruntfile "..\gruntfile.js" ${taskName}
}
function BowerInstall() {
	Push-Location ".."
	bower install
	Pop-Location
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