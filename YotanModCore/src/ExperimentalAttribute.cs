namespace YotanModCore
{
	[System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Method | System.AttributeTargets.Class, AllowMultiple = false)]
	internal class ExperimentalAttribute : System.Attribute
	{
		public readonly string Reason;

		public ExperimentalAttribute(string reason = "")
		{
			Reason = reason;
		}
	}
}
