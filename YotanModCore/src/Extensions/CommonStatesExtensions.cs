using UnityEngine;
using UnityEngine.Rendering;
using YotanModCore.Consts;

namespace YotanModCore.Extensions
{
	public static class CommonStatesExtensions
	{
		/// <summary>
		/// Deals damage to the CommonStates.
		/// </summary>
		/// <param name="_this">The CommonStates to deal damage to.</param>
		/// <param name="dmg">The damage information.</param>
		public static void TakeDamage(this CommonStates _this, DamageInfo dmg)
		{
			if (
				_this.dead == 1
				|| _this.invincible
				|| _this.life <= 0.0
				|| (_this.nMove?.orderType == NPCMove.OrderType.Undying && _this.debuff.discontent != 4)
			)
				return;

			// float num = 0.0f;
			// @TODO: The game originally receives a rate and uses this function to apply modifiers (food/defence/etc)
			//        Is there a better way to handle that?
			// if (dmg.Attacker?.attack > 0.0f)
			// {
			// 	Managers2.mn.sound.Go3DSound(dmg.HitSound, _this.transform.position, true, Managers2.mn.sound.soundBaseDist);
			// 	// num = this.DamageCheck(dmg.Attacker, damageRate);
			// 	_this.life = Math.Clamp(_this.life -= dmg.Damage, 0.0, _this.maxLife);
			// 	Managers2.mn.fxMN.GoDamageText(-(double)dmg.Damage, _this.transform.position + new Vector3(0.0f, 1.5f, 0.0f), 0);
			// }

			if (dmg.Damage > 0.0f)
			{
				Managers.mn.sound.Go3DSound(dmg.HitSound, _this.transform.position, true, Managers.mn.sound.soundBaseDist);
				_this.life = System.Math.Clamp(_this.life - dmg.Damage, 0.0, _this.maxLife);
				Managers.mn.fxMN.GoDamageText(-dmg.Damage, _this.transform.position + new Vector3(0.0f, 1.5f, 0.0f), 0);
			}

			if (dmg.Attacker == _this || dmg.Attacker?.CompareTag(Tags.Player) == true)
				Managers.mn.gameMN.StartCoroutine(Managers.mn.gameMN.EnemyLifeChange(_this));

			if (_this.life > 0.0)
			{

				if (_this.nMove == null)
					_this.nMove = _this.GetComponent<NPCMove>();

				if (_this.nMove.npcType == NPCMove.NPCType.Friend || _this.nMove.npcType == NPCMove.NPCType.Follow)
					_this.ExpChange(dmg.Damage);

				if (dmg.Attacker == _this)
					return; // Self attack, stop here

				if (_this.nMove.tmpEnemy == null)
				{
					_this.nMove.tmpEnemy = dmg.Attacker.gameObject;
					if (dmg.Attacker?.gameObject?.CompareTag(Tags.Player) == true)
						Managers.mn.gameMN.battler.Add(_this.nMove);
					_this.nMove.actType = NPCMove.ActType.Damage;
				}
				else
				{
					if (_this.nMove.tmpEnemy == dmg.Attacker.gameObject)
						return;

					CommonStates currentEnemy = _this.nMove.tmpEnemy.GetComponent<CommonStates>();
					if (Random.Range(0.0f, 100f) > 30.0 && currentEnemy.life > 0.0 && currentEnemy.faint > 0.0)
						return;

					_this.nMove.tmpEnemy = dmg.Attacker.gameObject;
					if (dmg.Attacker?.gameObject?.CompareTag(Tags.Player) == true)
						Managers.mn.gameMN.battler.Add(_this.nMove);

					if (_this.nMove.actType != NPCMove.ActType.Idle && _this.nMove.actType != NPCMove.ActType.Chase && _this.nMove.actType != NPCMove.ActType.Interval)
						return;

					_this.nMove.actType = NPCMove.ActType.Damage;
				}
			}
			else if (dmg.Attacker?.CompareTag(Tags.Player) == true)
			{
				dmg.Attacker.ExpChange(_this.exp);
				dmg.Attacker.GetComponent<PlayerMove>().target = null;

				_this.NPCDeath(dmg.DestroyBody, true);
			}
			else
			{
				if (dmg.Attacker?.CompareTag(Tags.NPC) == false)
					return;

				if (dmg.Attacker?.nMove?.npcType == NPCMove.NPCType.Friend || dmg.Attacker?.nMove?.npcType == NPCMove.NPCType.Follow)
					dmg.Attacker.ExpChange(_this.exp);

				var attackerNmove = dmg.Attacker?.GetComponent<NPCMove>();
				if (attackerNmove != null)
					attackerNmove.tmpEnemy = null;

				_this.NPCDeath(dmg.DestroyBody);
			}
		}
	}
}
