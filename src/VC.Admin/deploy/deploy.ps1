Param(
	[string]$RootDeploy
)
if ($RootDeploy.Equals("")) {
	throw "Root deploy is empty"
}
$targetNames = GetTargets
foreach ($target in $targetNames) {
	CopyTarget -targetName $target
	BowerInstall
	NpmCopy
	GruntTask -taskName "bower-install"
	GruntTask -taskName "development"
	DnuAll -deployPath "${RootDeploy}\${target}"
}