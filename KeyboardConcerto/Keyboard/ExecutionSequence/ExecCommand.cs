// ExecCommand.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.IO;
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
					return "Launch an Application";

				case Command.HTTP_LAUNCH:
					return "Open a Web Page";

				case Command.HTTPS_LAUNCH:
					return "Securely Open a Web Page";

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
					return "Launches an app, program, protocol, or process of your choosing.\n" +
						   "You may include arguments/options with your execution.";

				case Command.HTTP_LAUNCH:
					return "Launches a web page in your default browser.";

				case Command.HTTPS_LAUNCH:
					return "Launches a web page securely in your default browser.";

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
		private Command mCommand = Command.NONE;
		private string mTarget = "";
		private string mOptions = "";
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the command that needs to be run.
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
		/// Gets or sets the target of this command.
		/// </summary>
		public string Target {
			get {
				return this.mTarget;
			}
			set {
				this.mTarget = value;
			}
		}

		/// <summary>
		/// Gets or sets the user-defined options for this command.
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
		/// Initializes an automated command given the type, target, and options.
		/// </summary>
		/// <param name="command">The type of command that is being used.</param>
		/// <param name="target">The target details of the command.</param>
		/// <param name="options">Any optional options that are specified for the command.</param>
		public ExecCommand(Command command, string target = "", string options = "") {
			this.mCommand = command;
			this.mTarget = target;
			this.mOptions = options;
		}
		#endregion

		/// <summary>
		/// Attempts to run the command.
		/// </summary>
		/// <returns>True if execution was successful.</returns>
		public override bool Execute() {

			switch (this.mCommand) {

				// UNSPECIFIED COMMAND; DOES NOTHING
				case Command.NONE:
					return false;

				// LAUNCHES AN APPLICATION.
				case Command.APP_LAUNCH:

					// Checks if an application path was specified and that it exists.
					if (!File.Exists(this.mTarget))
						return false;

					// Attempt to run the process with the filename and arguments.
					// Return true if a process is initialized; false otherwise.
					return (Process.Start(this.mTarget, this.mOptions) != null);

				// LAUNCHES A WEBSITE
				case Command.HTTP_LAUNCH:

					// Checks if a URL was specified.
					if (this.mTarget == "")
						return false;

					// Attempt to load a web page.
					// Return true if the page is launched; false otherwise.
					return (Process.Start(String.Format("http:{0}", this.mTarget), this.mOptions) != null);

				// SECURELY LAUNCHES A WEBSITE
				case Command.HTTPS_LAUNCH:

					// Checks if a URL was specified.
					if (this.mTarget == "")
						return false;

					// Attempt to load a web page securely.
					// Return true if the page is launched; false otherwise.
					return (Process.Start(String.Format("https:{0}", this.mTarget), this.mOptions) != null);

				default:
					return false;
			}
		}
	}
}
