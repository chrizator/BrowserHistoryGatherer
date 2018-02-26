using System;

namespace BrowserHistoryGatherer
{
    [Flags]
    public enum Browser
    {
        None = 0,
        InternetExplorer = 1,
        Firefox = 2,
        Chrome = 4,
        Safari = 8,
        All = 15
    }
}