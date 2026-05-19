using System.Collections;
using HFramework.Events;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Pregnancy
{
	/// <summary>
	/// Tries to spawn a child in a delivery.
	/// </summary>
	public class SpawnChild
	{
		public class ChildData
		{
			public int Gender;

			public int NpcID;

			public bool GenderAffectedByItem;
		}

		private readonly CommonStates Girl;

		public SpawnChild(CommonStates girl) {
			this.Girl = girl;
		}

		private void ChooseChildGender(ChildData childData) {
			LayerMask mask = LayerMask.GetMask(new string[] { "BG" });
			Collider[] array = Physics.OverlapSphere(this.Girl.gameObject.transform.position, 3f, mask);
			if (array.Length != 0) {
				for (int i = 0; i < array.Length; i++) {
					CookSupport component = array[i].gameObject.GetComponent<CookSupport>();
					if (component != null) {
						switch (component.supportType) {
							case CookSupport.SupportType.DeliveryBoy:
								childData.Gender = Gender.Male;
								childData.GenderAffectedByItem = true;
								return;

							case CookSupport.SupportType.DeliveryGirl:
								childData.Gender = Gender.Female;
								childData.GenderAffectedByItem = true;
								return;
						}
					}
				}
			}

			childData.Gender = Random.Range(0, 2) == 0 ? Gender.Female : Gender.Male;
		}

		private void ChooseChildID(ChildData data) {
			if (data.Gender != Gender.Male && data.Gender != Gender.Female) {
				PLogger.LogError("ChooseChildID: Unexpected gender: " + data.Gender);
				data.NpcID = NpcID.MaleNative;
				return;
			}

			int childNpcId;

			// first we determine the real deal
			switch (this.Girl.npcID) {
				case NpcID.Yona:
				case NpcID.YoungLady:
				case NpcID.Daughter:
					childNpcId = data.Gender == Gender.Male ? NpcID.Son : NpcID.Daughter;
					break;

				// Decompiled code "default"
				case NpcID.FemaleNative:
				case NpcID.NativeGirl:
					childNpcId = data.Gender == Gender.Male ? NpcID.NativeBoy : NpcID.NativeGirl;
					break;

				case NpcID.FemaleLargeNative:
				case NpcID.LargeNativeGirl:
					childNpcId = data.Gender == Gender.Male ? NpcID.LargeNativeBoy : NpcID.LargeNativeGirl;
					break;

				case NpcID.UndergroundWoman:
				case NpcID.UndergroundGirl:
					childNpcId = data.Gender == Gender.Male ? NpcID.UnderGroundBoy : NpcID.UndergroundGirl;
					break;

				default:
					PLogger.LogWarning("GetChildNpcId: Unexpected npcID: " + this.Girl.npcID);
					childNpcId = data.Gender == Gender.Male ? NpcID.NativeBoy : NpcID.NativeGirl;
					break;
			}

			data.NpcID = childNpcId;
		}

		/// <summary>
		/// Given an NPC ID, find their replacement ID if they are not available in the game.
		/// A NPC may not be available in the game for a few reasons:
		/// 1. They were introduced in a newer version
		/// 2. They are a DLC character and the game does not have the DLC installed
		///
		/// In those cases, we can usually fallback to another version (e.g. young characters become adults)
		/// </summary>
		/// <param name="npcId">The NPC ID to get the equivalent for.</param>
		/// <param name="motherId">The mother's NPC ID.</param>
		/// <param name="fatherId">The father's NPC ID.</param>
		/// <returns>The equivalent NPC ID.</returns>
		private int GetEquivalentNpcId(int npcId, int motherId = NpcID.None, int fatherId = NpcID.None) {
			// @TODO: Decide whether to move this to YotanModCore or keep here
			// @TODO: Implement the other fallbacks
			// Note: This is only considering some special cases for player character children
			//       We need to implement the other fallbacks here based on NpcManager::GenSpawn

			if (!GameInfo.HasDLC) {
				switch (npcId) {
					case NpcID.NativeBoy:
						if (fatherId == NpcID.Man)
							npcId = NpcID.YoungMan;
						else
							npcId = NpcID.MaleNative;
						break;

					case NpcID.NativeGirl:
						if (GameInfo.GameVersion < GameInfo.ToVersion("0.4.2"))
							npcId = NpcID.FemaleNative;
						break;
				}
			}

			return npcId;
		}

		public IEnumerator Run(CommonContext ctx) {
			var childData = new ChildData();
			this.ChooseChildGender(childData);
			this.ChooseChildID(childData);

			var fatherId = CommonUtils.GetPregnantFatherId(this.Girl);
			childData.NpcID = this.GetEquivalentNpcId(childData.NpcID, this.Girl.npcID, fatherId);

			// Must be spawned as a new Coroutine or it will finish early
			yield return Managers.sexMN.StartCoroutine(Managers.mn.npcMN.GenSpawn(childData.NpcID, this.Girl.gameObject, null, 0f, null, true, true));

			CommonStates tmpGenerate = Managers.mn.npcMN.tmpGenerate;
			Managers.mn.npcMN.tmpGenerate = null;
			if (childData.Gender == Gender.Female)
				tmpGenerate.charaName = Managers.mn.randChar.GirlNaming();
			else
				tmpGenerate.charaName = Managers.mn.randChar.ManNaming();

			tmpGenerate.age = 0;
			tmpGenerate.friendID = Managers.mn.npcMN.CreateFriendID();
			Managers.mn.npcMN.NPCtoFriend(tmpGenerate.gameObject, true, false);
			Managers.mn.randChar.GenChildSet(tmpGenerate, this.Girl);

			var eventArgs = new BirthEventArgs() {
				Ctx = ctx,
				MotherNpcIdx = 0,
				Mother = this.Girl,
				WasBorn = true,
				Child = new BirthEventArgs.ChildDetails(tmpGenerate, false)
			};
			SexEvents.OnBirth.Trigger(eventArgs);
		}
	}
}
