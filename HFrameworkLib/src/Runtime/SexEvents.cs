using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

	public class SexEventInfo
	{
		public ISexEventBase Event { get; private set; }
		public Type EventType { get; private set; }

		public void SetEvent<T>(ISexEvent<T> evt) where T : BaseSexEventArgs, new()
		{
			Event = evt;
			EventType = typeof(T);
		}
	}

	public static class SexEvents
	{
		// General performance (e.g. starting to do something)
		[SexEvent("HF", "Perform: Scissoring")]
		public static readonly SexEvent<FromToEventArgs> OnPerformScissor = new SexEvent<FromToEventArgs>("HF.perform.scissoring");
		[SexEvent("HF", "Perform: Titfuck")]
		public static readonly SexEvent<FromToEventArgs> OnPerformTitFuck = new SexEvent<FromToEventArgs>("HF.perform.titfuck");
		[SexEvent("HF", "Perform: Handjob")]
		public static readonly SexEvent<FromToEventArgs> OnPerformHandJob = new SexEvent<FromToEventArgs>("HF.perform.handjob");
		[SexEvent("HF", "Perform: Masturbation")]
		public static readonly SexEvent<SelfEventArgs> OnPerformMasturbation = new SexEvent<SelfEventArgs>("HF.perform.masturbation");
		[SexEvent("HF", "Perform: Delivery")]
		public static readonly SexEvent<FromToEventArgs> OnPerformDelivery = new SexEvent<FromToEventArgs>("HF.perform.delivery");

		// Penetration events (dick being inserted somewhere)
		[SexEvent("HF", "Penetrate: Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnPenetrateVagina = new SexEvent<FromToEventArgs>("HF.penetrate.vagina");
		[SexEvent("HF", "Penetrate: Ass")]
		public static readonly SexEvent<FromToEventArgs> OnPenetrateAss = new SexEvent<FromToEventArgs>("HF.penetrate.ass");
		[SexEvent("HF", "Penetrate: Mouth")]
		public static readonly SexEvent<FromToEventArgs> OnPenetrateMouth = new SexEvent<FromToEventArgs>("HF.penetrate.mouth");

		// Lick events (tongue being used)
		[SexEvent("HF", "Lick: Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnLickVagina = new SexEvent<FromToEventArgs>("HF.lick.vagina");

		// Sex "finishings"
		[SexEvent("HF", "Orgasm")]
		public static readonly SexEvent<FromToEventArgs> OnOrgasm = new SexEvent<FromToEventArgs>("HF.orgasm");
		[SexEvent("HF", "Cum: Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnVagina = new SexEvent<FromToEventArgs>("HF.cum.vagina");
		[SexEvent("HF", "Cum: Ass")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnAss = new SexEvent<FromToEventArgs>("HF.cum.ass");
		[SexEvent("HF", "Cum: Mouth")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnMouth = new SexEvent<FromToEventArgs>("HF.cum.mouth");
		[SexEvent("HF", "Cum: Tits")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnTits = new SexEvent<FromToEventArgs>("HF.cum.tits");

		// Delivery outcomes
		[SexEvent("HF", "Delivery: Birth")]
		public static readonly SexEvent<FromToEventArgs> OnGiveBirth = new SexEvent<FromToEventArgs>("HF.delivery.birth");
		[SexEvent("HF", "Delivery: Stillbirth")]
		public static readonly SexEvent<FromToEventArgs> OnStillbirth = new SexEvent<FromToEventArgs>("HF.delivery.stillbirth");

		// Other
		[SexEvent("HF", "Other: Player Defeated")]
		public static readonly SexEvent<FromToEventArgs> OnPlayerDefeated = new SexEvent<FromToEventArgs>("HF.other.playerDefeated");

		public static readonly Dictionary<string, SexEventInfo> Events = new Dictionary<string, SexEventInfo>();

		static SexEvents()
		{
			Events = new Dictionary<string, SexEventInfo>();

			var fields = TypeCache.GetFieldsWithAttribute<SexEventAttribute>();
			foreach (var fld in fields)
			{
				var evt = fld.GetValue(null);
				var eventType = fld.FieldType;
				var eventInstance = evt as ISexEventBase;

				if (eventInstance != null)
				{
					var eventInfo = new SexEventInfo();
					var genericType = eventType.GetGenericArguments()[0];
					var setMethod = typeof(SexEventInfo).GetMethod("SetEvent")
						.MakeGenericMethod(genericType);
					setMethod.Invoke(eventInfo, new[] { evt });

					Events[eventInstance.GetId()] = eventInfo;
				}
			}

		}
	}
}
