@echo off
cd "%1\src\WorkerBuilder"
if not exist "%1\artifacts\bin\ExportWorkerRoleWithSBQueue\" mkdir "%1\artifacts\bin\ExportWorkerRoleWithSBQueue\"
dnx run %1\artifacts\bin\ExportWorkerRoleWithSBQueue\
if not exist "%1\artifacts\obj\ExportWorkerRoleWithSBQueue\Debug\" mkdir "%1\artifacts\obj\ExportWorkerRoleWithSBQueue\Debug\"
dnx --configuration Debug run %1\artifacts\obj\ExportWorkerRoleWithSBQueue\Debug\ -silent
if not exist "%1\artifacts\obj\ExportWorkerRoleWithSBQueue\Release\" mkdir "%1\artifacts\obj\ExportWorkerRoleWithSBQueue\Release\"
dnx --configuration Release run %1\artifacts\obj\ExportWorkerRoleWithSBQueue\Release\ -silent 
if exist "%1\artifacts\bin\ExportWorkerRoleWithSBQueue\Debug\dnx451\ExportWorkerRoleWithSBQueue.pdb" copy "%1\artifacts\bin\ExportWorkerRoleWithSBQueue\Debug\dnx451\ExportWorkerRoleWithSBQueue.pdb" "%1\artifacts\obj\ExportWorkerRoleWithSBQueue\Debug\ExportWorkerRoleWithSBQueue.pdb"
if exist "%1\artifacts\bin\ExportWorkerRoleWithSBQueue\Release\dnx451\ExportWorkerRoleWithSBQueue.pdb" copy "%1\artifacts\bin\ExportWorkerRoleWithSBQueue\Release\dnx451\ExportWorkerRoleWithSBQueue.pdb" "%1\artifacts\obj\ExportWorkerRoleWithSBQueue\Release\ExportWorkerRoleWithSBQueue.pdb"
cd %1