using BrowserHistoryGatherer.Data;
using BrowserHistoryGatherer.Gathering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BrowserHistoryGatherer
{
    /// <summary>
    /// Class to gather browser history informations
    /// </summary>
    public static class BrowserHistory
    {
        #region Private Members

        #endregion


        #region Public Properties

        #endregion


        #region Events

        #endregion



        public static IList<HistoryEntry> GetHistory(Browser browser, DateTime? startTime, DateTime? endTime)
        {
            List<HistoryEntry> historyEntries = new List<HistoryEntry>();

            if ((browser & Browser.Chrome) == Browser.Chrome)
                historyEntries.AddRange(ChromeGatherer.Instance.GetBrowserHistory(startTime, endTime));

            if ((browser & Browser.Firefox) == Browser.Firefox)
                historyEntries.AddRange(FirefoxGatherer.Instance.GetBrowserHistory(startTime, endTime));

            if ((browser & Browser.Safari) == Browser.Safari)
                historyEntries.AddRange(SafariGatherer.Instance.GetBrowserHistory(startTime, endTime));

            if ((browser & Browser.InternetExplorer) == Browser.InternetExplorer)
                historyEntries.AddRange(IEGatherer.Instance.GetBrowserHistory(startTime, endTime));

            return historyEntries;
        }

        public static ICollection<HistoryEntry> GetHistory(Browser browser, DateTime? startTime)
            => GetHistory(browser, startTime, null);

        public static ICollection<HistoryEntry> GetHistory(Browser browser)
            => GetHistory(browser, null);
    }
}