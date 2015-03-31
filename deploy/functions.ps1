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
	robocopy "${targetName}\" ".." /e /ndl /nfl /njh /is /it >> copy.log
}
function DnuAll($deployPath) {
	Push-Location ".."
	dnu restore --parallel >> restore.log
	dnu publish -o "${deployPath}"
	Pop-Location
}
function GruntTask($taskName) {
	Push-Location ".."
	grunt $taskName >> grunt.log
	Pop-Location
}
function BowerInstall() {
	Push-Location ".."
	bower install >> bower.log
	Pop-Location
}
function NpmCopy($projectPath) {
	robocopy "c:\inetpub\temp\npm\" ".." /e /ndl /nfl /njh /is /it
	Push-Location ".."
	npm install >> npm.log
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