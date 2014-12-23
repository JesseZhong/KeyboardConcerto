// ExecNode.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Execution node.
	/// </summary>
	public abstract class ExecNode {

		/// <summary>
		/// Executes user defined behavior.
		/// </summary>
		/// <returns>True if execution was successful.</returns>
		public abstract bool Execute();
	}
}
