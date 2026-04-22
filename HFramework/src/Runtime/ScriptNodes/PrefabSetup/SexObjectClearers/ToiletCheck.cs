using HFramework.SexScripts.ScriptContext;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup.SexObjectClearers
{
	public class ToiletCheck : SexObjectClearer
	{
		public override void Clear(CommonContext ctx) {
			Managers.sexMN.StartCoroutine(Managers.mn.gameMN.ToiletCheck(ctx.SexPlace.gameObject.GetComponent<InventorySlot>(), 0));
		}
	}
}
