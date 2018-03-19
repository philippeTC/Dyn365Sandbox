#	TEST MAINTENANCE CMDLETS
#	***********************

Set-Location -Path 'D:\Development\Thomas Cook\msd-online-application\TC.PowerShell'
Remove-Module TC.PowerShell.Maintenance
Import-Module .\TC.PowerShell.Maintenance\bin\Debug\TC.PowerShell.Maintenance.dll -Force

# List all commands in module
Get-Command -module TC.PowerShell.Maintenance

# Get help on command
Get-Help Set-BusinessRuleOrder

# Test
# Source: DEV 
# Target: SIT
Set-BusinessRuleOrder `
 -CrmSourceConnectionString "Url=https://thomascookdev.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365" `
 -CrmTargetConnectionString "Url=https://thomascookstaging.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365" `
 -EntityName "tc_compensation"

 # Debug from visual studio
 # Set Debug options:
 # - Start external program: C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe 
 # - Command line arguments: -NoLogo -NoExit -Command "Import-Module .\TC.PowerShell.Maintenance.dll -Force; Set-BusinessRuleOrder -CrmSourceConnectionString 'Url=https://thomascookdev.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365' -CrmTargetConnectionString 'Url=https://thomascooksit.crm4.dynamics.com;Username=tcgdynamicscrmnoprd@thomascook.onmicrosoft.com;Password=9L%$!AIOps$$;authtype=Office365' -EntityName 'tc_compensation'";