using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace HFramework.Events
{
	[Experimental]
	public static class SexEvents
	{
		// Interactions
		[SexEvent("HF", "Interact: Vagina to Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnInteractVagina2Vagina = new("HF.interact.vagina2vagina");

		[SexEvent("HF", "Interact: Tongue to Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnInteractTongue2Vagina = new("HF.interact.tongue2vagina");

		[SexEvent("HF", "Interact: Penis to Mouth")]
		public static readonly SexEvent<FromToEventArgs> OnInteractPenis2Mouth = new("HF.interact.penis2mouth");

		[SexEvent("HF", "Interact: Penis to Tits")]
		public static readonly SexEvent<FromToEventArgs> OnInteractPenis2Tits = new("HF.interact.penis2tits");

		[SexEvent("HF", "Interact: Penis to Hand")]
		public static readonly SexEvent<FromToEventArgs> OnInteractPenis2Hand = new("HF.interact.penis2hand");

		[SexEvent("HF", "Interact: Penis to Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnInteractPenis2Vagina = new("HF.interact.penis2vagina");

		[SexEvent("HF", "Interact: Penis to Ass")]
		public static readonly SexEvent<FromToEventArgs> OnInteractPenis2Ass = new("HF.interact.penis2ass");

		// Self-interactions
		[SexEvent("HF", "Self-interact: Hand to Vagina")]
		public static readonly SexEvent<SelfEventArgs> OnSelfHand2Vagina = new("HF.self-interact.hand2vagina");

		[SexEvent("HF", "Self-interact: Hand to Penis")]
		public static readonly SexEvent<SelfEventArgs> OnSelfHand2Penis = new("HF.self-interact.hand2penis");



		// General performance (e.g. starting to do something) -- Deprecated
#region Deprecated
		[SexEvent("HF", "[Deprecated] Perform: Scissoring")]
		[Obsolete("Use OnInteractVagina2Vagina instead")]
		public static readonly SexEvent<FromToEventArgs> OnPerformScissor = new("HF.perform.scissoring");

		[SexEvent("HF", "[Deprecated] Perform: Titfuck")]
		[Obsolete("Use OnInteractPenis2Tits instead")]
		public static readonly SexEvent<FromToEventArgs> OnPerformTitFuck = new("HF.perform.titfuck");

		[SexEvent("HF", "[Deprecated] Perform: Handjob")]
		[Obsolete("Use OnInteractPenis2Hand instead")]
		public static readonly SexEvent<FromToEventArgs> OnPerformHandJob = new("HF.perform.handjob");

		[SexEvent("HF", "[Deprecated] Perform: Masturbation")]
		[Obsolete("Use OnSelfHand2Vagina instead")]
		public static readonly SexEvent<SelfEventArgs> OnPerformMasturbation = new("HF.perform.masturbation");

		// Penetration events (dick being inserted somewhere) -- deprecated
		[SexEvent("HF", "[Deprecated] Penetrate: Vagina")]
		[Obsolete("Use OnInteractPenis2Vagina instead")]
		public static readonly SexEvent<FromToEventArgs> OnPenetrateVagina = new("HF.penetrate.vagina");

		[SexEvent("HF", "[Deprecated] Penetrate: Ass")]
		[Obsolete("Use OnInteractPenis2Ass instead")]
		public static readonly SexEvent<FromToEventArgs> OnPenetrateAss = new("HF.penetrate.ass");

		[SexEvent("HF", "[Deprecated] Penetrate: Mouth")]
		[Obsolete("Use OnInteractPenis2Mouth instead")]
		public static readonly SexEvent<FromToEventArgs> OnPenetrateMouth = new("HF.penetrate.mouth");

		// Lick events (tongue being used) -- deprecated
		[SexEvent("HF", "[Deprecated] Lick: Vagina")]
		[Obsolete("Use OnInteractTongue2Vagina instead")]
		public static readonly SexEvent<FromToEventArgs> OnLickVagina = new("HF.lick.vagina");
#endregion

		// Sex "finishings"
		[SexEvent("HF", "Orgasm")]
		public static readonly SexEvent<SelfEventArgs> OnOrgasm = new("HF.orgasm");

		[SexEvent("HF", "Cum: Mouth")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnMouth = new("HF.cum.mouth");

		[SexEvent("HF", "Cum: Tits")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnTits = new("HF.cum.tits");

		[SexEvent("HF", "Cum: Vagina")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnVagina = new("HF.cum.vagina");

		[SexEvent("HF", "Cum: Ass")]
		public static readonly SexEvent<FromToEventArgs> OnCumOnAss = new("HF.cum.ass");

		// Pregnancy
		[SexEvent("HF", "Perform: Delivery")]
		public static readonly SexEvent<SelfEventArgs> OnPerformDelivery = new("HF.perform.delivery");

		[SexEvent("HF", "Birth")]
		public static readonly SexEvent<BirthEventArgs> OnBirth = new("HF.birth");

		[SexEvent("HF", "Stillbirth")]
		public static readonly SexEvent<BirthEventArgs> OnStillbirth = new("HF.stillbirth");

		[SexEvent("HF", "End")]
		public static readonly SexEvent<SexEventArgs> OnEnd = new("HF.end");

		[SexEvent("HF", "Noop")]
		public static readonly SexEvent<SexEventArgs> OnNoop = new("HF.noop");

		public static readonly Dictionary<string, SexEventInfo> Events = new();

		static SexEvents()
		{
			Events = new Dictionary<string, SexEventInfo>();

#if UNITY_EDITOR
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
#else
			var t = typeof(SexEvents);
			var fields = t.GetFields(BindingFlags.Public | BindingFlags.Static);
			foreach (var fld in fields)
			{
				var attr = fld.GetCustomAttribute<SexEventAttribute>();
				if (attr != null)
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
#endif
		}
	}
}
