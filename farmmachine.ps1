# start-process powershell -argument "C:\Scripts\Backup.ps1 TestBackup"
$type=$args[0]

If ($type -eq 'build') {
    dotnet build .
    Write-Output "Begin build"
}

If ($type -eq 'start') {
    Write-Output "Begin start FarmMachine.ExchangeBroker"
    Start-Process -FilePath '.\FarmMachine.ExchangeBroker\bin\Debug\netcoreapp3.0\FarmMachine.ExchangeBroker.exe' -WorkingDirectory ".\FarmMachine.ExchangeBroker\bin\Debug\netcoreapp3.0\"

    Write-Output "Begin start FarmMachine.MonitorStrategy"
    Start-Process -FilePath ".\FarmMachine.MonitorStrategy\bin\x86\Debug\FarmMachine.MonitorStrategy.exe" -WorkingDirectory ".\FarmMachine.MonitorStrategy\bin\x86\Debug\"
}