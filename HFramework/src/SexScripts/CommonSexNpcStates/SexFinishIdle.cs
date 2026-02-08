namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class SexFinishIdle : LoopAnimForTime
	{
		public SexFinishIdle(BaseState onSuccessState) : base("A_Finish_idle", 8f, onSuccessState) { }
	}
}
