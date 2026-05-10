using YotanModCore;

namespace HFramework.Events.EventHandlers
{
	internal class DeliveryEventHandler
	{
		internal static DeliveryEventHandler Instance = new();

		internal void Init() {
			SexEvents.OnBirth.Triggered += this.OnBirth;
			SexEvents.OnStillbirth.Triggered += this.OnStillbirth;
		}

		private void OnBirth(object sender, BirthEventArgs e) {
			// "[Wish come true.]"  message if it was affected by items
			string itemEffectLog = "";
			if (e.Child?.AffectedByItems ?? false) {
				itemEffectLog = Managers.mn.textMN.logTexts[26];
			}

			// "XXXX named the child YYYY."
			string birthMessage = Managers.mn.textMN.texts[14]
				.Replace("XXXX", e.Mother?.charaName ?? "Unknown")
				.Replace("YYYY", e.Child?.Common.charaName ?? "Unknown");

			Managers.mn.uiMN.GoLogText($"{itemEffectLog}{birthMessage}");

			if (e.Mother == null)
				return;

			Managers.sexMN.SexCountChange(e.Mother, null, SexManager.SexCountState.Delivery);
			e.Mother.age++;

			Managers.sexMN.Pregnancy(e.Mother, null, false);
		}

		private void OnStillbirth(object sender, BirthEventArgs e) {
			Managers.mn.itemMN.GetItem(Managers.mn.itemMN.FindItem("orb_life_01"), 1);

			// "XXXX had a stillbirth."
			string failureLog = Managers.mn.textMN.texts[15]
				.Replace("XXXX", e.Mother?.charaName ?? "Unknown");
			Managers.mn.uiMN.GoLogText(failureLog);

			if (e.Mother == null)
				return;

			Managers.sexMN.Pregnancy(e.Mother, null, false);
		}
	}
}
