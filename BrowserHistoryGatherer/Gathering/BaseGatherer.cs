using BrowserHistoryGatherer.Data;
using System;
using System.Collections.Generic;

namespace BrowserHistoryGatherer.Gathering
{
    /// <summary>
    /// The base class for a browser gatherer
    /// </summary>
    internal abstract class BaseGatherer
    {
        #region Private Members

        #endregion


        #region Properties

        #endregion


        #region Events

        #endregion



        public abstract ICollection<HistoryEntry> GetBrowserHistory(DateTime? startTime, DateTime? endTime);


        protected bool IsEntryInTimelimit(DateTime entryVisit, DateTime? startTime, DateTime? endTime)
        {
            if (startTime == null && endTime == null)
                return true;

            bool isLaterThanStart = DateTime.Compare(entryVisit, (DateTime)startTime) >= 0;

            if (endTime == null)
                return isLaterThanStart;

            bool isEarlierThanEnd = DateTime.Compare(entryVisit, (DateTime)endTime) <= 0;

            if (isLaterThanStart && isEarlierThanEnd)
                return true;

            return false;
        }
    }
}