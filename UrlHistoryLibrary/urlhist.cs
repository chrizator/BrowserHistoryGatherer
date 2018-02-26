using System;
using System.Runtime.InteropServices;
using System.Collections;

namespace UrlHistoryLibrary
{
	/// <summary>
	/// Used by QueryUrl method
	/// </summary>
	public enum STATURL_QUERYFLAGS : uint
	{
		/// <summary>
		/// The specified URL is in the content cache.
		/// </summary>
		STATURL_QUERYFLAG_ISCACHED = 0x00010000,
		/// <summary>
		/// Space for the URL is not allocated when querying for STATURL.
		/// </summary>
		STATURL_QUERYFLAG_NOURL    = 0x00020000,
		/// <summary>
		/// Space for the Web page's title is not allocated when querying for STATURL.
		/// </summary>
		STATURL_QUERYFLAG_NOTITLE  = 0x00040000,
		/// <summary>
		/// //The item is a top-level item.
		/// </summary>
		STATURL_QUERYFLAG_TOPLEVEL = 0x00080000,

	}
	/// <summary>
	/// Flag on the dwFlags parameter of the STATURL structure, used by the SetFilter method.
	/// </summary>
	public enum STATURLFLAGS : uint
	{
		/// <summary>
		/// Flag on the dwFlags parameter of the STATURL structure indicating that the item is in the cache.
		/// </summary>
		STATURLFLAG_ISCACHED   = 0x00000001,
		/// <summary>
		/// Flag on the dwFlags parameter of the STATURL structure indicating that the item is a top-level item.
		/// </summary>
		STATURLFLAG_ISTOPLEVEL = 0x00000002,
	}
	/// <summary>
	/// Used bu the AddHistoryEntry method.
	/// </summary>
	public enum ADDURL_FLAG : uint
	{
		/// <summary>
		/// Write to both the visited links and the dated containers. 
		/// </summary>
		ADDURL_ADDTOHISTORYANDCACHE = 0,
		/// <summary>
		/// Write to only the visited links container.
		/// </summary>
		ADDURL_ADDTOCACHE = 1
	}


	/// <summary>
	/// The structure that contains statistics about a URL. 
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct STATURL 
	{
		/// <summary>
		/// Struct size
		/// </summary>
		public int cbSize;
        /// <summary>
        /// URL
        /// </summary>                                                                   
		[MarshalAs(UnmanagedType.LPWStr)] public string pwcsUrl;
		/// <summary>
		/// Page title
		/// </summary>
		[MarshalAs(UnmanagedType.LPWStr)] public string pwcsTitle;
		/// <summary>
		/// Last visited date (UTC)
		/// </summary>
		public FILETIME ftLastVisited;
		/// <summary>
		/// Last updated date (UTC)
		/// </summary>
		public FILETIME ftLastUpdated;
		/// <summary>
		/// The expiry date of the Web page's content (UTC)
		/// </summary>
		public FILETIME ftExpires;
		/// <summary>
		/// Flags. STATURLFLAGS Enumaration.
		/// </summary>
		public STATURLFLAGS dwFlags;								
		
		/// <summary>
		/// sets a column header in the DataGrid control. This property is not needed if you do not use it.
		/// </summary>
		public string URL
		{
			get{return pwcsUrl;}
		}
		/// <summary>
		/// sets a column header in the DataGrid control. This property is not needed if you do not use it.
		/// </summary>
		public string Title
		{
			get{
				if(pwcsUrl.StartsWith("file:"))
					return  Win32api.CannonializeURL(pwcsUrl, Win32api.shlwapi_URL.URL_UNESCAPE).Substring(8).Replace('/', '\\');
				else
					return pwcsTitle;
			}
		}
		/// <summary>
		/// sets a column header in the DataGrid control. This property is not needed if you do not use it.
		/// </summary>
		public DateTime LastVisited
		{
			get
			{
				return Win32api.FileTimeToDateTime(ftLastVisited).ToLocalTime();
			}
		}
		/// <summary>
		/// sets a column header in the DataGrid control. This property is not needed if you do not use it.
		/// </summary>
		public DateTime LastUpdated
		{
			get
			{
				return Win32api.FileTimeToDateTime(ftLastUpdated).ToLocalTime();
			}
		}
		/// <summary>
		/// sets a column header in the DataGrid control. This property is not needed if you do not use it.
		/// </summary>
		public DateTime Expires
		{
			get
			{
				try
				{
					return Win32api.FileTimeToDateTime(ftExpires).ToLocalTime();
				}
				catch(Exception)
				{
					return DateTime.Now;
				}
			}
		}

	} 

	[StructLayout(LayoutKind.Sequential)]
	public struct UUID
	{
		public int Data1;
		public short Data2;
		public short Data3;
		public byte[] Data4;
	}

	//Enumerates the cached URLs
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3C374A42-BAE4-11CF-BF7D-00AA006946EE")]
	public interface IEnumSTATURL 
	{
		void Next(int celt, ref STATURL rgelt, out int pceltFetched);	//Returns the next \"celt\" URLS from the cache
		void Skip(int celt);	//Skips the next \"celt\" URLS from the cache. doed not work.
		void Reset();	//Resets the enumeration
		void Clone(out IEnumSTATURL ppenum);	//Clones this object
		void SetFilter([MarshalAs(UnmanagedType.LPWStr)] string  poszFilter, STATURLFLAGS dwFlags);	//Sets the enumeration filter
			
	}

	
	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("3C374A41-BAE4-11CF-BF7D-00AA006946EE")]
	public interface IUrlHistoryStg
	{
		void AddUrl(string pocsUrl, string pocsTitle, ADDURL_FLAG dwFlags);	//Adds a new history entry
		void DeleteUrl(string pocsUrl, int dwFlags);	//Deletes an entry by its URL. does not work!
		void QueryUrl ([MarshalAs(UnmanagedType.LPWStr)] string pocsUrl , STATURL_QUERYFLAGS dwFlags , ref STATURL lpSTATURL );	//Returns a STATURL for a given URL
		void BindToObject ([In] string pocsUrl, [In] UUID riid, IntPtr ppvOut); //Binds to an object. does not work!
		object EnumUrls{[return: MarshalAs(UnmanagedType.IUnknown)] get;}	//Returns an enumerator for URLs
		

	}

	[ComImport]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("AFA0DC11-C313-11D0-831A-00C04FD5AE38")]
	public interface IUrlHistoryStg2 : IUrlHistoryStg
	{
		new void AddUrl(string pocsUrl, string pocsTitle, ADDURL_FLAG dwFlags);	//Adds a new history entry
		new void DeleteUrl(string pocsUrl, int dwFlags);	//Deletes an entry by its URL. does not work!
		new void QueryUrl ([MarshalAs(UnmanagedType.LPWStr)] string pocsUrl , STATURL_QUERYFLAGS dwFlags , ref STATURL lpSTATURL );	//Returns a STATURL for a given URL
		new void BindToObject ([In] string pocsUrl, [In] UUID riid, IntPtr ppvOut);	//Binds to an object. does not work!
		new object EnumUrls{[return: MarshalAs(UnmanagedType.IUnknown)] get;}	//Returns an enumerator for URLs

		void AddUrlAndNotify(string pocsUrl, string pocsTitle, int dwFlags, int fWriteHistory, object poctNotify, object punkISFolder);//does not work!
		void ClearHistory();	//Removes all history items


	}
	
	//UrlHistory class
	[ComImport]
	[Guid("3C374A40-BAE4-11CF-BF7D-00AA006946EE")]
	public class UrlHistoryClass
	{	
	}


}
