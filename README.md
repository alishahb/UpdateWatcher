[![Build status](https://ci.appveyor.com/api/projects/status/3rgccemaeafr2jv8?svg=true)](https://ci.appveyor.com/project/alishahb/updatewatcher)

![Icon](https://snag.gy/9EupgT.jpg) 
# UpdateWatcher
Utility to watch for new version of software by provided URL and Auto download & Extract, rename / Copy needed files.

Be always in touch! ლ(ಠ益ಠლ) .

##Goal:
* **Download** zip packages from Url
* **Extract archive** to `Extract folder` specified by Settings
* **Copy files:** specified by Settings from `Extract folder`to other locations
* **Rename files**: in `Extract folder` with filtering by name and extension, support option to enable recursive check in subfolders
* **Delete Existing**: option to delete existing folder and all contents, if needed (i.e. to perform **Clean install**)
* **Manage builds**: Store all info about downloaded builds, ability to mark them as ignored if needed
* **Revert builds**: If current build marked as ignored, it will be reverted to previous one, if available
* **Mods:** 
  * _Manual_: one time check by buttpn press
  * _Daemon_: run check each time when specified delay timeout passed

All Settings stored in `Settings.json` file.

_In Example folder you can finde settings file for Honorbuddy._

![GUI Screenshot - Settings](https://snag.gy/oIhLnd.jpg)
![GUI Screenshot - Downloading](https://snag.gy/AIFkOP.jpg)
![GUI Screenshot - Daemon](https://snag.gy/AcR0vB.jpg)
![GUI Screenshot - Builds](https://snag.gy/ish87Q.jpg)


#Download compiled
* **Last development build** from Appveyor: https://ci.appveyor.com/project/alishahb/updatewatcher/build/artifacts
* **Last release build** from Github: https://github.com/alishahb/UpdateWatcher/releases/

#Require .Net 4.6.1
Download: https://www.microsoft.com/en-us/download/details.aspx?id=49981

#Nuget Dependencies
* Restoring NuGet package NLog.4.3.8.
* Restoring NuGet package Newtonsoft.Json.9.0.1.
* Restoring NuGet package OpenDialog.1.1.1.
* Restoring NuGet package SevenZipSharp.0.64.0.
* Restoring NuGet package SevenZipSharp.Net45.1.0.4

#Legal
**UpdateWatcher Licence:** Apache License version 2.0

* _eye icon:_ by by Trinh Ho in medical via Flaticon Base License http://file000.flaticon.com/downloads/license/license.pdf
  * http://www.flaticon.com/free-icon/eye_217458
  * http://www.flaticon.com/free-icon/eye_217463
