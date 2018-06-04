# PnP Changelog
*Please do not commit changes to this file, it is maintained by the repo owner.*

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/).

## [June 2018 - Unreleased]

### Added

### Changed

- SharePoint.UIExperience.Scanner v1.6: Add Excel based "List and library readiness for modern UI" report generation
- SharePoint.Modernization.Scanner v1.5: Add Excel based "Group connection readiness" and "Page transformation readiness" report generation
- SharePoint.Modernization.Scanner v1.4: page webpart mapping percentage and unmapped web parts column + additional commandline options to control the export of web part properties and use of search for site/page usage information

## [May 2018]

### Added

- First preview version of page migration tech that can be used to transform wiki and webpart pages into modern client side pages

### Changed

- Search query tool v2.8.1: Updated PSSQT version [trevis62]

## [April 2018]

### Added

### Changed

- Search query tool v2.8: Removed old SPO login as it fails too often. Fixed ADAL login for viewing all properties
- SharePoint.Modernization.Scanner v1.3: Site usage information being included
- SharePoint.Modernization.Scanner v1.2: Reliability improvements
- SharePoint.PermissiveFile.Scanner v1.6: Reliability improvements
- SharePoint.Visio.Scanner v1.1: Also scan SiteAssets library for web part pages + reliability improvements
- SharePoint Scanning Framework: Using March PnP Sites Core version + improved reliability/output writing in sample scanner


### Deprecated

## [March 2018]

### Added

- SharePoint.Visio.Scanner v1.0: Scanner to help support upcoming Visio Web Access deprecation (https://blogs.msdn.microsoft.com/visio/2017/09/25/migrate-from-visio-web-access-to-visio-online)
- Script to rename html/html files in a site to aspx (RemediateSiteCollection.ps1, see SharePoint.PermissiveFile.Scanner) 

### Changed

- SharePoint.UIExperience.Scanner v1.5: Reliability improvements + always log list basetemplate value
- SharePoint.PermissiveFile.Scanner v1.5: Improved output writing approach to increase performance
- SharePoint.PermissiveFile.Scanner v1.4: Allow site scoping via -r or -v parameter

### Deprecated
