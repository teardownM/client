@echo off
dotnet build /p:Configuration=Release /p:Platform="x64"
if /i "%1" equ "-r" (
    cd ..\..\..\
    start .\sledge.exe -nolauncher
    exit
)