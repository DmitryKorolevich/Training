$rootDeploy = "c:\inetpub\wwwroot\vitalchoice_new"

ls -Path "${rootDeploy}\" | `
foreach{
	if ($_.GetType().Name.Equals("DirectoryInfo") -And -Not ($_.Name.Equals("logs"))) {
		$target = $_.Name
		echo "starting ${target}"
		Push-Location "${rootDeploy}\${target}\"
		start .\kestrel -NoNewWindow -RedirectStandardOutput "..\logs\${target}.log" -RedirectStandardError "..\logs\${target}_errors.log"
		Pop-Location
	}
}