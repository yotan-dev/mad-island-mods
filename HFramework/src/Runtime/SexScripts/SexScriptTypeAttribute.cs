using System;

namespace HFramework.SexScripts
{
	[AttributeUsage(AttributeTargets.Class)]
	public class SexScriptTypeAttribute : Attribute
	{
		public string TypeName { get; }

		public SexScriptTypeAttribute(string typeName) {
			TypeName = typeName;
		}
	}
}
