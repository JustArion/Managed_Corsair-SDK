$ErrorActionPreference = 'Stop'

function NotifyExec($str)
{
    Write-Output "Exec | $str"
    Invoke-Expression $str
}

# Set the Working Directory to the Script directory
NotifyExec("cd $PSScriptRoot")

# Install ClangSharp if not installed
NotifyExec('dotnet tool restore')

# Download the Corsair SDK Header files
NotifyExec('git submodule init')

# Update the Corsair SDK Header files (If newer)
NotifyExec('git submodule update')

# Invoke ClangSharp to generate the C$ bindings (Arguments are within the ClangSharpArgs.rsp file
$path_to_clang_args = Join-Path $PSScriptRoot 'ClangSharpArgs.rsp'
NotifyExec("ClangSharpPInvokeGenerator `"@$path_to_clang_args`"")