using System.Collections;
using HFramework.Hook;
using HFramework.Scenes;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Handlers
{
	/// <summary>
	/// Tries to spawn a child in a delivery.
	/// </summary>
	public class SpawnChild : BaseHandler
	{
		private readonly CommonStates Girl;

		private readonly float SuccessRate;

		public SpawnChild(IScene scene, CommonStates girl, float successRate) : base(scene)
		{
			this.Girl = girl;
			this.SuccessRate = successRate;
		}

		private void GetChildGender(out int gender, out string addText)
		{
			addText = "";

			LayerMask mask = LayerMask.GetMask(new string[] { "BG" });
			Collider[] array = Physics.OverlapSphere(this.Girl.gameObject.transform.position, 3f, mask);
			if (array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					CookSupport component = array[i].gameObject.GetComponent<CookSupport>();
					if (component != null)
					{
						switch (component.supportType)
						{
							case CookSupport.SupportType.DeliveryBoy:
								gender = Gender.Male;
								addText = Managers.mn.textMN.logTexts[26];
								return;

							case CookSupport.SupportType.DeliveryGirl:
								gender = Gender.Female;
								addText = Managers.mn.textMN.logTexts[26];
								return;
						}
					}
				}
			}

			gender = Random.Range(0, 2) == 0 ? Gender.Female : Gender.Male;
		}

		private int GetChildNpcId(int gender)
		{
			if (gender != Gender.Male && gender != Gender.Female)
			{
				PLogger.LogWarning("GetChildNpcId: Unexpected gender: " + gender);
				return NpcID.MaleNative;
			}

			int childNpcId;

			// first we determine the real deal
			switch (this.Girl.npcID)
			{
				case NpcID.Yona:
				case NpcID.YoungLady:
				case NpcID.Daughter:
					childNpcId = gender == Gender.Male ? NpcID.Son : NpcID.Daughter;
					break;

				// Decompiled code "default"
				case NpcID.FemaleNative:
				case NpcID.NativeGirl:
					childNpcId = gender == Gender.Male ? NpcID.NativeBoy : NpcID.NativeGirl;
					break;

				case NpcID.FemaleLargeNative:
				case NpcID.LargeNativeGirl:
					childNpcId = gender == Gender.Male ? NpcID.LargeNativeBoy : NpcID.LargeNativeGirl;
					break;

				case NpcID.UndergroundWoman:
				case NpcID.UndergroundGirl:
					childNpcId = gender == Gender.Male ? NpcID.UnderGroundBoy : NpcID.UndergroundGirl;
					break;

				default:
					PLogger.LogWarning("GetChildNpcId: Unexpected npcID: " + this.Girl.npcID);
					childNpcId = gender == Gender.Male ? NpcID.NativeBoy : NpcID.NativeGirl;
					break;
			}

			// Now we fallback to skip DLC stuff.
			// Note: From v0.4.2 onwards, the game handles DLC on NPCManager::GenSpawn
			// So we are only handling edge cases here.
			if (!GameInfo.HasDLC)
			{
				switch (childNpcId)
				{
					case NpcID.NativeBoy:
						if (CommonUtils.GetPregnantFatherId(this.Girl) == NpcID.Man)
							childNpcId = NpcID.YoungMan;
						else
							childNpcId = NpcID.MaleNative;
						break;

					case NpcID.NativeGirl:
						if (GameInfo.GameVersion < GameInfo.ToVersion("0.4.2"))
							childNpcId = NpcID.FemaleNative;
						break;
				}
			}

			return childNpcId;
		}

		protected override IEnumerator Run()
		{
			if (Random.Range(0, 100) > this.SuccessRate)
			{
				yield return HookManager.Instance.RunEventHook(this.Scene, EventNames.OnStillbirth, null);
				Managers.mn.itemMN.GetItem(Managers.mn.itemMN.FindItem("orb_life_01"), 1);
				string failureLog = Managers.mn.textMN.texts[15].Replace("XXXX", this.Girl.charaName);
				Managers.mn.uiMN.GoLogText(failureLog);

				/* TODO:
					string str = sexManager.mn.textMN.texts[15].Replace("XXXX", common.charaName);
					if (!FreeCoopSession.FacilityBirthNotice(sexManager, common, str))
						sexManager.mn.uiMN.GoLogText(str);
				*/
				yield break;
			}

			this.GetChildGender(out int gender, out string addText);

			// TODO: FreePersonRuntime.BirthParentNpcId((Component) sexManager, common.pregnant[0], friend);
			int npcId = this.GetChildNpcId(gender);

			yield return Managers.mn.npcMN.GenSpawn(npcId, this.Girl.gameObject, null, 0f, null, true);

			/* TODO:
			int num2 = FreePersonRuntime.ChildSpawnId((Component) sexManager, selected);
            if (FreePersonRuntime.InvalidChildSpawn((Component) sexManager, num2, common, tmpWorkPlace, tmpSexPlace, tmpSearchAngle))
              yield break;
            if (num2 != -1)
              yield return (object) sexManager.mn.npcMN.GenSpawn(num2, common.gameObject, (GameObject) null, 0.0f, (EnemySpawner) null, true, true);
            else
              Debug.LogError((object) "child not found");
			*/

			CommonStates tmpGenerate = Managers.mn.npcMN.tmpGenerate;
			Managers.mn.npcMN.tmpGenerate = null;

			/* TODO:
			if (FreePersonRuntime.MissingGeneratedChild((Component) sexManager, tmpGenerate, common, tmpWorkPlace, tmpSexPlace, tmpSearchAngle))
              yield break;
			*/

			if (gender == Gender.Female)
				tmpGenerate.charaName = Managers.mn.randChar.GirlNaming();
			else
				tmpGenerate.charaName = Managers.mn.randChar.ManNaming();

			tmpGenerate.age = 0;
			tmpGenerate.friendID = Managers.mn.npcMN.CreateFriendID();
			tmpGenerate.houseID = this.Girl.houseID;
			Managers.mn.npcMN.NPCtoFriend(tmpGenerate.gameObject, true, false);
			Managers.mn.randChar.GenChildSet(tmpGenerate, this.Girl);

			yield return HookManager.Instance.RunEventHook(this.Scene, EventNames.OnBirth, null);
			string log = addText + Managers.mn.textMN.texts[14].Replace("XXXX", this.Girl.charaName).Replace("YYYY", tmpGenerate.charaName);
			Managers.mn.uiMN.GoLogText(log);

			/* TODO:
				string str = addText + sexManager.mn.textMN.texts[14].Replace("XXXX", common.charaName).Replace("YYYY", tmpGenerate.charaName);
				if (!FreeCoopSession.FacilityBirthNotice(sexManager, common, str))
				sexManager.mn.uiMN.GoLogText(str);
			*/
		}
	}
}
