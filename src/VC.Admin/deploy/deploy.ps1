Param(
	[string]$RootDeploy
)
if ($RootDeploy.Equals("")) {
	throw "Root deploy is empty"
}
NpmCopy -npmPath "D:\Temp\npm\"
BowerInstall
$targetNames = GetTargets
foreach ($target in $targetNames) {
	CopyTarget -targetName $target
	GulpTask -taskName "release"
	BuildAll -deployPath "${RootDeploy}\${target}"
}