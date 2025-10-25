using System.Collections;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;
using YotanModCore.Items;

namespace YoCustomItems
{
	public class LandMine : MonoBehaviour
	{
		public float ExplosionTime = 0.5f;

		public float ExplosionRange = 15f;

		public int Damage = 1000;

		private bool Triggered = false;

		private CDefenceInfo DefenceInfo;

		private CustomItemInfo ItemInfo;

		private BoxCollider Collider;

		private void Awake()
		{
			this.DefenceInfo = this.gameObject.GetComponentInParent<CDefenceInfo>();
			this.ItemInfo = this.gameObject.GetComponentInParent<CustomItemInfo>();
			this.Collider = this.gameObject.GetComponent<BoxCollider>();

			this.DefenceInfo.DefenceActiveStateChanged += (sender, active) => this.Collider.enabled = active;
			this.DefenceInfo.DefenceRepaired += (sender, _) => this.Triggered = false;
		}

		private IEnumerator TriggerExplosion()
		{
			if (Triggered)
				yield break;

			Triggered = true;

			yield return new WaitForSeconds(this.ExplosionTime);

			Managers.mn.fxMN.GoFX(FX.ExplosionBig, this.gameObject.transform.position);
			Managers.mn.sound.GoSound(AudioSE.ExplosionFear01, this.gameObject.transform.position);
			Collider[] colliderArray = Physics.OverlapSphere(this.gameObject.transform.position, this.ExplosionRange, (int)(LayerMask)LayerMask.GetMask(Layers.Chara));
			for (int index = 0; index < colliderArray.Length; ++index)
			{
				var common = colliderArray[index].GetComponent<CommonStates>();
				if (common.employ == CommonStates.Employ.None)
					common.TakeDamage(new DamageInfo() { Damage = this.Damage, DestroyBody = true });
			}

			this.DefenceInfo.ChangeLife(null, -this.ItemInfo.life, false);
		}

		private void OnTriggerEnter(Collider other)
		{
			// Check for active, because we might we in placement mode (which should not trigger)
			if (!this.DefenceInfo.active)
				return;

			if (other.gameObject.tag != Tags.NPC)
				return;

			CommonStates target = other.gameObject.GetComponent<CommonStates>();
			if (target?.nMove == null || target.groupID == 0)
				return;

			if (!this.Triggered)
				StartCoroutine(TriggerExplosion());
		}
	}
}
