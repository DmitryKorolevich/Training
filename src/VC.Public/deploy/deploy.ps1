Param(
	[string]$RootDeploy
)

. "..\..\..\deploy\functions.ps1"

if ($RootDeploy.Equals("")) {
	throw "Root deploy is empty"
}
NpmCopy -npmPath "D:\Temp\npm\"
BowerInstall
$targetNames = GetTargets
foreach ($target in $targetNames) {
	CopyTarget -targetName $target
	GruntTask -taskName "bower:install"
	GruntTask -taskName "release"
	cmd /c "%windir%\system32\inetsrv\appcmd" stop apppool /apppool.name:public
	BuildAll -deployPath "${RootDeploy}\${target}"
}