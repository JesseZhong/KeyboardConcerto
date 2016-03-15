// ExecCommand.cs
// Authored by Jesse Z. Zhong
#region Usings
using System.Diagnostics;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Enumerates the possible commands a user can macro.
	/// </summary>
	public enum Command {
		NONE,
		APP_LAUNCH,
		HTTP_LAUNCH,
		HTTPS_LAUNCH
	}

	/// <summary>
	/// Provides the actual name and description of a command.
	/// </summary>
	public static class CommandMapper {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static string GetName(Command command) {
			switch (command) {
				case Command.NONE:
					return "None";
				case Command.APP_LAUNCH:
					return "Launch Application";
				default:
					return "";
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public static string GetDesc(Command command) {
			switch (command) {
				case Command.NONE:
					return "No action. Please choose an actual command.";

				case Command.APP_LAUNCH:
					return "Launches an app, program, or process of your choosing.\n" +
						   "You may include arguments/options with your execution.";
				default:
					return "";
			}
		}
	}

	/// <summary>
	/// Execute command.
	/// </summary>
	public class ExecCommand : ExecNode {

		#region Members
		private Command mCommand;
		private string mOptions;
		#endregion

		#region Properties
		/// <summary>
		/// Command that needs to be run.
		/// </summary>
		public Command Command {
			get {
				return this.mCommand;
			}
			set {
				this.mCommand = value;
			}
		}

		/// <summary>
		/// Stores the user defined options associated with this command.
		/// </summary>
		public string Options {
			get {
				return this.mOptions;
			}
			set {
				this.mOptions = value;
			}
		}
		#endregion

		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="options"></param>
		public ExecCommand(Command command, string options) {
			this.mCommand = command;
			this.mOptions = options;
		}
		#endregion

		/// <summary>
		/// Attempts to run the command.
		/// </summary>
		/// <returns>True if execution was successful.</returns>
		public override bool Execute() {

			switch (this.mCommand) {
				case Command.NONE:
					return false;

				// LAUNCHES AN APPLICATION.
				case Command.APP_LAUNCH:

					// Separate the file name from the arguments.
					// null: use white spaces as delimiters, 2: limit to 2 substrings (filename and arguments), 
					// RemoveEmptyEntries: do not include empty strings.
					string[] options = this.mOptions.Split(null as char[], 2, System.StringSplitOptions.RemoveEmptyEntries);

					// Check if there are any substrings; return false if there is no file to run.
					if (options.Length == 0)
						return false;

					// Attempt to run the process with the filename and arguments.
					// Return true if a process is initialized; false otherwise.
					return (Process.Start(options[0], ((options.Length > 1) ? options[1] : null)) != null);

				default:
					return false;
			}
		}
	}
}
