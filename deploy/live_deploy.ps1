Param(
	[string]$RootDeploy
)

$ConfigFolder = "test_config"

if ($RootDeploy.Equals("")) {
	$RootDeploy = "c:\inetpub\wwwroot"
}

$filesLinkSource = "c:\inetpub\wwwroot\content\files"
$designLinkSource = "c:\inetpub\wwwroot\content\design"
cmd /c mkdir "C:\Temp\Backup\public"
cmd /c mkdir "C:\Temp\Backup\admin"
cmd /c mkdir "C:\Temp\Backup\jobs"

if (test-path ".\packages\admin.7z")
{
	robocopy "empty\" "C:\Temp\Backup\admin\" /mir /nfl /ndl /njh >> clean.log
	robocopy "empty\" "C:\Temp\Deploy\admin\" /mir /nfl /ndl /njh >> clean.log

	cmd /c 7z x -r .\packages\admin.7z -o"C:\Temp\Deploy\admin"
	if(-Not $?){exit $LASTEXITCODE}

	cmd /c "%windir%\system32\inetsrv\appcmd" stop apppool /apppool.name:admin
	cmd /c rmdir "${RootDeploy}\admin\wwwroot\files"
	cmd /c rmdir "${RootDeploy}\admin\wwwroot\design"
	robocopy "${RootDeploy}\admin\" "C:\Temp\Backup\admin\" /xf "nlog.config" /e /move /nfl /ndl /njh >> backup.log
	robocopy "C:\Temp\Deploy\admin\" "${RootDeploy}\admin\" /e /move /nfl /ndl /njh >> deploy.log
	$destinationPath = $RootDeploy + "\admin\wwwroot\files"
	cmd /c mklink /D $destinationPath "${filesLinkSource}"
	$destinationPath = $RootDeploy + "\admin\wwwroot\design"
	cmd /c mklink /D $destinationPath "${designLinkSource}"
	cp libuv.dll "c:\inetpub\wwwroot\admin\" -Force
	cp ".\${ConfigFolder}\admin.config.json" "c:\inetpub\wwwroot\admin\config.json" -Force
	cmd /c "%windir%\system32\inetsrv\appcmd" start apppool /apppool.name:admin
}

if (test-path ".\packages\public.7z")
{
	robocopy "empty\" "C:\Temp\Backup\public\" /mir /nfl /ndl /njh >> clean.log
	robocopy "empty\" "C:\Temp\Deploy\public\" /mir /nfl /ndl /njh >> clean.log

	cmd /c 7z x -r .\packages\public.7z -o"C:\Temp\Deploy\public"
	if(-Not $?){exit $LASTEXITCODE}

	cmd /c "%windir%\system32\inetsrv\appcmd" stop apppool /apppool.name:public
	cmd /c rmdir "${RootDeploy}\public\wwwroot\files"
	cmd /c rmdir "${RootDeploy}\public\wwwroot\design"
	robocopy "${RootDeploy}\public\" "C:\Temp\Backup\public\" /xf "nlog.config" /e /move /nfl /ndl /njh >> backup.log
	robocopy "C:\Temp\Deploy\public\" "${RootDeploy}\public\" /e /move /nfl /ndl /njh >> deploy.log
	$destinationPath = $RootDeploy + "\public\wwwroot\files"
	cmd /c mklink /D $destinationPath "${filesLinkSource}"
	$destinationPath = $RootDeploy + "\public\wwwroot\design"
	cmd /c mklink /D $destinationPath "${designLinkSource}"
	cp libuv.dll "c:\inetpub\wwwroot\public\" -Force
	cp ".\${ConfigFolder}\public.config.json" "c:\inetpub\wwwroot\public\config.json" -Force
	cmd /c "%windir%\system32\inetsrv\appcmd" start apppool /apppool.name:public
}

if (test-path ".\packages\jobs.7z")
{
	robocopy "empty\" "C:\Temp\Backup\jobs\" /mir /nfl /ndl /njh >> clean.log
	robocopy "empty\" "C:\Temp\Deploy\jobs\" /mir /nfl /ndl /njh >> clean.log

	cmd /c 7z x -r .\packages\jobs.7z -o"C:\Temp\Deploy\jobs"
	if(-Not $?){exit $LASTEXITCODE}

	net stop jobsService
	
	$process = Get-Process "VitalChoice.Jobs" -ErrorAction SilentlyContinue
	while ($process) {
		Sleep 1
		$process = Get-Process "VitalChoice.Jobs" -ErrorAction SilentlyContinue
	}

	robocopy "C:\Program Files\VitalChoice.Jobs" C:\Temp\Backup\jobs\ /e /move /nfl /ndl /njh >> backup.log
	robocopy C:\Temp\Deploy\jobs\ "C:\Program Files\VitalChoice.Jobs" /e /move /nfl /ndl /njh >> deploy.log
	cp ".\${ConfigFolder}\jobs.config.json" "C:\Program Files\VitalChoice.Jobs\config.json" -Force

	net start jobsService
}