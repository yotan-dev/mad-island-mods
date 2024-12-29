using System;
using System.Collections;
using HFramework.Scenes;

namespace HFramework.Hook
{
	public class Hook
	{
		public string UID { get; private set; }

		public string Target { get; private set; }

		public Func<IScene2, object, IEnumerator> Handler { get; private set; }

		public Hook(string uid)
		{
			this.UID = uid;
			this.Handler = this.DummyHandler;
		}

		private IEnumerator DummyHandler(IScene2 scene, object param)
		{
			yield break;
		}

		public Hook(string uid, string target, Func<IScene2, object, IEnumerator> handler)
		{
			this.UID = uid;
			this.Target = target;
			this.Handler = handler;
		}

		// override object.Equals
		public override bool Equals(object obj)
		{
			if (obj is not Hook hook)
				return false;

			return this.UID == hook.UID;
		}

		// override object.GetHashCode
		public override int GetHashCode()
		{
			return this.UID.GetHashCode();
		}
	}
}
