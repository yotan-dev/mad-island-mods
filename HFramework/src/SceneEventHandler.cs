using System.Collections;
using HFramework.Scenes;

namespace HFramework
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

		/// <summary>
		/// Called when "from" character uses "to" in toilet
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public virtual IEnumerable OnToilet(CommonStates from, CommonStates to)
		{
			yield return null;
		}

		/// <summary>
		/// Called when "from" rapes "to"
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public virtual IEnumerable OnRape(CommonStates from, CommonStates to)
		{
			yield return null;
		}

		/// <summary>
		/// Called when "from" rapes "to"
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public virtual IEnumerable OnRape(IScene scene, CommonStates from, CommonStates to)
		{
			yield return null;
		}

		public virtual IEnumerable OnSleepRapeTypeChange(IScene scene, ManRapeSleepState type)
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

		public virtual IEnumerable OnDelivery(Delivery scene, CommonStates mother)
		{
			yield return null;
		}

		public virtual IEnumerable AfterRape(CommonStates victim, CommonStates rapist)
		{
			yield return null;
		}

		public virtual IEnumerable AfterManRape(CommonStates victim, CommonStates rapist)
		{
			yield return null;
		}

		public virtual IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			yield return null;
		}

		public virtual IEnumerable AfterDelivery(IScene scene, CommonStates mother)
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
