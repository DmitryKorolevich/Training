@echo off
cd "%2\src\WorkerBuilder"
if not exist "%2\packages\WorkerRole\" mkdir "%2\packages\WorkerRole\"
dnx --configuration %1 run %2\packages\WorkerRole\ -silent
cd %2