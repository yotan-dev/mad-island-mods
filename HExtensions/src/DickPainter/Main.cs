using System.Collections;
using System.Collections.Generic;
using HFramework.Hook;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;

namespace HExtensions.DickPainter
{
	/// <summary>
	/// - Paints dick juice in red if female is virgin.
	/// - Paints male dick in a pinkish if any actor is using a condom
	/// </summary>
	public class Main
	{
		private const string MemoryId = "HExt.VirginMemory";

		public void Init()
		{
			HookManager.RegisterHooksEvent += () => this.InitHooks();
		}

		private Dictionary<string, string[]> VirginJuices = new Dictionary<string, string[]>()
		{
			{ "HF_Toilet_Man_FemaleNative",       new string[] { "TinkoJuice" } }, // gengirl_01_toilet
			{ "HF_Toilet_Man_NativeGirl",         new string[] { "TinkoJuice" } }, // gengirl_02_toilet
			// ?? gengirl_01_toilet_gen_01
			// ?? gengirl_02_toilet_gen_01
			// AssWall
			{ "HF_AssWall_Man_FemaleNative",      new string[] { "TinkoJuice" } }, // gengirl_01_wall (15)
			{ "HF_AssWall_Man_NativeGirl",        new string[] { "TinkoJuice" } }, // gengirl_01_wall (16)
			{ "HF_AssWall_Man_FemaleLargeNative", new string[] { "TinkoJuice" } }, // gengirl_01_wall (17)
			{ "HF_AssWall_Man_UnderGroundWoman",  new string[] { "TinkoJuice" } }, // gengirl_01_wall (44)
			{ "HF_AssWall_Man_ElderSisterNative", new string[] { "TinkoJuice" } }, // gengirl_01_wall (90)

			// CommonSexNpc
			{ "HF_MaleNative_FemaleNative_Friendly_Doggy_Normal", new string[] { "Juice" } }, //  gen_01_gg01_sex_01
			// { "HF_FemaleNative_FemaleNative_Friendly_Normal", new string[] { } }, // 
			// { "HF_YoungMan_FemaleNative_Friendly_Normal", new string[] { } }, // 
			// { "HF_YoungMan_FemaleNative_Friendly_Pregnant", new string[] { } }, // 
			{ "HF_MaleNative_NativeGirl_Friendly_Normal", new string[] { "Juice" } }, // gen_01_gg02_sex_01
			// { "HF_NativeGirl_NativeGirl_Friendly_Normal", new string[] { } }, // 
			{ "HF_YoungMan_NativeGirl_Friendly_Normal", new string[] { "Juice" } }, // 
			{ "HF_YoungMan_FemaleLargeNative_Friendly_Normal", new string[] { "Juice" } }, // 
			// { "HF_YoungMan_UnderGroundWoman_Friendly_Normal", new string[] { } }, // 
			{ "HF_YoungMan_ElderSisterNative_Friendly_Normal", new string[] { "Juice" } }, // 

			// CommonSexPlayer
			{ "HF_Man_Reika_Friendly_Cowgirl",                    new string[] { "Juice" } }, // man_01_sex_reika_02
			{ "HF_Man_Reika_Friendly_RevCowgirl",                 new string[] { "Juice" } }, // man_01_sex_reika_01
			{ "HF_Man_FemaleNative_Friendly_Normal",              new string[] { "juice01" } }, // man_01_sex_01
			{ "HF_Man_FemaleNative_Friendly_Pregnant",            new string[] { "Juice" } }, // man_01_sex_01_preg_01
			{ "HF_Man_NativeGirl_Friendly_Normal",                new string[] { "Juice" } }, // man_01_sex_02 or man_01_sex_02_0
			{ "HF_Man_FemaleLargeNative_Friendly_Cowgirl_Normal", new string[] { "Juice" } }, // man_01_rapes_03
			{ "HF_Man_FemaleLargeNative_Friendly_Doggy_Normal",   new string[] { "Juice" } }, // man_01_rapes_03
			// { "HF_Man_Mummy_Friendly_Normal", new string[] { } }, // man_01_sex_mummy_01
			// { "HF_Man_UnderGroundWoman_Friendly_Normal", new string[] { } },
			// { "HF_Man_Mermaid_Friendly_TittyFuck", new string[] { } }, // man_01_sex_mermaid_01
			{ "HF_Man_Mermaid_Friendly_Fuck",                   new string[] { "Juice" } }, // man_01_sex_mermaid_01
			{ "HF_Man_ElderSisterNative_Friendly_Doggy_Normal", new string[] { "Juice" } }, // man_01_sex_bba_02
			{ "HF_Man_Giant_Friendly_Normal",                   new string[] { "Juice" } }, // man_01_sex_gengiant_01
			{ "HF_Man_Cassie2_Friendly_Normal",                 new string[] { "Juice" } }, // man_01_sex_cassie_01
			// { "HF_Man_Shino_Friendly_TittyFuck", new string[] { } }, // man_01_sex_shino_01 -- No penetration
			// This simply doesn't work... It looks like the juice here is set in a different way and we can't color it...
			// Players will probably just get her through event anyway
			// { "HF_Man_Shino_Friendly_Fuck", new string[] { "Juice" } }, // man_01_sex_shino_01
			{ "HF_Man_Sally_Friendly_Normal", new string[] { "Juice2" } }, // boss_prison_01_rapes_01
			{ "HF_Man_Merry_Friendly_Normal", new string[] { "Juice_00" /* Maybe Juice_01 */ } }, // man_01_sex_santa_01
			{ "HF_Yona_MaleNative_Friendly_Normal", new string[] { "Juice" } }, // gen_01_rapes_yona_01 (?)
			{ "HF_Yona_BigNative_Friendly_Normal", new string[] { "Juice" } }, // gen_02_sex_yona_01
			{ "HF_Yona_SmallNative_Friendly_Normal", new string[] { "Juice" } }, // gen_03_rapes_yona_01 (?)
			{ "HF_Yona_ElderBrotherNative_Friendly_Normal", new string[] { "Juice" } }, // gengg_02_sex_yona_01

			// Daruma
			{ "HF_Man_FemaleNative_Daruma", new string[] { "Juice" } }, // man_01_rapes_daruma_01
			{ "HF_Man_NativeGirl_Daruma", new string[] { "Juice" } }, // man_01_rapes_daruma_gg_02 (?)

			// ManRapes
			{ "HF_Man_Yona_Rape",                          new string[] { "Juice" } }, // man_01_rapes_yona_01
			{ "HF_Man_FemaleNative_Rape_Fainted",          new string[] { "Juice" } }, // man_01_rapes_01_faint
			{ "HF_Man_FemaleNative_Rape_Grapple",          new string[] { "Juice_Tinko" } }, // man_01_rapes_01
			{ "HF_Man_FemaleNative_Rape_Grapple_Pregnant", new string[] { "Juice", "Juice2" } }, // man_01_rapes_01_preg
			{ "HF_Man_NativeGirl_Rape_Fainted",            new string[] { "Juice" } }, // man_01_rapes_02_faint
			{ "HF_Man_NativeGirl_Rape_Grapple",            new string[] { "Juice" } }, // man_01_rapes_02
			{ "HF_Man_FemaleLargeNative_Rape",             new string[] { "Juice3" } }, // man_01_rapes_03
			{ "HF_Man_OldWomanNative_Rape",                new string[] { "Juice_Tinko" } }, // man_01_rapes_genbba_01
			// { "HF_Man_Mummy_Rape", new string[] { } }, // 
			// Looks like it doesn't work because the animation for insert "rolls down" juice turning to white.
			{ "HF_Man_UnderGroundWoman_Rape", new string[] { "Juice1" } }, // man_01_rapes_genunder_01
			{ "HF_Man_ElderSisterNative_Rape", new string[] { "Juice" } }, // man_01_rapes_genbba_02
			// { "HF_Man_Shino_Rape", new string[] { } }, // 

			// ManRapesSleep
			{ "HF_Man_FemaleNative_SleepRape_Normal", new string[] { "TinkoJuice" } }, // man_01_rapes_sleep_01
			{ "HF_Man_NativeGirl_SleepRape_Normal",   new string[] { "TinkoJuice" } }, // man_01_rapes_sleep_02

			// PlayerRaped
			{ "HF_MaleNative_Yona_Rape_Doggy_Normal", new string[] { "Juice" } }, // gen_01_rapes_yona_01
			{ "HF_BigNative_Yona_Rape", new string[] { "TinkoJuice" } }, // gen_02_rapes_01
			{ "HF_Bigfoot_Yona_Rape", new string[] { "TinkoJuice" } }, // bigfoot_01_0
			{ "HF_Werewolf_Yona_Rape", new string[] { "Juice" } }, // werewolf_01_rapes_yona_01
			{ "HF_Oldguy_Yona_Rape", new string[] { "TinkoJuice" } }, // boss_01_rapes_01
			{ "HF_Spike_Yona_Rape", new string[] { "Juice" } }, // boss_spider_01_rapes
			{ "HF_Planton_Yona_Rape", new string[] { "TinkoJuice" } }, // boss_plant_01_rapes
			// { "HF_BossNative_Yona_Rape", new string[] { "" } },  // Nothing
			// { "HF_FemaleLargeNative_Man_Rape_Battle", new string[] { } }, // gengirl_03_attack_man_01
			// { "HF_FemaleLargeNative_Man_Rape_Sex", new string[] { "Juice" } }, // gengirl_03_rapes_man_01 -- Anal
			// { "HF_Mummy_Man_Rape_Battle_Sex", new string[] { } },
			// @TODO: Event only
			// boyfriend_02_0
			// boyfriend_01_rapes_reika_01
			// boyfriend_01_rapes_yona_01
			// boss_hunter_01_rapes
			// boss_daruman_01_rapes_01
			// boss_doctor_01_rapes

			// Slave
			{ "HF_Slave_Man_Giant", new string[] { "Juice" } }, // man_01_sex_gengiant_01
			{ "HF_Slave_Man_Shino", new string[] { } },
			{ "HF_Slave_Man_Sally", new string[] { } },
		};

		public void InitHooks()
		{
			HookBuilder.New("HExt.DickPainter.Main")
				.HookStepStart("Main")
				.Call(this.OnStart);

			HookBuilder.New("HExt.DickPainter.Penetrate")
				.HookEvent(EventNames.OnPenetrateVag)
				.Memorize(() => new VirginMemory(MemoryId))
				.Call(this.OnPenetrate);
		}

		private IEnumerator OnStart(IScene scene, object param)
		{
			var actors = scene.GetActors();
			bool hasComdom = false;

			foreach (var actor in actors)
			{
				if (actor.equip[7].itemKey == "acce_s_01")
				{
					hasComdom = true;
					break;
				}
			}

			if (hasComdom)
			{
				var dickSlot = scene.GetSkelAnimation()?.skeleton?.FindSlot("Tinko");
				if (dickSlot != null)
				{
					var dickColor = (Color32)dickSlot.GetColor();
					var condomColor = new Color32(209, 6, 119, 255);

					var finalColor = Utils.AlphaBlend(dickColor, condomColor, .6f);
					dickSlot.SetColor(finalColor);
				}
			}

			yield break;
		}

		private IEnumerator OnPenetrate(IScene scene, object param)
		{
			var memory = scene.GetHookMemory(MemoryId) as VirginMemory;
			if (memory == null || memory.IsVirgin == false)
				yield break;
		
			var performerId = scene.GetPerformer()?.Info?.Id ?? "";
			if (performerId == "")
				yield break;

			if (!this.VirginJuices.ContainsKey(performerId))
				yield break;

			foreach (var juice in this.VirginJuices[performerId])
			{
				var slot = scene.GetSkelAnimation()?.skeleton?.FindSlot(juice) ?? null;
				if (slot == null)
					continue;

				var baseColor = slot.GetColor();
				slot.SetColor(Utils.AlphaBlend(baseColor, new Color32(255, 0, 0, 255), 1f));
			}
		}
	}
}
