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
	dnu restore --parallel > restore.log
	echo "Publishing project..."
	dnu publish -o "${deployPath}" --runtime active --configuration Debug --no-source --wwwroot wwwroot
	Pop-Location
}
function GruntTask($taskName) {
	Push-Location ".."
	echo "Running grunt task ${taskName}..."
	grunt $taskName > grunt.log
	Pop-Location
}
function BowerInstall() {
	Push-Location ".."
	echo "Running bower install..."
	bower install > bower.log
	Pop-Location
}
function NpmCopy($npmPath) {
	if (-Not (test-path $npmPath)) {
		ni -itemtype directory -path $npmPath -Force
		Push-Location ".."
		echo "Installing missed packages..."
		npm install > npm.log
		Pop-Location
		robocopy "..\node_modules" $npmPath /e /ndl /nfl /njh /is > copy-npm.log
	}
	else {
		echo "Copying npm..."
		robocopy $npmPath "..\node_modules" /e /ndl /nfl /njh /is > copy-npm.log
		Push-Location ".."
		echo "Installing missed packages..."
		npm install > npm.log
		Pop-Location
	}
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

function RestoreRuntime($deployPath) {
	echo "Restoring missed runtimes..."
	ls -Path "${deployPath}\approot\runtimes" | `
	foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo")) {
			$runtimeName = $_.Name
			if (-Not(test-path "${deployPath}\${runtimeName}\bin\dnx.clr.managed.dll")) {
				cp "${env:USERPROFILE}\.dnx\runtimes\${runtimeName}" "${deployPath}\approot\runtimes" -Force -Recurse
			}
		}
	}
}