#region Usings
using System;
using System.Text;
using System.Runtime.InteropServices;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Provides information to the <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">
	/// IQueryAssociations</a> interface methods.
	/// Description summaries taken from: https://msdn.microsoft.com/en-us/library/windows/desktop/bb762471(v=vs.85).aspx
	/// </summary>
	public enum AssocF {

		/// <summary>None of the following options are set.</summary>
		NONE = 0x00000000,

		/// <summary>
		/// Instructs <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">
		/// IQueryAssociations</a> interface methods not to map CLSID values to ProgID values.
		/// </summary>
		INIT_NO_REMAP_CLSID = 0x00000001,

		/// <summary>
		/// Identifies the value of the pwszAssoc parameter of <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761398(v=vs.85).aspx">
		/// IQueryAssociations::Init</a> an executable file name. If this flag is not set, the root key will be set to the ProgID associated with the 
		/// <b>.exe</b> key instead of the executable file's ProgID.
		/// </summary>
		INIT_BY_EXE_NAME = 0x00000002,

		/// <summary>Identical to <see cref="INIT_BY_EXE_NAME"/>.</summary>
		OPEN_BY_EXE_NAME = 0x00000002,

		/// <summary>
		/// Specifies that when an <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">IQueryAssociations</a>
		/// method does not find the requested value under the root key, it should attempt to retrieve the comparable value from the <b>*</b> subkey.
		/// </summary>
		INIT_DEFAULT_TO_STAR = 0x00000004,

		/// <summary>
		/// Specifies that when a <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">IQueryAssociations</a> 
		/// method does not find the requested value under the root key, it should attempt to retrieve the comparable value from the Folder <b>subkey</b>.
		/// </summary>
		INIT_DEFAULT_TO_FOLDER = 0x00000008,

		/// <summary>
		/// Specifies that only <b>HKEY_CLASSES_ROOT</b> should be searched, and that <b>HKEY_CURRENT_USER</b> should be ignored.
		/// </summary>
		NO_USER_SETTINGS = 0x00000010,

		/// <summary>
		/// Specifies that the return string should not be truncated. Instead, 
		/// return an error value and the required size for the complete string.
		/// </summary>
		NO_TRUNCATE = 0x00000020,

		/// <summary>
		/// Instructs <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">IQueryAssociations</a> 
		/// methods to verify that data is accurate. This setting allows <b>IQueryAssociations</b> methods to read data from the user's 
		/// hard disk for verification. For example, they can check the friendly name in the registry against the one stored in the .exe 
		/// file. Setting this flag typically reduces the efficiency of the method.
		/// </summary>
		VERIFY = 0x00000040,

		/// <summary>
		/// Instructs <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">IQueryAssociations</a> 
		/// methods to ignore Rundll.exe and return information about its target. Typically <b>IQueryAssociations</b> methods return 
		/// information about the first .exe or .dll in a command string. If a command uses Rundll.exe, setting this flag tells the 
		/// method to ignore Rundll.exe and return information about its target.
		/// </summary>
		REMAP_RUNDLL = 0x00000080,

		/// <summary>
		/// Instructs <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761400(v=vs.85).aspx">IQueryAssociations</a> 
		/// methods not to fix errors in the registry, such as the friendly name of a function not matching the one found in the .exe file.
		/// </summary>
		NO_FIXUPS = 0x00000100,

		/// <summary>Specifies that the BaseClass value should be ignored.</summary>
		IGNORE_BASE_CLASS = 0x00000200,

		/// <summary>
		/// <b>Introduced in Windows 7.</b> Specifies that the "Unknown" ProgID should be ignored; instead, fail.
		/// </summary>
		INIT_IGNORE_UNKNOWN = 0x00000400,

		/// <summary>
		/// <b>Introduced in Windows 8.</b> Specifies that the supplied ProgID should 
		/// be mapped using the system defaults, rather than the current user defaults.
		/// </summary>
		INIT_FIXED_PROGID = 0x00000800,

		/// <summary>
		/// <b>Introduced in Windows 8.</b> Specifies that the value is a protocol, and should be mapped using the current user defaults.
		/// </summary>
		IS_PROTOCOL = 0x00001000,

		/// <summary>
		/// <b>Introduced in Windows 8.1.</b> Specifies that the ProgID corresponds with a 
		/// file extension based association. Use together with <b>INIT_FIXED_PROGID</b>.
		/// </summary>
		INIT_FOR_FILE = 0x00002000
	}

	/// <summary>
	/// Defines the type of string that is returned for the 
	/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761396(v=vs.85).aspx">IQueryAssociations::GetString()</a> method.
	/// Description summaries taken from: https://msdn.microsoft.com/en-us/library/windows/desktop/bb762475(v=vs.85).aspx
	/// </summary>
	public enum AssocStr {

		/// <summary>A command string associated with a Shell verb.</summary>
		COMMAND = 1,

		/// <summary>
		/// An executable from a Shell verb command string. For example,  
		/// this string is found as the (Default) value for a subkey such as 
		/// HKEY_CLASSES_ROOT\ApplicationName\shell\Open\command. If the command 
		/// uses Rundll.exe, set the ASSOCF_REMAPRUNDLL flag in the flags parameter 
		/// of IQueryAssociations::GetString to retrieve the target executable.
		/// </summary>
		EXECUTABLE,

		/// <summary>The friendly name of a document type.</summary>
		FRIENDLY_DOC_NAME,

		/// <summary>The friendly name of an executable file.</summary>
		FRIENDLY_APP_NAME,

		/// <summary>Ignore the information associated with the <b>open</b> subkey.</summary>
		NO_OPEN,

		/// <summary>Look under the <b>ShellNew</b> subkey.</summary>
		SHELL_NEW_VALUE,

		/// <summary>A template for DDE commands.</summary>
		DDE_COMMAND,

		/// <summary>The DDE command to use to create a process.</summary>
		DDE_IF_EXEC,

		/// <summary>The application name in a DDE broadcast.</summary>
		DDE_APPLICATION,

		/// <summary>The topic name in a DDE broadcast.</summary>
		DDE_TOPIC,

		/// <summary>
		/// Corresponds to the InfoTip registry value. Returns an info tip for an item, or list of properties in the form of an 
		/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761511(v=vs.85).aspx">IPropertyDescriptionList</a> 
		/// from which to create an info tip, such as when hovering the cursor over a file name. The list of properties can be parsed with 
		/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb762079(v=vs.85).aspx">PSGetPropertyDescriptionListFromString</a>.
		/// </summary>
		INFO_TIP,

		/// <summary>
		/// <b>Introduced in Internet Explorer 6.</b> Corresponds to the QuickTip registry value. Same as INFO_TIP, except that it always returns a 
		/// list of property names in the form of an <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761511(v=vs.85).aspx">
		/// IPropertyDescriptionList</a>. The difference between this value and ASSOCSTR_INFOTIP is that this returns properties that are safe 
		/// for any scenario that causes slow property retrieval, such as offline or slow networks. Some of the properties returned from INFO_TIP 
		/// might not be appropriate for slow property retrieval scenarios. The list of properties can be parsed with 
		/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb762079(v=vs.85).aspx">PSGetPropertyDescriptionListFromString</a>.
		/// </summary>
		QUICK_TIP,

		/// <summary>
		/// <b>Introduced in Internet Explorer 6.</b> Corresponds to the TileInfo registry value. Contains a list of properties to be displayed for a 
		/// particular file type in a Windows Explorer window that is in tile view. This is the same as INFO_TIP, but, QUICK_TIP, it also returns 
		/// a list of property names in the form of an <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761511(v=vs.85).aspx">
		/// IPropertyDescriptionList</a>. The list of properties can be parsed with 
		/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb762079(v=vs.85).aspx">PSGetPropertyDescriptionListFromString</a>.
		/// </summary>
		TILE_INFO,

		/// <summary>
		/// <b>Introduced in Internet Explorer 6.</b> Describes a general type of MIME file association, such as 
		/// image and bmp, so that applications can make general assumptions about a specific file type.
		/// </summary>
		CONTENT_TYPE,

		/// <summary>
		/// <b>Introduced in Internet Explorer 6.</b> Returns the path to the icon resources to use by default for this association. 
		/// Positive numbers indicate an index into the dll's resource table, while negative numbers indicate a resource ID. 
		/// An example of the syntax for the resource is "c:\myfolder\myfile.dll,-1".
		/// </summary>
		DEFAULT_ICON,

		/// <summary>
		/// <b>Introduced in Internet Explorer 6.</b> For an object that has a Shell extension associated with it, you can use this to retrieve the CLSID 
		/// of that Shell extension object by passing a string representation of the IID of the interface you want to retrieve as the pwszExtra parameter of 
		/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761396(v=vs.85).aspx">IQueryAssociations::GetString</a>. For example, if you 
		/// want to retrieve a handler that implements the <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761848(v=vs.85).aspx">
		/// IExtractImage</a> interface, you would specify "{BB2E617C-0920-11d1-9A0B-00C04FC2D6C1}", which is the IID of <b>IExtractImage</b>.
		/// </summary>
		SHELL_EXTENSION,

		/// <summary>
		/// <b>Introduced in Internet Explorer 8.</b> For a verb invoked through COM and the <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms679679(v=vs.85).aspx">
		/// IDropTarget</a> interface, you can use this flag to retrieve the <b>IDropTarget</b> object's CLSID. This CLSID is registered in the DropTarget subkey. 
		/// The verb is specified in the pwszExtra parameter in the call to <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761396(v=vs.85).aspx">
		/// IQueryAssociations::GetString</a>.
		/// </summary>
		DROP_TARGET,

		/// <summary>
		/// <b>Introduced in Internet Explorer 8.</b> For a verb invoked through COM and the <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/dd378382(v=vs.85).aspx">
		/// IExecuteCommand</a> interface, you can use this flag to retrieve the <b>IExecuteCommand</b> object's CLSID. This CLSID is registered in the verb's <b>command</b> 
		/// subkey as the DelegateExecute entry. The verb is specified in the pwszExtra parameter in the call to 
		/// <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/bb761396(v=vs.85).aspx">IQueryAssociations::GetString</a>.
		/// </summary>
		DELEGATE_EXECUTE,

		/// <summary><b>Introduced in Windows 8.</b></summary>
		SUPPORTED_URI_PROTOCOLS,

		/// <summary>
		/// The ProgID provided by the app associated with the file type or URI scheme. 
		/// This if configured by users in their default program settings.
		/// </summary>
		PROG_ID,

		/// <summary>
		/// The AppUserModelID of the app associated with the file type or URI scheme. 
		/// This is configured by users in their default program settings.
		/// </summary>
		APP_ID,

		/// <summary>
		/// The publisher of the app associated with the file type or URI scheme. 
		/// This is configured by users in their default program settings.
		/// </summary>
		APP_PUBLISHER,

		/// <summary>
		/// The icon reference of the app associated with the file type or URI scheme. 
		/// This is configured by users in their default program settings.
		/// </summary>
		APP_ICONREFERENCE,

		/// <summary>The maximum defined ASSOCSTR value, used for validation purposes.</summary>
		MAX
	}

	public static class Assoc {

		/// <summary>
		/// 
		/// </summary>
		/// <param name="association"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public static string AssocQueryString(AssocStr association, string extension) {
			const int S_OK = 0;
			const int S_FALSE = 1;

			uint length = 0;
			uint ret = AssocQueryString(AssocF.NONE, association, extension, null, null, ref length);
			if (ret != S_FALSE) {
				throw new InvalidOperationException("Could not determine associated string");
			}

			var sb = new StringBuilder((int)length); // (length-1) will probably work too as the marshaller adds null termination
			ret = AssocQueryString(AssocF.NONE, association, extension, null, sb, ref length);
			if (ret != S_OK) {
				throw new InvalidOperationException("Could not determine associated string");
			}

			return sb.ToString();
		}

		[DllImport("Shlwapi.dll", CharSet = CharSet.Unicode)]
		public static extern uint AssocQueryString(AssocF flags, AssocStr str,
		   string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, ref uint
		   pcchOut);
	}
}
