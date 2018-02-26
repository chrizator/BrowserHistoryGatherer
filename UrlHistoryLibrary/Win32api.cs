using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.IO;
using System.Text;

namespace UrlHistoryLibrary
{
	/// <summary>
	/// Some Win32Api Pinvoke.
	/// </summary>
	public class Win32api
	{
		/// <summary>
		/// Used by CannonializeURL method.
		/// </summary>
		[Flags]
		public enum shlwapi_URL : uint
		{
			/// <summary>
			/// Treat "/./" and "/../" in a URL string as literal characters, not as shorthand for navigation. 
			/// </summary>
			URL_DONT_SIMPLIFY = 0x08000000,
			/// <summary>
			/// Convert any occurrence of "%" to its escape sequence.
			/// </summary>
			URL_ESCAPE_PERCENT = 0x00001000,
			/// <summary>
			/// Replace only spaces with escape sequences. This flag takes precedence over URL_ESCAPE_UNSAFE, but does not apply to opaque URLs.
			/// </summary>
			URL_ESCAPE_SPACES_ONLY = 0x04000000,
			/// <summary>
			/// Replace unsafe characters with their escape sequences. Unsafe characters are those characters that may be altered during transport across the Internet, and include the (<, >, ", #, {, }, |, \, ^, ~, [, ], and ') characters. This flag applies to all URLs, including opaque URLs.
			/// </summary>
			URL_ESCAPE_UNSAFE = 0x20000000,
			/// <summary>
			/// Combine URLs with client-defined pluggable protocols, according to the World Wide Web Consortium (W3C) specification. This flag does not apply to standard protocols such as ftp, http, gopher, and so on. If this flag is set, UrlCombine does not simplify URLs, so there is no need to also set URL_DONT_SIMPLIFY.
			/// </summary>
			URL_PLUGGABLE_PROTOCOL = 0x40000000,
			/// <summary>
			/// Un-escape any escape sequences that the URLs contain, with two exceptions. The escape sequences for "?" and "#" are not un-escaped. If one of the URL_ESCAPE_XXX flags is also set, the two URLs are first un-escaped, then combined, then escaped.
			/// </summary>
			URL_UNESCAPE = 0x10000000
		}
		
		[DllImport("shlwapi.dll")]
		public static extern int UrlCanonicalize(
			string pszUrl,
			StringBuilder pszCanonicalized,
			ref int  pcchCanonicalized,
			shlwapi_URL dwFlags
			);
		
		
		/// <summary>
		/// Takes a URL string and converts it into canonical form
		/// </summary>
		/// <param name="pszUrl">URL string</param>
		/// <param name="dwFlags">shlwapi_URL Enumeration. Flags that specify how the URL is converted to canonical form.</param>
		/// <returns>The converted URL</returns>
		public static string CannonializeURL(string pszUrl, shlwapi_URL dwFlags)
		{
			StringBuilder buff = new StringBuilder(260);
			int s = buff.Capacity;
			int c = UrlCanonicalize(pszUrl , buff,ref s, dwFlags);
			if(c ==0)
				return buff.ToString();
			else
			{
				buff.Capacity = s;
				c = UrlCanonicalize(pszUrl , buff,ref s, dwFlags);
				return buff.ToString();
			}
		}


		public struct SYSTEMTIME
		{
			public Int16 Year;
			public Int16 Month;
			public Int16 DayOfWeek;
			public Int16 Day;
			public Int16 Hour;
			public Int16 Minute;
			public Int16 Second;
			public Int16 Milliseconds;
		}

		[DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
		static extern bool FileTimeToSystemTime
			(ref FILETIME FileTime, ref SYSTEMTIME SystemTime);

		
		/// <summary>
		/// Converts a file time to DateTime format.
		/// </summary>
		/// <param name="filetime">FILETIME structure</param>
		/// <returns>DateTime structure</returns>
		public static DateTime FileTimeToDateTime(FILETIME filetime)
		{
			SYSTEMTIME st = new SYSTEMTIME();
			FileTimeToSystemTime(ref filetime, ref st);
			return new DateTime(st.Year, st.Month, st.Day, st.Hour , st.Minute, st.Second, st.Milliseconds);
			
		}
		
		[DllImport("Kernel32.dll", CharSet=CharSet.Auto)]
		static extern bool SystemTimeToFileTime([In] ref SYSTEMTIME lpSystemTime, 
			out FILETIME lpFileTime);
		
		
		/// <summary>
		/// Converts a DateTime to file time format.
		/// </summary>
		/// <param name="datetime">DateTime structure</param>
		/// <returns>FILETIME structure</returns>
		public static FILETIME DateTimeToFileTime(DateTime datetime)
		{
			SYSTEMTIME st = new SYSTEMTIME();
			st.Year = (short)datetime.Year;
			st.Month = (short)datetime.Month;
			st.Day = (short)datetime.Day;
			st.Hour = (short)datetime.Hour;
			st.Minute = (short)datetime.Minute;
			st.Second = (short)datetime.Second;
			st.Milliseconds = (short)datetime.Millisecond;
			FILETIME filetime;
			SystemTimeToFileTime(ref st, out filetime);
			return filetime;
			
		}
		//compares two file times.
		[DllImport("Kernel32.dll")]
		public static extern int CompareFileTime([In] ref FILETIME lpFileTime1,[In] ref FILETIME lpFileTime2);

		
		
		
		//Retrieves information about an object in the file system.
		[DllImport("shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
		
		public const uint SHGFI_ATTR_SPECIFIED =
			0x20000;
		public const uint SHGFI_ATTRIBUTES = 0x800;
		public const uint SHGFI_PIDL = 0x8;
		public const uint SHGFI_DISPLAYNAME =
			0x200;
		public const uint SHGFI_USEFILEATTRIBUTES
			= 0x10;
		public const uint FILE_ATTRIBUTRE_NORMAL =
			0x4000;
		public const uint SHGFI_EXETYPE = 0x2000;
		public const uint SHGFI_SYSICONINDEX =
			0x4000;
		public const uint ILC_COLORDDB = 0x1;
		public const uint ILC_MASK = 0x0;
		public const uint ILD_TRANSPARENT = 0x1;
		public const uint SHGFI_ICON = 0x100;
		public const uint SHGFI_LARGEICON = 0x0;
		public const uint SHGFI_SHELLICONSIZE =
			0x4;
		public const uint SHGFI_SMALLICON = 0x1;
		public const uint SHGFI_TYPENAME = 0x400;
		public const uint SHGFI_ICONLOCATION =
			0x1000;
		


		
	}

	

	/// <summary>
	/// Contains information about a file object.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct SHFILEINFO 
	{
		public IntPtr hIcon;
		public IntPtr iIcon;
		public uint dwAttributes;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string szDisplayName;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
		public string szTypeName;
	};

	
	
	/// <summary>
	/// The helper class to sort in ascending order by FileTime(LastVisited).
	/// </summary>
	public class SortFileTimeAscendingHelper : IComparer
	{
		[DllImport("Kernel32.dll")]
		static extern int CompareFileTime([In] ref FILETIME lpFileTime1,[In] ref FILETIME lpFileTime2);
		

		int IComparer.Compare(object a, object b)
		{
			STATURL c1=(STATURL)a;
			STATURL c2=(STATURL)b;
			
			return (CompareFileTime(ref c1.ftLastVisited, ref c2.ftLastVisited));
			
		}

		public static IComparer SortFileTimeAscending()
		{      
			return (IComparer) new SortFileTimeAscendingHelper();
		} 

	} 
}
