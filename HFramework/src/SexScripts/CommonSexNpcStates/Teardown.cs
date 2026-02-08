#nullable enable

using UnityEngine;

namespace HFramework.SexScripts.CommonSexNpcStates
{
	[Experimental]
	public class Teardown : BaseState
	{
		public override void OnEnter(CommonSexNpcScript script)
		{
			PLogger.LogDebug("Teardown.OnEnter");
			base.OnEnter(script);
		}

		public override void OnExit(CommonSexNpcScript script)
		{
			PLogger.LogDebug("Teardown.OnExit");
			base.OnExit(script);
		}

		private void RestoreLivingCharacter(CommonStates? character, float? searchAngle)
		{
			if (character == null)
				return;

			var col = character.GetComponent<CapsuleCollider>();
			if (col != null)
				col.enabled = true;

			var mesh = character.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			var move = character.nMove;
			if (move.actType == NPCMove.ActType.Wait)
				move.actType = NPCMove.ActType.Travel;
			else
				move.actType = NPCMove.ActType.Interval;

			if (searchAngle != null)
				move.searchAngle = searchAngle.Value;
			character.sex = CommonStates.SexState.None;
			if (character.debuff.perfume <= 0.0)
				character.libido -= 20f;
		}

		public override void Update(CommonSexNpcScript script)
		{
			Debug.Log("Teardown.Update");
			if (script.TmpSex != null)
				UnityEngine.Object.Destroy(script.TmpSex);

			if (script.SexPlace != null)
				script.SexPlace.user = null;

			RestoreLivingCharacter(script.NpcA, script.NpcAAngle);
			RestoreLivingCharacter(script.NpcB, script.NpcBAngle);

			script.ChangeState(null);

			// @TODO: Moral change
			// npcA.MoralChange(3f);
			// npcB.MoralChange(3f);
		}
	}
}
