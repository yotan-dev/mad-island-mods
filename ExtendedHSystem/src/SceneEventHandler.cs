using System.Collections;

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

		public virtual IEnumerable AfterRape(CommonStates victim, CommonStates rapist)
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