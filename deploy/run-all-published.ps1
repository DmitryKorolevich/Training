$rootDeploy = "c:\inetpub\wwwroot\vitalchoice_new"

ls -Path $rootDeploy | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo") -And -Not ($_.Name.Equals("log"))) {
		$target = $_.Name
		Push-Location "${rootDeploy}\${target}\"
		start kestrel
		Pop-Location
	}
}