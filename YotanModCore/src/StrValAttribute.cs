namespace YotanModCore
{
	[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property, AllowMultiple = false)]
	internal class StrValAttribute : System.Attribute
	{
		public readonly string StrVal;

		public bool Obsolete { get; set; }

		public StrValAttribute(string strVal, bool obsolete = false)
		{
			StrVal = strVal;
			Obsolete = obsolete;
		}
	}
}
