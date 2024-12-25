using System;
using UnityEngine;
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

		private GameObject Root;

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
			this.Root = sexMeterBar.transform.parent.gameObject;
			this.FillingBar = sexMeterBar;
			this.EmptyBg = sexMeterBar.transform.parent.Find("Background (1)").GetComponent<Image>();
			this.HighValueFilledBg = sexMeterBar.transform.Find("Background (1)").GetComponent<Image>();
		}

		public void Init(Vector3 position, float dividerPercent)
		{
			this.Root.transform.position = position;
			this.SetFillAmount(0f);
			this.RealValue = 0f;
			this.SetDividerPercent(dividerPercent);
		}

		public void Show()
		{
			this.Root.SetActive(true);
		}

		public void Hide()
		{
			this.Root.SetActive(false);
		}

		public void SetFillAmount(float value)
		{
			this.FillingBar.fillAmount = Math.Clamp(value, 0f, 1f);
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