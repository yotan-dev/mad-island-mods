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

			LayerMask mask = LayerMask.GetMask(["BG"]);
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
			switch (gender)
			{
				case Gender.Female:
					if (GameInfo.HasDLC)
						return NpcID.NativeGirl;
					else
						return NpcID.FemaleNative;

				case Gender.Male:
					if (GameInfo.HasDLC)
						return NpcID.NativeBoy;
					else if (CommonUtils.GetPregnantFatherId(this.Girl) == NpcID.Man)
						return NpcID.YoungMan;
					else
						return NpcID.MaleNative;

				default:
					PLogger.LogError("GetChildNpcId: Unexpected gender: " + gender);
					return NpcID.MaleNative;
			}
		}

		protected override IEnumerator Run()
		{
			if (Random.Range(0, 100) > this.SuccessRate)
			{
				yield return HookManager.Instance.RunEventHook(this.Scene, EventNames.OnStillbirth, null);
				Managers.mn.itemMN.GetItem(Managers.mn.itemMN.FindItem("orb_life_01"), 1);
				string failureLog = Managers.mn.textMN.texts[15].Replace("XXXX", this.Girl.charaName);
				Managers.mn.uiMN.GoLogText(failureLog);
				yield break;
			}

			this.GetChildGender(out int gender, out string addText);
			int npcId = this.GetChildNpcId(gender);

			yield return Managers.mn.npcMN.GenSpawn(npcId, this.Girl.gameObject, null, 0f, null, true);

			CommonStates tmpGenerate = Managers.mn.npcMN.tmpGenerate;
			Managers.mn.npcMN.tmpGenerate = null;
			if (gender == Gender.Female)
				tmpGenerate.charaName = Managers.mn.randChar.GirlNaming();
			else
				tmpGenerate.charaName = Managers.mn.randChar.ManNaming();

			tmpGenerate.age = 0;
			tmpGenerate.friendID = Managers.mn.npcMN.CreateFriendID();
			Managers.mn.npcMN.NPCtoFriend(tmpGenerate.gameObject, true, false);
			Managers.mn.randChar.GenChildSet(tmpGenerate, this.Girl);
			
			yield return HookManager.Instance.RunEventHook(this.Scene, EventNames.OnBirth, null);
			string log = addText + Managers.mn.textMN.texts[14].Replace("XXXX", this.Girl.charaName).Replace("YYYY", tmpGenerate.charaName);
			Managers.mn.uiMN.GoLogText(log);
		}
	}
}
