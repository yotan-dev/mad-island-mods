using System;

namespace HFramework.Events
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	[Experimental]
	public class SexEventAttribute : Attribute
	{
		public string Source;
		public string Name;

		public SexEventAttribute(string source, string name)
		{
			Source = source;
			Name = name;
		}
	}
}
