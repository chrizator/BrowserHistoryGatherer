using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BrowserHistoryGatherer.Data;
using BrowserHistoryGatherer.Utils;

namespace BrowserHistoryGatherer.Gathering
{
    /// <summary>
    /// A gatherer to get firefox history entries
    /// </summary>
    internal sealed class FirefoxGatherer : BaseGatherer

    {
        #region Private Members

        private const string FIREFOX_DATA_PATH = @"Mozilla\Firefox\Profiles\";
        private const string DATABASE_NAME = "places.sqlite";
        private const string TABLE_NAME = "moz_places";
        private const string VISITS_TABLE_NAME = "moz_historyvisits";

        private IEnumerable<string> _firefoxDatabasePaths;

        #endregion


        #region Public Properties

        public static FirefoxGatherer Instance { get; } = new FirefoxGatherer();

        #endregion


        #region Events

        #endregion



        /// <summary>
        /// Initializes a new instance of <see cref="FirefoxGatherer"/>
        /// </summary>
        FirefoxGatherer()
        {
            this._firefoxDatabasePaths = GetFirefoxDatabasePaths();
        }



        public sealed override ICollection<HistoryEntry> GetBrowserHistory(DateTime? startTime, DateTime? endTime)
        {
            List<HistoryEntry> entryList = new List<HistoryEntry>();

            string query = string.Format("SELECT url, title, visit_count, datetime(visit_date/1000000,'unixepoch') AS last_visit " +
                                         "FROM {0}, {1} " +
                                         "WHERE {0}.id = {1}.place_id", TABLE_NAME, VISITS_TABLE_NAME);
            
            foreach (string dbPath in _firefoxDatabasePaths)
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
                    catch(UriFormatException)
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

                    HistoryEntry entry = new HistoryEntry(uri, title, lastVisit, visitCount, Browser.Firefox);
                    entryList.Add(entry);
                }
            }

            return entryList;
        }



        private IEnumerable<string> GetFirefoxDatabasePaths()
        {
            ICollection<string> databasePaths = new List<string>();

            string dataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                FIREFOX_DATA_PATH);

            if (Directory.Exists(dataFolder))
            {
                foreach (string profileFolder in Directory.EnumerateDirectories(dataFolder))
                {
                    string dbPath = Path.Combine(dataFolder, profileFolder, DATABASE_NAME);
                    if (File.Exists(dbPath))
                        databasePaths.Add(dbPath);
                }
            }

            return databasePaths;
        }
    }
}