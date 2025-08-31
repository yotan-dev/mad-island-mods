#nullable enable

using YotanModCore.Consts;

namespace YotanModCore.Extensions
{
	/// <summary>
	/// Used to pass damage information to TakeDamage.
	/// </summary>
	public class DamageInfo
	{
		public float Damage { get; set; }

		public CommonStates? Attacker { get; set; }

		public bool DestroyBody { get; set; } = false;

		public int HitSound { get; set; } = AudioSE.HitMeat01;
	}
}
