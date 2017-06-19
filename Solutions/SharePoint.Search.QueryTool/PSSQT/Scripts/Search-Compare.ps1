# some ideas for scripts. Comare two different queries or compare same query against two sites
$qry1 = "marriott"
$qry2 = "frode marriott"

Search-SPindex -Site https://sp13/search -Properties title $qry1 -WarningAction SilentlyContinue | Out-File results-1.txt
Search-SPindex -Site https://sp13/search -Properties title $qry2 -WarningAction SilentlyContinue | Out-File results-2.txt

& 'C:\Program Files\Perforce\p4merge.exe' -nl $qry1 -nr $qry2 -C 'utf16' .\results-1.txt .\results-2.txt
