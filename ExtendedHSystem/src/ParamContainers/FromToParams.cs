namespace ExtendedHSystem.ParamContainers
{
	public struct FromToParams
	{
		public CommonStates From;
		public CommonStates To;

		public FromToParams(CommonStates from, CommonStates to)
		{
			this.From = from;
			this.To = to;
		}
	}
}
