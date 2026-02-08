namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class SexFinish : AnimOnce
	{
		// @TODO: Emit events (Orgasm, creampie)
		public SexFinish(BaseState onSuccessState) : base("A_Finish", onSuccessState) { }
	}
}
