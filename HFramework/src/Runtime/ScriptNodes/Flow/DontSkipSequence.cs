using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Flow
{
	/// <summary>
	/// Node to mark part of a Skippable Section as a MUST-run. It must be a direct children of SkippableSection node.
	///
	/// This Node will run all of its children in sequence (just like a Sequence node), and:
	/// 1. will fail if any of them fail.
	/// 2. will succeed only if all children succeed.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Flow/Dont Skip Sequence")]
	[MovedFrom(true, "HFramework.ScriptNodes", null, "DontSkipSequence")]
	public class DontSkipSequence : Sequence
	{
		/**
		 * This node is exactly the same as Sequence, but a special case
		 * to be used in conjunction with SkippableSection
		 */
	}
}
