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
	echo "Replace with target files..."
	cp "${targetName}\*" -Destination ".." -Recurse -Force
}
function DnuAll($deployPath) {
	Push-Location ".."
	echo "Restoring project packages..."
	dnu restore --parallel >> restore.log
	echo "Publishing project..."
	dnu publish -o "${deployPath}" >> publish.log
	Pop-Location
}
function GruntTask($taskName) {
	Push-Location ".."
	echo "Running grunt task..."
	grunt $taskName >> grunt.log
	Pop-Location
}
function BowerInstall() {
	Push-Location ".."
	echo "Running bower install..."
	bower install >> bower.log
	Pop-Location
}
function NpmCopy($projectPath) {
	echo "Copying npm..."
	robocopy "c:\inetpub\temp\npm\" ".." /e /ndl /nfl /njh /is >> copy-npm.log
	Push-Location ".."
	echo "Installing missed packages..."
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