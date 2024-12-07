using System.Collections;
using ExtendedHSystem.Scenes;

namespace ExtendedHSystem
{
	public abstract class SceneEventHandler
	{
		public readonly string Name;

		public SceneEventHandler(string name)
		{
			this.Name = name;
		}

		public virtual IEnumerable PlayerDefeated()
		{
			yield return null;
		}

		public virtual IEnumerable PlayerRaped(CommonStates player, CommonStates rapist, bool silent)
		{
			yield return null;
		}

		public virtual IEnumerable OnNormalSex(CommonStates a, CommonStates b)
		{
			yield return null;
		}

		public virtual IEnumerable OnBusted(CommonStates from, CommonStates to, int specialFlag)
		{
			yield return null;
		}

		public virtual IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			yield return null;
		}

		public virtual IEnumerable AfterRape(CommonStates victim, CommonStates rapist)
		{
			yield return null;
		}

		public virtual IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			yield return null;
		}

		public virtual IEnumerable BeforeRespawn()
		{
			yield return null;
		}

		public virtual IEnumerable Respawn(CommonStates player, CommonStates other)
		{
			yield return null;
		}
	}
}