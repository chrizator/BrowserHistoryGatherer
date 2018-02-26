using System;
using System.Collections.Generic;
using System.IO;
using BrowserHistoryGatherer.Data;
using BrowserHistoryGatherer.Utils;
using Claunia.PropertyList;

namespace BrowserHistoryGatherer.Gathering
{
    /// <summary>
    /// A gatherer to get chrome history entries
    /// </summary>
    internal sealed class SafariGatherer : BaseGatherer
    {
        #region Private Members

        private const string SAFARI_PLIST_FILE_PATH = @"Apple Computer\Safari\History.plist";
        private const string PLIST_ARRAY_KEY_NAME = "WebHistoryDates";

        private string _safariFullPlistFilePath;

        #endregion


        #region Public Properties

        public static SafariGatherer Instance { get; } = new SafariGatherer();

        #endregion


        #region Events

        #endregion



        /// <summary>
        /// Initializes a new instance of <see cref="SafariGatherer"/>
        /// </summary>
        SafariGatherer()
        {
            _safariFullPlistFilePath = GetSafariPlistFilePath();
        }



        public sealed override ICollection<HistoryEntry> GetBrowserHistory(DateTime? startTime, DateTime? endTime)
        {
            List<HistoryEntry> entryList = new List<HistoryEntry>();

            if (string.IsNullOrEmpty(_safariFullPlistFilePath))
                return entryList;

            var rootDict = (NSDictionary)PropertyListParser.Parse(_safariFullPlistFilePath);
            var entryArray = ((NSArray)rootDict.ObjectForKey(PLIST_ARRAY_KEY_NAME));
            foreach (var nsEntry in entryArray)
            {
                Uri uri;
                DateTime lastVisit;
                string title;
                int? visitCount;

                var entryAsDict = (NSDictionary)nsEntry;

                if (!DateUtils.TryParsePlistToLocal(entryAsDict.ObjectForKey("lastVisitedDate").ToString(), out lastVisit))
                    continue;
                if (!base.IsEntryInTimelimit(lastVisit, startTime, endTime))
                    continue;

                try
                {
                    uri = new Uri(entryAsDict.ObjectForKey(string.Empty).ToString());
                }
                catch (UriFormatException)
                {
                    continue;
                }

                title = entryAsDict.TryGetValue("title", out NSObject nsTitle)
                    ? nsTitle.ToString()
                    : null;

                visitCount = int.TryParse(entryAsDict.ObjectForKey("visitCount").ToString(), out int outVal)
                    ? (int?)outVal
                    : null;

                HistoryEntry entry = new HistoryEntry(uri, title, lastVisit, visitCount, Browser.Safari);
                entryList.Add(entry);
            }
            
            return entryList;
        }


        private string GetSafariPlistFilePath()
        {
            string path = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                SAFARI_PLIST_FILE_PATH);

            return File.Exists(path)
                ? path
                : null;
        }
    }
}