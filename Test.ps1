# 2024 / github.com/JustArion
# CorsairSDK Test Runner

$ErrorActionPreference = 'Stop'
$workingDirectory = Get-Location
function WriteInfo($str)
{
    Write-Output "[*] $str"
}
function NotifyExec($str)
{
    WriteInfo("Exec | $str")
    Invoke-Expression $str
}


function ResetWorkingDirectory
{
    cd $workingDirectory
}

try
{
    # Set the Working Directory to the Script directory
    NotifyExec("cd $PSScriptRoot")
    NotifyExec("dotnet clean ./src/")

    WriteInfo("Running .NET 6 Windows Tests")
    NotifyExec("dotnet test -f net6.0-windows .\src\")
    NotifyExec("dotnet clean ./src/")    
}
finally
{
    ResetWorkingDirectory
}