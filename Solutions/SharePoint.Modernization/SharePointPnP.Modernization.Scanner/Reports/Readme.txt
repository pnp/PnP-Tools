How to correctly add data to the Excel template

1. Go to empty sheet
2. Data -> From Text/CSV -> Import your CSV file
3. Name sheet (e.g. PubPages)
4. Data -> Queries and Connections -> Delete the query that was added due to CSV import
5. Select created table -> Design -> Convert to Range to remove the created table 'name'
5. Formulas -> Name Manager -> Create 'dynamic' range like shown here: PubPagesData=OFFSET(PubPages!$A$1;0;0;COUNTA(PubPages!$A:$A);COUNTA(PubPages!$1:$1))
6. Data -> Get Data -> From Table/Range -> define range as the dynamic name (e.g PubPagesData)
7. Close Power Query Editor that was opened -> Keep
8. Delete the added sheet (as it's a duplicate)
9. Data -> Queries and Connections -> Rename table if needed + "Load to...", check "Add this data to the data model" -> OK
10. Data -> Manage Data Model -> Add relationships + furhter data configuration
11. Data -> Manage Data Model -> Insert PivotCharts and/or PivotTables
12. Hide the "original" sheet with imported data