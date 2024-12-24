using UnityEngine.UI;
using YotanModCore;

namespace ExtendedHSystem.Scenes
{
	public class SexMeter
	{
		public static SexMeter Instance { get; private set; } = new SexMeter();

		public float FillAmount { get { return this.FillingBar.fillAmount; } }

		public float RealValue { get; private set; }

		public float DividerPercent { get; private set; }

		private Image EmptyBg;

		private Image HighValueFilledBg;

		private Image FillingBar;

		private SexMeter()
		{
			this.Reload();
		}

		public void Reload()
		{
			var sexMeterBar = Managers.mn.sexMN.sexMeter;
			this.FillingBar = sexMeterBar;
			this.EmptyBg = sexMeterBar.transform.parent.Find("Background (1)").GetComponent<Image>();
			this.HighValueFilledBg = sexMeterBar.transform.Find("Background (1)").GetComponent<Image>();
		}

		public void Init(float dividerPercent)
		{
			this.SetFillAmount(0f);
			this.RealValue = 0f;
			this.SetDividerPercent(dividerPercent);
		}

		public void SetFillAmount(float value)
		{
			this.FillingBar.fillAmount = value;
			this.RealValue = value;
		}

		public void Fill(float amount)
		{
			this.SetFillAmount(this.FillAmount + amount);
		}

		public void SetDividerPercent(float value)
		{
			this.DividerPercent = value;
			this.EmptyBg.fillAmount = value;
			this.HighValueFilledBg.fillAmount = value;
		}
	}
}