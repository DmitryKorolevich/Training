. ".\functions.ps1"

$rootDeploy = "c:\inetpub\wwwroot\vitalchoice_new"

ls -Path $rootDeploy | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo") -And -Not ($_.Name.Equals("log"))) {
		Push-Location "${rootDeploy}\" + $_.Name + "\"
		start kestrel
		Pop-Location
	}
}