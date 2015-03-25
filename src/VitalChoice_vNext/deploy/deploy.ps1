Param(
	[string]$RootDeploy
)
if ($RootDeploy.Equals("")) {
	throw "Root deploy is empty"
}
$targetNames = GetTargets
foreach ($target in $targetNames) {
	CopyTarget -targetName $target
	GruntTask -taskName "development"
	KpmAll -deployPath "${RootDeploy}${target}"
}