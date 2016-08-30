. ".\functions.ps1"

robocopy "empty\" "C:\Temp\Deploy\public\" /mir /nfl /ndl /njh >> clean.log
robocopy "empty\" "C:\Temp\Deploy\admin\" /mir /nfl /ndl /njh >> clean.log

Push-Location ".."
dotnet restore -v Warning
Pop-Location

Push-Location "..\src\VC.Admin\deploy"

BowerInstall
GruntTask -taskName "bower:install"
GruntTask -taskName "release"
BuildAll -deployPath "C:\Temp\Deploy\admin"

Pop-Location

Push-Location "..\src\VC.Public\deploy"

BowerInstall
GruntTask -taskName "bower:install"
GruntTask -taskName "release"
BuildAll -deployPath "C:\Temp\Deploy\public"

Pop-Location

Push-Location "..\src\VitalChoice.Jobs\Jobs"
BuildAll -deployPath "C:\Temp\Deploy\jobs"
Pop-Location

Push-Location "..\src\VitalChoice.ExportService\Services"
BuildAll -deployPath "C:\Temp\Deploy\export"
Pop-Location

cmd /c del /S /F C:\Temp\Deploy\*.pdb

rm C:\Temp\Deploy\admin.7z -Force
rm C:\Temp\Deploy\public.7z -Force
rm C:\Temp\Deploy\jobs.7z -Force
rm C:\Temp\Deploy\export.7z -Force

cmd /c "C:\Program Files\7-Zip\7z.exe" a C:\Temp\Deploy\admin.7z C:\Temp\Deploy\admin\** -m0=lzma2
cmd /c "C:\Program Files\7-Zip\7z.exe" a C:\Temp\Deploy\public.7z C:\Temp\Deploy\public\** -m0=lzma2
cmd /c "C:\Program Files\7-Zip\7z.exe" a C:\Temp\Deploy\jobs.7z C:\Temp\Deploy\jobs\** -m0=lzma2
cmd /c "C:\Program Files\7-Zip\7z.exe" a C:\Temp\Deploy\export.7z C:\Temp\Deploy\export\** -m0=lzma2