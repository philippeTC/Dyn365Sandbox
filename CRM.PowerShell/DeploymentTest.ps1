#	TEST DEPLOYMENT CMDLETS
#	***********************

Set-Location -Path 'D:\Development\Thomas Cook\msd-online-application\TC.PowerShell'
Remove-Module TC.PowerShell.Deployment
Import-Module .\TC.PowerShell.Deployment\bin\Debug\TC.PowerShell.Deployment.dll -Force

# List all commands in module
Get-Command -module TC.PowerShell.Deployment

# Get help on command
Get-Help Get-LatestVersionDate

# Test Get-LatestVersionDate
$latestversiondate = Get-LatestVersionDate `
 -CrmConnectionString "Url=https://thomascooksit.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365" `
 -PublisherId "E8E7A6A2-09D8-E611-80F9-3863BB354FF0"

 # Debug from visual studio
 # Set Debug options:
 # - Start external program: C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe 
 # - Command line arguments: -NoLogo -NoExit -Command "Import-Module .\TC.PowerShell.Deployment.dll -Force; Get-LatestVersionDate -CrmConnectionString 'Url=https://thomascooksit.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365' -PublisherId "E8E7A6A2-09D8-E611-80F9-3863BB354FF0";

 # Test Set-ComponentSolutions
Set-ComponentSolutions `
 -CrmConnectionString "Url=https://thomascookdev.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365" `
 -LatestVersionDate $latestversiondate `
 -SourceSolutionName "ThomasCook"

 # Debug from visual studio
 # Set Debug options:
 # - Start external program: C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe 
 # - Command line arguments: 
 -NoLogo -NoExit -Command "Import-Module .\TC.PowerShell.Deployment.dll -Force; `
 $latestversiondate = Get-LatestVersionDate -CrmConnectionString 'Url=https://thomascookdev.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365' -PublisherId 'E8E7A6A2-09D8-E611-80F9-3863BB354FF0'; `
 Set-ComponentSolutions -CrmConnectionString 'Url=https://thomascookdev.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365' -LatestVersionDate $latestversiondate -SourceSolutionNames 'ThomasCook;ThomasCook_OfflineProfile';"
