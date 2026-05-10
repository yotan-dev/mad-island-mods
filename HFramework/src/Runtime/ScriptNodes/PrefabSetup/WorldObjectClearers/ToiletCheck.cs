using HFramework.SexScripts.ScriptContext;
using YotanModCore;

namespace HFramework.ScriptNodes.PrefabSetup.WorldObjectClearers
{
	public class ToiletCheck : WorldObjectClearer
	{
		public override void Clear(CommonContext ctx) {
			Managers.sexMN.StartCoroutine(Managers.mn.gameMN.ToiletCheck(ctx.ScriptPlace.GetObject().GetComponent<InventorySlot>(), 0));
		}
	}
}
