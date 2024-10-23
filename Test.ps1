# 2024 / github.com/JustArion
# CorsairSDK Test Runner

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
    Set-Location $workingDirectory
}

try
{
    # Set the Working Directory to the Script directory
    NotifyExec("cd $PSScriptRoot")
    NotifyExec("dotnet clean ./src/ --verbosity minimal")

    WriteInfo("Running .NET 6 Windows Tests")
    NotifyExec("dotnet test -f net6.0-windows .\src\")
    # NotifyExec("dotnet clean ./src/ --verbosity minimal")    
}
finally
{
    ResetWorkingDirectory
}