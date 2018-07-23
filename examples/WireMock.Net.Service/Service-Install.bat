@echo off
call Service-Uninstall.bat

SET mypath=%~dp0
SET targetpath=C:\Services\WireMock.Net.Service\

mkdir "%targetpath%"
xcopy "%mypath%*" "%targetpath%*"

C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe "%targetpath%WireMock.Net.Service.exe"
net start "WireMock.Net.Service"