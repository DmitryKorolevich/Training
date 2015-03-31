Param(
	[string]$RootDeploy
)
if ($RootDeploy.Equals("")) {
	throw "Root deploy is empty"
}
NpmCopy
BowerInstall
$targetNames = GetTargets
foreach ($target in $targetNames) {
	CopyTarget -targetName $target
	GruntTask -taskName "bower-install"
	GruntTask -taskName "development"
	DnuAll -deployPath "${RootDeploy}\${target}"
}