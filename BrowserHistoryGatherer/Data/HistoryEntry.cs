using System;

namespace BrowserHistoryGatherer.Data
{
    public class HistoryEntry
    {
        #region Private Members

        #endregion


        #region Properties

        public Uri Uri { get; }
        public string Title { get; }
        public DateTime LastVisitTime { get; }
        public int? VisitCount { get; }
        public Browser Browser { get; }

        public string SafeVisitCount => VisitCount == null 
            ? "N/A" 
            : VisitCount.ToString();

        #endregion


        #region Events

        #endregion



        /// <summary>
        /// Initializes a new instance of <see cref="HistoryEntry"/>
        /// </summary>
        public HistoryEntry(Uri uri, string title, DateTime visitTime, int? visitCount, Browser browser)
        {
            this.Uri = uri;
            this.Title = title;
            this.LastVisitTime = visitTime;
            this.VisitCount = visitCount;
            this.Browser = browser;
        }
    }
}