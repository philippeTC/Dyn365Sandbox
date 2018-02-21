Set-Location -Path 'D:\Development\Test\Dyn365 Sandbox'
Remove-Module CRM.PowerShell.Deployment
Import-Module .\CRM.PowerShell.Deployment\bin\Debug\CRM.PowerShell.Deployment.dll -Force
Get-Command -module CRM.PowerShell.Deployment
Get-Help Get-LatestVersion