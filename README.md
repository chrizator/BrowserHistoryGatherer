# BrowserHistoryGatherer - Browser history retrieval library
BrowserHistoryGatherer is a library that gathers history entries of major browsers.:
- Chrome
- Firefox
- Internet Explorer
- Safari

## Usage
**Get history of all browsers without time limitation:**
```cs
BrowserHistory.GetHistory(Browser.All);
```
**Get today's browser history of Firefox:**
```cs
BrowserHistory.GetHistory(Browser.Firefox, DateTime.Today);
```
**Get history of all browsers for the last 10 minutes:**
```cs
BrowserHistory.GetHistory(Browser.All, DateTime.AddMinutes(-10));
```
**Get browser history of Google Chrome and Firefox for a certain time span:**
```cs
BrowserHistory.GetHistory(Browser.Chrome | Browser.Firefox, DateTime.Parse("4/2/2018"), DateTime.Parse("4/4/2018"));
```
**Print all visited entries by all browsers in the console:**
```cs
var history = BrowserHistory.GetHistory(Browser.All, DateTime.Parse("4/2/2018"), DateTime.Parse("4/4/2018"));
foreach (var entry in history)
{
    Console.WriteLine(entry.Uri);
    Console.WriteLine(entry.Title);
    Console.WriteLine(entry.LastVisitTime);
    Console.WriteLine(entry.FriendlyVisitCount);
}
```

## Installation
- UrlHistoryLibrary DLL (wrapper)
	- https://www.codeproject.com/Articles/7500/The-Tiny-Wrapper-Class-for-URL-History-Interface-i

**Required NuGet packages:**
- plist-cil
- System.Data.SQLite.Core
