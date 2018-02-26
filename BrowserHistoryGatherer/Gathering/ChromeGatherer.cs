using BrowserHistoryGatherer.Data;
using BrowserHistoryGatherer.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace BrowserHistoryGatherer.Gathering
{
    /// <summary>
    /// A gatherer to get chrome history entries
    /// </summary>
    internal sealed class ChromeGatherer : BaseGatherer
    {
        #region Private Members

        private const string CHROME_DATA_PATH = @"Google\Chrome\User Data";
        private const string DEFAULT_PROFILE_NAME = "Default";
        private const string CUSTOM_PROFILE_PATTERN = "Profile ?*";
        private const string DATABASE_NAME = "History";
        private const string TABLE_NAME = "urls";

        private string _fullDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            CHROME_DATA_PATH);

        private IEnumerable<string> _chromeDatabasePaths;

        #endregion


        #region Public Properties

        public static ChromeGatherer Instance { get; } = new ChromeGatherer();

        #endregion


        #region Events

        #endregion



        ChromeGatherer()
        {
            this._chromeDatabasePaths = GetChromeDatabasePaths();
        }



        public sealed override ICollection<HistoryEntry> GetBrowserHistory(DateTime? startTime, DateTime? endTime)
        {
            List<HistoryEntry> entryList = new List<HistoryEntry>();

            string query = string.Format("SELECT url, title, visit_count, datetime(last_visit_time/1000000-11644473600, 'unixepoch') AS last_visit " +
                                         "FROM {0}", TABLE_NAME);

            foreach (string dbPath in _chromeDatabasePaths)
            {
                DataTable historyDt = SqlUtils.QueryDataTable(dbPath, query);

                foreach (DataRow row in historyDt.Rows)
                {
                    Uri uri;
                    DateTime lastVisit;
                    string title;
                    int? visitCount;

                    lastVisit = DateTime.Parse(row["last_visit"].ToString()).ToLocalTime();
                    if (!base.IsEntryInTimelimit(lastVisit, startTime, endTime))
                        continue;

                    try
                    {
                        uri = new Uri(row["url"].ToString(), UriKind.Absolute);
                    }
                    catch (UriFormatException)
                    {
                        continue;
                    }

                    title = row["title"].ToString();
                    title = string.IsNullOrEmpty(title)
                        ? null
                        : title;

                    visitCount = int.TryParse(row["visit_count"].ToString(), out int outVal)
                        ? (int?)outVal
                        : null;

                    HistoryEntry entry = new HistoryEntry(uri, title, lastVisit, visitCount, Browser.Chrome);
                    entryList.Add(entry);
                }
            }

            return entryList;
        }


        private IEnumerable<string> GetChromeDatabasePaths()
        {
            ICollection<string> databasePaths = new List<string>();
            string path = null;

            if (TryGetFullPathByProfile(DEFAULT_PROFILE_NAME, out path))
                databasePaths.Add(path);

            foreach (string userPath in Directory.EnumerateDirectories(_fullDataPath, CUSTOM_PROFILE_PATTERN))
            {
                if (TryGetFullPathByProfile(new DirectoryInfo(userPath).Name, out path))
                    databasePaths.Add(path);
            }

            return databasePaths;
        }


        private bool TryGetFullPathByProfile(string profileName, out string fullPath)
        {
            string dbPath = Path.Combine(
                _fullDataPath,
                profileName,
                DATABASE_NAME);

            fullPath = File.Exists(dbPath)
                ? dbPath
                : null;

            return fullPath == null
                ? false 
                : true;
        }
    }
}