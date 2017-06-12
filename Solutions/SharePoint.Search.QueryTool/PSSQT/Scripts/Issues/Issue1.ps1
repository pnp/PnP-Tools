# Same behavior in UI version
# These give different results
Search-SPIndex -Site .\sp13.xml "frode sivertsen"
Search-SPIndex -Site .\sp13.xml "frode sivertsen" -SelectProperties title
