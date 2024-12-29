using System.ComponentModel;

/// Workaround for using Records.
/// https://stackoverflow.com/questions/73100829/compile-error-when-using-record-types-with-unity3d
namespace System.Runtime.CompilerServices
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal class IsExternalInit { }
}