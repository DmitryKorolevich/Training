Param(
	[string]$RootDeploy
)

. "..\..\..\deploy\functions.ps1"

if ($RootDeploy.Equals("")) {
	throw "Root deploy is empty"
}
NpmCopy -npmPath "C:\Temp\node_modules\"
BowerInstall
$targetNames = GetTargets
foreach ($target in $targetNames) {
	CopyTarget -targetName $target
	GruntTask -taskName "bower:install"
	GruntTask -taskName "release"
	cmd /c "%windir%\system32\inetsrv\appcmd" stop apppool /apppool.name:admin
	BuildAll -deployPath "${RootDeploy}\${target}"
}