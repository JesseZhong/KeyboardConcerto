// KeyMacro.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
#endregion

namespace KeyboardConcerto {

	[Serializable]
	public class KeyMacro {

		#region Members
		private LinkedList<ExecNode> mExecutionSequence;
		#endregion

		#region Properties
		/// <summary>
		/// List of keys, commands, and timers that can be executed.
		/// </summary>
		public LinkedList<ExecNode> ExecutionSequence {
			get {
				return this.mExecutionSequence;
			}
			set {
				this.mExecutionSequence = value;
			}
		}
		#endregion

		/// <summary>
		/// Initialize internal components.
		/// </summary>
		public KeyMacro() {
			this.mExecutionSequence = new LinkedList<ExecNode>();
		}

		/// <summary>
		/// Executes the user defined key sequence.
		/// </summary>
		/// <returns>True if sequence finished successfully.</returns>
		public bool Execute() {
			if((this.mExecutionSequence != null) || !this.mExecutionSequence.Any())
				return false;

			LinkedListNode<ExecNode> currNode = this.mExecutionSequence.First;

			while (currNode != null) {
				ExecNode execNode = currNode.Value;
				if (!execNode.Execute())
					return false;

				currNode = currNode.Next;
			}

			return true;
		}
	}
}
