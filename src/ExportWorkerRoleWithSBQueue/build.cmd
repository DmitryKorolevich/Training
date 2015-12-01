@echo off
cd "%1\src\WorkerBuilder"
if not exist "%1\packages\WorkerRole\" mkdir "%1\packages\WorkerRole\"
dnx --configuration Release run %1\packages\WorkerRole\ -silent
cd %1