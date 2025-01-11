using System;
using System.Collections;
using HFramework.Scenes;

namespace HFramework.Hook
{
	public class HookBuilder
	{
		enum HookType { None, StepStart, StepEnd, Event };

		private string UID;

		private HookType Type = HookType.None;

		private string[] SceneNames = { "*" };

		private string Target = "";

		private string BeforeUID = "";

		private string AfterUID = "";

		private HookBuilder(string uid)
		{
			this.UID = uid;
		}

		public static HookBuilder New(string uid)
		{
			return new HookBuilder(uid);
		}

		public HookBuilder ForScenes(params string[] sceneNames)
		{
			this.SceneNames = sceneNames;
			return this;
		}

		public HookBuilder HookEvent(string eventName)
		{
			if (this.Type != HookType.None)
				throw new System.Exception("A hook can only have one type");

			this.Type = HookType.Event;
			this.Target = eventName;
			return this;
		}

		public HookBuilder HookStepStart(string stepName)
		{
			if (this.Type != HookType.None)
				throw new System.Exception("A hook can only have one type");

			this.Type = HookType.StepStart;
			this.Target = stepName;
			return this;
		}

		public HookBuilder HookStepEnd(string stepName)
		{
			if (this.Type != HookType.None)
				throw new System.Exception("A hook can only have one type");

			this.Type = HookType.StepEnd;
			this.Target = stepName;
			return this;
		}

		public void Call(Func<IScene, object, IEnumerator> handler)
		{
			string hookPrefix;
			switch (this.Type)
			{
				case HookType.StepStart:
					hookPrefix = "StepStart";
					break;
				case HookType.StepEnd:
					hookPrefix = "StepEnd";
					break;
				case HookType.Event:
					hookPrefix = "Event";
					break;
				default:
					throw new System.Exception("Invalid hook type");
			}

			foreach (var sceneName in this.SceneNames)
			{
				var fullTarget = $"{hookPrefix}::{sceneName}::{this.Target}";
				if (this.BeforeUID != "")
					HookManager.Instance.AddHookBefore(fullTarget, this.BeforeUID, this.UID, handler);
				else if (this.AfterUID != "")
					HookManager.Instance.AddHookAfter(fullTarget, this.AfterUID, this.UID, handler);
				else
					HookManager.Instance.AddHook(fullTarget, this.UID, handler);
			}
		}

		public void CallBefore(string beforeUID, Func<IScene, object, IEnumerator> handler)
		{
			this.BeforeUID = beforeUID;
			this.Call(handler);
		}

		public void CallAfter(string afterUID, Func<IScene, object, IEnumerator> handler)
		{
			this.BeforeUID = afterUID;
			this.Call(handler);
		}
	}
}
