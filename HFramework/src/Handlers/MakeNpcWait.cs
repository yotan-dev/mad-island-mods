using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HFramework.Patches;
using HFramework.Scenes;
using UnityEngine;

namespace HFramework.Handlers
{
	/// <summary>
	/// Changes NPCs act type to "Wait" and yields until all NPCs are waiting
	/// </summary>
	public class MakeNpcWait : BaseHandler
	{
		private readonly CommonStates[] Npcs;

		private Dictionary<CommonStates, bool> IsWaiting = new Dictionary<CommonStates, bool>();

		public MakeNpcWait(IScene scene, CommonStates[] npcs) : base(scene)
		{
			this.Npcs = npcs;
			NpcMovePatches.OnActTypeChanged += OnActTypeChanged;
		}

		private void OnActTypeChanged(object sender, NPCMove.ActType e)
		{
			if (sender is not NPCMove nMove)
			{
				PLogger.LogDebug($"MakeNpcWait - OnActTypeChanged - sender is not NPCMove");
				return;
			}

			var targetNpc = this.Npcs.FirstOrDefault(n => n.nMove == nMove);
			if (targetNpc == null)
				return;

			if (e == NPCMove.ActType.Wait)
				this.IsWaiting[targetNpc] = true;
		}

		protected override IEnumerator Run()
		{
			bool areAllWaiting = true;
			foreach (var npc in this.Npcs)
			{
				this.IsWaiting.Add(npc, npc.nMove.actType == NPCMove.ActType.Wait);

				areAllWaiting = areAllWaiting && npc.nMove.actType == NPCMove.ActType.Wait;
				npc.nMove.actType = NPCMove.ActType.Wait;
			}

			if (areAllWaiting)
				yield break;

			yield return new WaitUntil(() =>
			{
				bool areAllWaiting = true;
				foreach (var npc in this.Npcs)
					areAllWaiting = areAllWaiting && this.IsWaiting[npc];

				return areAllWaiting;
			});
		}
	}
}
