namespace YotanModCore
{
	[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	internal class StrValAttribute : System.Attribute
	{
		public readonly string StrVal;

		public StrValAttribute(string strVal)
		{
			StrVal = strVal;
		}
	}
}