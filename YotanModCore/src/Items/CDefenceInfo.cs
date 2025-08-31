#nullable enable

using System;
using UnityEngine;
using YotanModCore.Consts;

namespace YotanModCore.Items
{
	/// <summary>
	/// Bridge to DefenceInfo for custom items.
	///
	/// DefenceInfo is used for placeable objects that can be destroyed,
	/// and at the same time may cause damage on touch.
	///
	/// If the item can't be damaged, it should not use DefenceInfo.
	/// For damage on touch only, use CDamageTrigger instead
	/// </summary>
	public class CDefenceInfo : DefenceInfo
	{
		/// <summary>
		/// Called when the Defence becomes active or inactive
		/// </summary>
		/// <param name="active"></param>
		public event EventHandler<bool>? DefenceActiveStateChanged;

		/// <summary>
		/// Called when the Defence is repaired
		/// </summary>
		public event EventHandler<object>? DefenceRepaired;

		private bool cachedActive = true;

		/// <summary>
		/// SE played when the Defence takes a hit (e.g. from a NPC attack).
		/// See AudioSE
		///
		/// Note: This is only used when you are implementing OnDefenceAttacked and using ChangeLife
		/// </summary>
		public int DefenceDamagedSE = AudioSE.HitMeat01;

		/// <summary>
		/// Whether this defence performs "contact" logic.
		/// When false (not ignoring contact), when NPCs passes nearby, touching the Defence,
		/// they will start to attack it.
		/// When true (ignoring contact), NPCs will simply go on with their business.
		///
		/// This check is ignored if you are handling OnDefenceTouched yourself
		/// </summary>
		public bool IgnoresContact = false;

		/// <summary>
		/// Changes the life of the structure.
		///
		/// If you are handling the damage completely by yourself,
		/// you may want to call ProcessUpDown after this, so the structure can get destroyed if needed.
		/// </summary>
		/// <param name="change"></param>
		public virtual void ChangeLife(CommonStates? attacker, float change, bool damageSound = true)
		{
			this.itemInfo.life += change;
			this.itemInfo.life = Mathf.Clamp(this.itemInfo.life, 0f, this.itemInfo.lifeMax);
			Managers.mn.fxMN.GoDamageText(change, base.transform.position + new Vector3(0f, 1.5f, 0f), 0);

			if (this.itemInfo.life <= 0.0)
			{
				this.Down();

				// clears attacker's target if it was targetting this defence
				if (attacker?.nMove?.tmpEnemy == this.gameObject)
					attacker.nMove.tmpEnemy = null;

				// Player attacks stops here... @TODO: why? just to prevent dropping items? does it make sense?
				if (!Managers.mn.gameMN.IsPlayer(attacker))
					return;

				DropItem dropItem = this.itemInfo.GetComponent<DropItem>();
				if (dropItem != null)
					Managers.mn.gameMN.GetDropItems(dropItem);
			}
			else if (damageSound)
			{
				Managers.mn.sound.Go3DSound(this.DefenceDamagedSE, this.transform.position, true, Managers.mn.sound.soundBaseDist);
			}
		}

		public virtual bool IsAlive()
		{
			return this.activeObjects.Length > 0 && !this.activeObjects[0].activeSelf;
		}

		public virtual bool IsDestroyed()
		{
			return this.brokenObjects.Length > 0 && !this.brokenObjects[0].activeSelf;
		}

		/// <summary>
		/// Called when something Attacks this structure
		/// It should return true if the custom code has handled it, false otherwise.
		/// When false is returned, the original handler will be executed.
		///
		/// Note that, when returning true, the original code won't execute and DefenceCounterAttack
		/// won't trigger. If you intend this to happen, call it manually during your handling.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="fixedDamage"></param>
		/// <returns></returns>
		public virtual bool OnDefenceAttacked(CommonStates attacker, float fixedDamage = 0f)
		{
			return false;
		}

		/// <summary>
		/// Called when calculating how the attacked structure should "Counter Attack".
		/// Counter Attack means an enemy attacked the structure and is getting damaged too, for example,
		/// when an enemy hits a Wall made of Cactus, they get "counter attacked" by the wall.
		///
		/// This is called as part of a Defence being attacked, thus, if OnDefenceAttacked custom handles
		/// this part, this will not be called unless OnDefenceAttacked does it explicitly.
		///
		/// It should return true if counter attack was handled, thus preventing the original counter attack
		/// logic from running. Return false if you want the original logic to run.
		/// </summary>
		/// <param name="attacker"></param>
		/// <param name="fixedDamage"></param>
		/// <returns></returns>
		public virtual bool DefenceCounterAttack(CommonStates attacker, float fixedDamage = 0f)
		{
			return false;
		}

		public virtual bool OnDefenceTouched(Collision collision)
		{
			return this.IgnoresContact;
		}

		/// <summary>
		/// Called when the structure is repaired.
		/// </summary>
		public void OnDefenceRepaired()
		{
			this.DefenceRepaired?.Invoke(this, EventArgs.Empty);
		}

		private void Update()
		{
			if (this.active != this.cachedActive)
			{
				Debug.Log("Active changed from " + this.cachedActive + " to " + this.active);
				this.cachedActive = this.active;
				this.DefenceActiveStateChanged?.Invoke(this, this.active);
			}
		}
	}
}
