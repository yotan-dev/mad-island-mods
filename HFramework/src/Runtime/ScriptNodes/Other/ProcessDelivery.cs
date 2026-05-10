using System.Collections;
using HFramework.Events;
using HFramework.Pregnancy;
using UnityEngine;

namespace HFramework.ScriptNodes.Other
{
	/// <summary>
	/// Handles the delivery process (checking for birth change/triggering birth or stillbirth)
	/// </summary>
	[ScriptNode("HFramework", "Other/Process Delivery")]
	public class ProcessDelivery : Action
	{
		[ActorIndex]
		public int PregnantActor = 0;

		private CommonStates PregnantCommon;

		private bool Born;

		private IEnumerator SpawnChildCoroutine;

		protected override void OnStart() {
			PregnantCommon = this.Context.Actors[PregnantActor].Common;
			var birthChanceCalculator = new BirthChanceCalculator();
			var birthChance = birthChanceCalculator.CalculateBirthChance(PregnantCommon, this.Context.ScriptPlace);

			if (Random.Range(0, 100) > birthChance) {
				Born = false;
			} else {
				Born = true;
				SpawnChildCoroutine = new SpawnChild(PregnantCommon).Run(this.Context);
			}
		}

		protected override State OnUpdate() {
			if (!Born) {
				SexEvents.OnStillbirth.Trigger(new BirthEventArgs() {
					ctx = this.Context,
					MotherNpcIdx = PregnantActor,
					Mother = PregnantCommon,
					WasBorn = false,
					Child = null,
				});
				return State.Success;
			}

			if (SpawnChildCoroutine != null && SpawnChildCoroutine.MoveNext()) {
				return State.Running;
			}

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
