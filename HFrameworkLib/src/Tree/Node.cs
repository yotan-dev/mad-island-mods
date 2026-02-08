using UnityEngine;

namespace HFramework.Tree
{
	public abstract class Node : ScriptableObject
	{
		public enum State
		{
			Running,
			Failure,
			Success
		}

		[HideInInspector] public State state = State.Running;
		[HideInInspector] public bool started = false;

		[HideInInspector] public string GUID;
		[HideInInspector] public Vector2 position;

		public State Update()
		{
			if (!started)
			{
				OnStart();
				started = true;
			}

			state = OnUpdate();

			if (state == State.Failure || state == State.Success)
			{
				OnStop();
				started = false;
			}

			return state;
		}

		public virtual Node Clone()
		{
			return Instantiate(this);
		}

		protected abstract void OnStart();
		protected abstract void OnStop();
		protected abstract State OnUpdate();
	}
}
