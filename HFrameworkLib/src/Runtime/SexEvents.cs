using System;

namespace HFramework
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
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

	public static class SexEvents
	{
		// General performance (e.g. starting to do something)
		[SexEvent("HF", "Perform: Scissoring")]
		public static readonly SexEvent OnPerformScissor = new SexEvent("HF.perform.scissoring");
		[SexEvent("HF", "Perform: Titfuck")]
		public static readonly SexEvent OnPerformTitFuck = new SexEvent("HF.perform.titfuck");
		[SexEvent("HF", "Perform: Handjob")]
		public static readonly SexEvent OnPerformHandJob = new SexEvent("HF.perform.handjob");
		[SexEvent("HF", "Perform: Masturbation")]
		public static readonly SexEvent OnPerformMasturbation = new SexEvent("HF.perform.masturbation");
		[SexEvent("HF", "Perform: Delivery")]
		public static readonly SexEvent OnPerformDelivery = new SexEvent("HF.perform.delivery");

		// Penetration events (dick being inserted somewhere)
		[SexEvent("HF", "Penetrate: Vagina")]
		public static readonly SexEvent OnPenetrateVagina = new SexEvent("HF.penetrate.vagina");
		[SexEvent("HF", "Penetrate: Ass")]
		public static readonly SexEvent OnPenetrateAss = new SexEvent("HF.penetrate.ass");
		[SexEvent("HF", "Penetrate: Mouth")]
		public static readonly SexEvent OnPenetrateMouth = new SexEvent("HF.penetrate.mouth");

		// Lick events (tongue being used)
		[SexEvent("HF", "Lick: Vagina")]
		public static readonly SexEvent OnLickVagina = new SexEvent("HF.lick.vagina");

		// Sex "finishings"
		[SexEvent("HF", "Orgasm")]
		public static readonly SexEvent OnOrgasm = new SexEvent("HF.orgasm");
		[SexEvent("HF", "Cum: Vagina")]
		public static readonly SexEvent OnCumOnVagina = new SexEvent("HF.cum.vagina");
		[SexEvent("HF", "Cum: Ass")]
		public static readonly SexEvent OnCumOnAss = new SexEvent("HF.cum.ass");
		[SexEvent("HF", "Cum: Mouth")]
		public static readonly SexEvent OnCumOnMouth = new SexEvent("HF.cum.mouth");
		[SexEvent("HF", "Cum: Tits")]
		public static readonly SexEvent OnCumOnTits = new SexEvent("HF.cum.tits");

		// Delivery outcomes
		[SexEvent("HF", "Delivery: Birth")]
		public static readonly SexEvent OnGiveBirth = new SexEvent("HF.delivery.birth");
		[SexEvent("HF", "Delivery: Stillbirth")]
		public static readonly SexEvent OnStillbirth = new SexEvent("HF.delivery.stillbirth");

		// Other
		[SexEvent("HF", "Other: Player Defeated")]
		public static readonly SexEvent OnPlayerDefeated = new SexEvent("HF.other.playerDefeated");
	}
}
