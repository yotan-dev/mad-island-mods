using System;
using System.Collections;
using System.Collections.Generic;
using HFramework.Scenes;
using YotanModCore.Events;

namespace HFramework.Hook
{
	/// <summary>
	/// Manages hooks.
	/// 
	/// Hook targets:
	/// <Type>::<SceneName>::<Specifier>
	/// 
	/// Execution order:
	/// - Hooks that have all 3 values set (e.g.: StepStart::CommonSexPlayer::Main)
	/// - Hooks that have SceneName set and Specifier as wildcard (*) (e.g. StepStart::CommonsexPlayer::*)
	/// - Hooks that have SceneName as wildcard and Specifier set (e.g. Event::*::OnPenetrate)
	/// - Hooks that have SceneName and Specifier as wildcards (e.g. Event::*::*)
	/// 
	/// Hooks are queued and executed in the order they are added, unless AddBefore/AddAfter is used, in this case,
	/// those hooks are put at that position, but if several hooks adds before the same key, the addition order is kept.
	/// 
	/// Type:
	/// - StepStart: Before a 'Step' (Battle, Fuck, Speed, Bust, etc) in the scene runs
	/// 	- SceneName: The name of the scene the step is in. Use * to mean any scene.
	/// 	- Specifier: The name of the step to trigger. Use * to mean any step.
	/// 		Scenes always have the 'Main' step, triggered at the start/end of the entire scene.
	/// - StepEnd: After a 'Step' in the animation runs
	/// 	- SceneName: The name of the scene the step is in. Use * to mean any scene.
	/// 	- Specifier: The name of the step to trigger. Use * to mean any step.
	/// 		Scenes always have the 'Main' step, triggered at the start/end of the entire scene.
	/// - Event: When a sex "event" (Penetration, Orgasm, Creampie, etc) happens
	/// 	- SceneName: The name of the scene the event is in. Use * to mean any scene.
	/// 	- Specifier: The name of the event to trigger. Use * to mean any event.
	/// </summary>
	public class HookManager
	{
		public delegate void RegisterHooks();

		public static event RegisterHooks RegisterHooksEvent;

		public static HookManager Instance { get; set; } = new HookManager();

		private Dictionary<string, HookQueue> HookDict = new Dictionary<string, HookQueue>();

		private Dictionary<string, List<Func<HookMemory>>> MemorizerDict = [];

		private HookManager()
		{
			GameLifecycleEvents.OnGameStartEvent += () =>
			{
				this.HookDict.Clear();
				this.MemorizerDict.Clear();
				HookManager.RegisterHooksEvent?.Invoke();
			};
			GameLifecycleEvents.OnGameEndEvent += () => {
				this.HookDict.Clear();
				this.MemorizerDict.Clear();
			};
		}

		public void AddMemorizer(string target, Func<HookMemory> memorizer)
		{
			if (!this.MemorizerDict.TryGetValue(target, out var list))
			{
				list = new List<Func<HookMemory>>();
				this.MemorizerDict.Add(target, list);
			}

			list.Add(memorizer);
		}

		public void AddHook(string target, string uid, Func<IScene, object, IEnumerator> handler)
		{
			Hook hook = new Hook(uid, target, handler);
			HookQueue queue;

			if (!this.HookDict.TryGetValue(target, out queue))
			{
				queue = new HookQueue();
				this.HookDict.Add(target, queue);
			}

			queue.Add(hook);
		}

		public void AddHookBefore(string target, string beforeUid, string uid, Func<IScene, object, IEnumerator> handler)
		{
			Hook hook = new Hook(uid, target, handler);
			HookQueue queue;

			if (!this.HookDict.TryGetValue(target, out queue))
			{
				queue = new HookQueue();
				this.HookDict.Add(target, queue);
			}

			queue.AddBefore(beforeUid, hook);
		}

		public void AddHookAfter(string target, string afterUid, string uid, Func<IScene, object, IEnumerator> handler)
		{
			Hook hook = new Hook(uid, target, handler);
			HookQueue queue;

			if (!this.HookDict.TryGetValue(target, out queue))
			{
				queue = new HookQueue();
				this.HookDict.Add(target, queue);
			}

			queue.AddAfter(afterUid, hook);
		}

		public void RemoveHook(string uid)
		{
			foreach (var queue in this.HookDict.Values)
				queue.Remove(uid);
		}

		private IEnumerator SaveHookMemory(IScene scene, string target, object param)
		{
			PLogger.LogDebug($"SetHookMemory: Running target - {target}");
			if (!this.MemorizerDict.TryGetValue(target, out var list))
				yield break;

			foreach (var memorizer in list)
			{
				var memoryInstance = memorizer();
				var existing = scene.GetHookMemory(memoryInstance.UID);

				if (existing != null)
					memoryInstance = existing;

				PLogger.LogDebug($"SetHookMemory: Running UID - {memoryInstance.UID}");
				memoryInstance.Save(scene, param);

				if (existing == null)
					scene.AddHookMemory(memoryInstance);
			}
		}

		private IEnumerator RunHooks(IScene scene, string target, object param)
		{
			PLogger.LogDebug($"RunHooks: Running target - {target}");
			if (!this.HookDict.TryGetValue(target, out var queue))
				yield break;

			foreach (var hook in queue.GetEnumerable())
			{
				PLogger.LogDebug($"RunHooks: Running UID - {hook.UID}");
				yield return hook.Handler(scene, param);
			}
		}

		public IEnumerator RunStepStartHook(IScene scene, string stepName)
		{
			var prefix = "StepStart";
			var sceneName = scene.GetName();

			yield return this.SaveHookMemory(scene, $"{prefix}::{sceneName}::{stepName}", null);
			yield return this.SaveHookMemory(scene, $"{prefix}::{sceneName}::*", null);
			yield return this.SaveHookMemory(scene, $"{prefix}::*::{stepName}", null);
			yield return this.SaveHookMemory(scene, $"{prefix}::*::*", null);

			yield return this.RunHooks(scene, $"{prefix}::{sceneName}::{stepName}", null);
			yield return this.RunHooks(scene, $"{prefix}::{sceneName}::*", null);
			yield return this.RunHooks(scene, $"{prefix}::*::{stepName}", null);
			yield return this.RunHooks(scene, $"{prefix}::*::*", null);
		}

		public IEnumerator RunStepEndHook(IScene scene, string stepName)
		{
			var prefix = "StepEnd";
			var sceneName = scene.GetName();

			yield return this.SaveHookMemory(scene, $"{prefix}::{sceneName}::{stepName}", null);
			yield return this.SaveHookMemory(scene, $"{prefix}::{sceneName}::*", null);
			yield return this.SaveHookMemory(scene, $"{prefix}::*::{stepName}", null);
			yield return this.SaveHookMemory(scene, $"{prefix}::*::*", null);

			yield return this.RunHooks(scene, $"{prefix}::{sceneName}::{stepName}", null);
			yield return this.RunHooks(scene, $"{prefix}::{sceneName}::*", null);
			yield return this.RunHooks(scene, $"{prefix}::*::{stepName}", null);
			yield return this.RunHooks(scene, $"{prefix}::*::*", null);
		}

		public IEnumerator RunEventHook(IScene scene, string eventName, object param)
		{
			var prefix = "Event";
			var sceneName = scene.GetName();

			yield return this.SaveHookMemory(scene, $"{prefix}::{sceneName}::{eventName}", param);
			yield return this.SaveHookMemory(scene, $"{prefix}::{sceneName}::*", param);
			yield return this.SaveHookMemory(scene, $"{prefix}::*::{eventName}", param);
			yield return this.SaveHookMemory(scene, $"{prefix}::*::*", param);

			yield return this.RunHooks(scene, $"{prefix}::{sceneName}::{eventName}", param);
			yield return this.RunHooks(scene, $"{prefix}::{sceneName}::*", param);
			yield return this.RunHooks(scene, $"{prefix}::*::{eventName}", param);
			yield return this.RunHooks(scene, $"{prefix}::*::*", param);
		}
	}
}
