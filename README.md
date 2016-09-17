[![Build status](https://ci.appveyor.com/api/projects/status/3rgccemaeafr2jv8?svg=true)](https://ci.appveyor.com/project/alishahb/updatewatcher)

# UpdateWatcher
Utility to watch for new version of software by provided URL

##Goal:
* **Download** zip packages from Url
* **Extract archive** to `Extract folder` specified by Settings
* **Copy files:** specified by Settings from `Extract folder`to other locations
* **Rename files**: in `Extract folder` with filtering by name and extension, support option to enable recursive check in subfolders
* **Delete Existing**: option to delete existing folder and all contents, if needed (i.e. to perform **Clean install**)
* Mods: 
  * _Manual_: one time check by buttpn press
  * _Daemon_: run check each time when specified delay timeout passed




Download last succcess build: https://ci.appveyor.com/project/alishahb/updatewatcher/build/artifacts

#Require .Net 4.6.1
Download: https://www.microsoft.com/en-us/download/details.aspx?id=49981

#Nuget Dependencies
* Restoring NuGet package NLog.4.3.8.
* Restoring NuGet package Newtonsoft.Json.9.0.1.
* Restoring NuGet package OpenDialog.1.1.1.
* Restoring NuGet package SevenZipSharp.0.64.0.
* Restoring NuGet package SevenZipSharp.Net45.1.0.4
