# Compatibility with PS major versions <= 2
if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $Script:MyInvocation.MyCommand.Path
}

# Add PSSQT modules directory to the autoload path.
$PSSQTModulePath = Join-path $PSScriptRoot "psmodules/"

if( -not $env:PSModulePath.Contains($PSSQTModulePath) ){
    $env:PSModulePath = $env:PSModulePath.Insert(0, "$PSSQTModulePath;")
}
