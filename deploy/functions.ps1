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
	robocopy "${targetName}\" ".." /e /ndl /nfl /njh /is /it
}
function DnuAll($deployPath) {
	Push-Location ".."
	dnu restore --parallel
	dnu publish -o "${deployPath}"
	Pop-Location
}
function GruntTask($taskName) {
	Push-Location ".."
	grunt $taskName
	Pop-Location
}
function BowerInstall() {
	Push-Location ".."
	bower install
	Pop-Location
}
function NpmInstall() {
	Push-Location ".."
	npm install
	Pop-Location
}
function BuildDbProject() {
	
}
function Any($name, $inlist) {
	foreach($item in $inlist) {
		if ($name.Equals($item)) { 
			return $TRUE;
		}
	}
	return $FALSE;
}