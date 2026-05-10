namespace HFramework.ScriptNodes
{
	[System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
	public class ScriptNodeAttribute : System.Attribute
	{
		public readonly string Source;

		public readonly string MenuName;

		public ScriptNodeAttribute(string source, string menuName) {
			Source = source;
			MenuName = menuName;
		}
	}
}
