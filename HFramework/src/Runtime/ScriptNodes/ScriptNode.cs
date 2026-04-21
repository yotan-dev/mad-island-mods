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

		internal CommonContext Context;

		[HideInInspector] public State ScriptState = State.Running;
		[HideInInspector] public bool Started = false;

		[HideInInspector] public string GUID;
		[HideInInspector] public Vector2 Position;

		public State Update() {
			if (!Started) {
				PLogger.LogDebug($"Node {this.name} started");
				OnStart();
				Started = true;
			}

			PLogger.LogDebug($"Node {this.name} updating");
			ScriptState = OnUpdate();

			if (ScriptState == State.Failure || ScriptState == State.Success) {
				this.Terminate(false);
			}

			return ScriptState;
		}

		public virtual void Terminate(bool fromOutside = true) {
			PLogger.LogDebug($"Node {this.name} stopped ({(fromOutside ? "from outside" : "naturally")})");
			this.OnStop();
			this.Started = false;
		}

		public virtual ScriptNode Clone(CommonContext context) {
			var node = Instantiate(this);
			node.Context = context;
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
