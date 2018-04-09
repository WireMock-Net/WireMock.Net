@echo off
SET mypath=%~dp0
SET targetpath=C:\Services\WireMock.Net.Service\

net stop "WireMock.Net.Service"
C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u "%mypath%WireMock.Net.Service.exe"
sc delete "WireMock.Net.Service"

rmdir /S /Q "%targetpath%"