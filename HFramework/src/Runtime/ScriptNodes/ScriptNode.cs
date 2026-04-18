using UnityEngine;
#if UNITY_EDITOR
using HFramework.EditorUI.SexScripts;
#endif

namespace HFramework.ScriptNodes
{
	[Experimental]
	public abstract class ScriptNode : ScriptableObject
	{
		public enum State
		{
			Running,
			Failure,
			Success
		}

		/// <summary>
		/// Unique identifier for the node, used to reference from code.
		/// Should be unique within the script.
		/// </summary>
		[Tooltip("Unique identifier for the node, used to reference from code. Should be unique within the script.")]
		public string ID;

		internal CommonContext context;

		[HideInInspector] public State state = State.Running;
		[HideInInspector] public bool started = false;

		[HideInInspector] public string GUID;
		[HideInInspector] public Vector2 position;

		public State Update() {
			if (!started) {
				PLogger.LogDebug($"Node {this.name} started");
				OnStart();
				started = true;
			}

			PLogger.LogDebug($"Node {this.name} updating");
			state = OnUpdate();

			if (state == State.Failure || state == State.Success) {
				this.Terminate(false);
			}

			return state;
		}

		public virtual void Terminate(bool fromOutside = true) {
			PLogger.LogDebug($"Node {this.name} stopped ({(fromOutside ? "from outside" : "naturally")})");
			this.OnStop();
			this.started = false;
		}

		public virtual ScriptNode Clone(CommonContext context) {
			var node = Instantiate(this);
			node.context = context;
			return node;
		}

#if UNITY_EDITOR
		private void OnValidate() {
			NodeEvents.TriggerNodeChanged(this);
		}
#endif

		protected abstract void OnStart();
		protected abstract void OnStop();
		protected abstract State OnUpdate();
	}
}
