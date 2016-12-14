. ".\functions.ps1"

robocopy "empty\" "C:\Temp\Deploy\export\" /mir /nfl /ndl /njh >> clean.log

Push-Location ".."
dotnet restore -v Warning
Pop-Location

Push-Location "..\src\VitalChoice.ExportService\Services"
BuildAll -deployPath "C:\Temp\Deploy\export"
Pop-Location

cmd /c del /S /F C:\Temp\Deploy\*.pdb

rm C:\Temp\Deploy\export.7z -Force

cmd /c "C:\Program Files\7-Zip\7z.exe" a C:\Temp\Deploy\export.7z C:\Temp\Deploy\export\** -m0=lzma2