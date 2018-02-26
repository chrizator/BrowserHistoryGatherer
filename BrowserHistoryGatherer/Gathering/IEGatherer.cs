using System;
using System.Collections.Generic;
using System.Diagnostics;
using BrowserHistoryGatherer.Data;
using UrlHistoryLibrary;

namespace BrowserHistoryGatherer.Gathering
{
    /// <summary>
    /// A gatherer to get ie history entries
    /// </summary>
    internal sealed class IEGatherer : BaseGatherer
    {
        #region Private Members

        #endregion


        #region Public Properties

        public static IEGatherer Instance { get; } = new IEGatherer();

        #endregion


        #region Events

        #endregion



        /// <summary>
        /// Initializes a new instance of <see cref="IEGatherer"/>
        /// </summary>
        IEGatherer()
        {

        }



        public sealed override ICollection<HistoryEntry> GetBrowserHistory(DateTime? startTime, DateTime? endTime)
        {
            List<HistoryEntry> entryList = new List<HistoryEntry>();

            var historyEnumertator = GetHistoryEnumerator();
            while (historyEnumertator.MoveNext())
            {
                Uri uri;
                DateTime lastUpdate;
                string title;

                lastUpdate = historyEnumertator.Current.LastUpdated;
                if (!base.IsEntryInTimelimit(lastUpdate, startTime, endTime))
                    continue;

                try
                {
                    uri = new Uri(historyEnumertator.Current.URL, UriKind.Absolute);
                }
                catch (UriFormatException)
                {
                    continue;
                }

                title = string.IsNullOrEmpty(historyEnumertator.Current.Title)
                    ? null
                    : historyEnumertator.Current.Title;

                var historyEntry = new HistoryEntry(uri, title, lastUpdate, null, Browser.InternetExplorer);
                entryList.Add(historyEntry);
            }

            return entryList;
        }


        private UrlHistoryWrapperClass.STATURLEnumerator GetHistoryEnumerator()
        {
            var historyWrapper = new UrlHistoryWrapperClass();
            return historyWrapper.GetEnumerator();
        }
    }
}