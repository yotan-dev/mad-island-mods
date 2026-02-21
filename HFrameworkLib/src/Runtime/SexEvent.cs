using System;
using HFramework.Tree;
using UnityEngine;

namespace HFramework
{
	// Base non-generic interface for type erasure
	public interface ISexEventBase
	{
		string GetId();
		void TriggerWithBaseArgs(SexEventArgs args);
	}

	// Covariant interface for read-only access to events
	public interface IReadOnlySexEvent<out T> : ISexEventBase where T : SexEventArgs
	{
		// Can be used in covariant scenarios
	}

	// Contravariant interface for write/trigger access to events
	public interface ITriggerableSexEvent<in T> where T : SexEventArgs
	{
		void Trigger(T args);
	}

	// Combined interface for concrete event implementations
	public interface ISexEvent<T> : IReadOnlySexEvent<T>, ITriggerableSexEvent<T> where T : SexEventArgs, new()
	{
		// Combines both covariant and contravariant interfaces
	}

	[Serializable]
	public class SexEventArgs : EventArgs
	{
		/// <summary>
		/// Optional comment to describe the event or provide additional context.
		/// Shown in the inspector for better documentation.
		///
		/// It is actually an workaround because for some reason EmitEventNode_Inspector can't render properly
		/// if this class does not have a property and an Event uses it directly. Dunno why, but a comment field is not bad, right?
		/// </summary>
		[TextArea(1, 3)]
		public string comment;

		[HideInInspector] public CommonContext ctx;

		public virtual void Populate(CommonContext ctx, EmitEventNode node)
		{
			this.ctx = ctx;
		}
	}

	[Serializable]
	public class FromToEventArgs : SexEventArgs
	{
		public int fromNpcIdx;
		public int toNpcIdx;

		public bool isRape;

		[HideInInspector] public CommonStates From;
		[HideInInspector] public CommonStates To;

		public override void Populate(CommonContext ctx, EmitEventNode node)
		{
			base.Populate(ctx, node);
			From = ctx.Actors[fromNpcIdx].Common;
			To = ctx.Actors[toNpcIdx].Common;
		}
	}

	[Serializable]
	public class SelfEventArgs : SexEventArgs
	{
		public int fromNpcIdx;
		[HideInInspector] public CommonStates Self;

		public override void Populate(CommonContext ctx, EmitEventNode node)
		{
			base.Populate(ctx, node);
			Self = ctx.Actors[fromNpcIdx].Common;
		}
	}

	public class SexEvent<T> : ISexEvent<T> where T : SexEventArgs, new()
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

		void ISexEventBase.TriggerWithBaseArgs(SexEventArgs args)
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
