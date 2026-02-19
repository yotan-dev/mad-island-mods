using System;
using HFramework.Tree;
using UnityEngine;

namespace HFramework
{
	// Base non-generic interface for type erasure
	public interface ISexEventBase
	{
		string GetId();
		void TriggerWithBaseArgs(BaseSexEventArgs args);
	}

	// Covariant interface for read-only access to events
	public interface IReadOnlySexEvent<out T> : ISexEventBase where T : BaseSexEventArgs
	{
		// Can be used in covariant scenarios
	}

	// Contravariant interface for write/trigger access to events
	public interface ITriggerableSexEvent<in T> where T : BaseSexEventArgs
	{
		void Trigger(T args);
	}

	// Combined interface for concrete event implementations
	public interface ISexEvent<T> : IReadOnlySexEvent<T>, ITriggerableSexEvent<T> where T : BaseSexEventArgs, new()
	{
		// Combines both covariant and contravariant interfaces
	}

	[Serializable]
	public abstract class BaseSexEventArgs : EventArgs
	{
		public abstract void Populate(CommonContext ctx, EmitEventNode node);
	}

	[Serializable]
	public class FromToEventArgs : BaseSexEventArgs
	{
		public int fromNpcIdx;
		public int toNpcIdx;

		[HideInInspector] public CommonStates From;
		[HideInInspector] public CommonStates To;

		public override void Populate(CommonContext ctx, EmitEventNode node)
		{
			From = ctx.Actors[fromNpcIdx].Common;
			To = ctx.Actors[toNpcIdx].Common;
		}
	}

	[Serializable]
	public class SelfEventArgs : BaseSexEventArgs
	{
		public int fromNpcIdx;
		[HideInInspector] public CommonStates Self;

		public override void Populate(CommonContext ctx, EmitEventNode node)
		{
			Self = ctx.Actors[fromNpcIdx].Common;
		}
	}

	public class SexEvent<T> : ISexEvent<T> where T : BaseSexEventArgs, new()
	{
		public readonly string id;

		public event EventHandler<T> Triggered;

		public SexEvent(string id)
		{
			this.id = id;
		}

		public void Trigger(T args)
		{
			// Implementation here
			PLogger.LogError($"Event triggered: {id}");
			Triggered?.Invoke(this, args);
		}

		void ISexEventBase.TriggerWithBaseArgs(BaseSexEventArgs args)
		{
			if (args is T typedArgs)
			{
				Trigger(typedArgs);
			}
			else
			{
				throw new ArgumentException($"Expected argument of type {typeof(T).Name}, but got {args.GetType().Name}");
			}
		}

		public string GetId()
		{
			return id;
		}
	}
}
