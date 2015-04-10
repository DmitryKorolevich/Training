﻿$rootDeploy = "c:\inetpub\wwwroot\vitalchoice_new"

ls -Path "${rootDeploy}\" | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo") -And -Not ($_.Name.Equals("logs"))) {
		$target = $_.Name
		echo "starting ${target}"
		Push-Location "${rootDeploy}\${target}\"
		start kestrel.cmd | tee "..\logs\${target}.log"
		Pop-Location
	}
}