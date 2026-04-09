using HFramework.Scenes;

namespace HFramework.Hook
{
	public abstract class HookMemory
	{
		public string UID { get; set; }

		public HookMemory(string uid)
		{
			this.UID = uid;
		}

		public abstract void Save(IScene scene, object param);
	}
}
