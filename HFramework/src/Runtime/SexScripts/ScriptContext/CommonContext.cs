#nullable enable

using System.Collections.Generic;
using HFramework.ScriptNodes;
using Spine.Unity;
using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	[Experimental]
	public class CommonContext
	{
		private ContextNpc[] _actors = new ContextNpc[0];
		public ContextNpc[] Actors {
			get { return this._actors; }
			set { this._actors = value; this.LoadActorsVariables(); }
		}

		public SexScripts.SexScript? SexScript { get; set; }

		public ISexScriptMenuSession? MenuSession { get; set; }

		public SexAction SexAction { get; set; } = SexAction.Idle;

		public string? PendingChoiceId { get; set; }

		public string? PendingChoiceAction { get; set; }

		public string? LastChoiceId { get; set; }

		public SexPlace? SexPlace { get; set; }

		public Vector3? SexPlacePos { get; set; }

		public GameObject? TmpSex { get; set; }

		public SkeletonAnimation? TmpSexAnim { get; set; }

		public Dictionary<string, string> Variables { get; set; } = new Dictionary<string, string>();

		/// <summary>
		/// Whether the main canvas visibility has been changed by a node.
		/// We use that to restore the canvas on Teardown
		/// Note: We only expect to have 1 script touching the canvas at any given time
		/// </summary>
		public bool HasChangedMainCanvasVisibility { get; set; }

		/// <summary>
		/// Whether the skip button visibility has been changed by a node.
		/// We use that to hide the skip button on Teardown
		/// Note: We only expect to have 1 script touching the skip button at any given time
		/// </summary>
		public bool HasChangedSkipButtonVisibility { get; set; }

		/// <summary>
		/// Whether the sex meter has been created by a node.
		/// We use that to hide the sex meter on Teardown.
		/// Note: We only expect to have 1 script using sex meter at any given time
		/// </summary>
		public bool HasSexMeter { get; set; }

		public int SexType = -1;

		public ScriptNode.State MainNodeState { get; set; }

		public CommonContext(SexScripts.SexScript sexScript) {
			this.SexScript = sexScript;
		}

		public virtual void LoadActorsVariables() {
			int idx = 0;
			foreach (var actor in this.Actors) {
				var missingLegs = actor.Common.dissect[4] == 1 && actor.Common.dissect[5] == 1;
				this.Variables[$"actors[{idx}].npcId"] = actor.Common.npcID.ToString();
				this.Variables[$"actors[{idx}].tits"] = actor.Common.parameters[6].ToString("00");
				this.Variables[$"actors[{idx}].disleg"] = missingLegs ? "DisLeg_" : "";
				idx++;
			}
		}
	}
}
