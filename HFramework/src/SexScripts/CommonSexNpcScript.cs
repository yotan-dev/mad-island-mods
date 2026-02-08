#nullable enable

using HFramework.SexScripts.CommonSexNpcStates;
using HFramework.SexScripts.PrefabCreators;
using Spine.Unity;
using UnityEngine;

namespace HFramework.SexScripts
{
	[Experimental]
	public class CommonSexNpcScript
	{
		[Experimental]
		public class StateMachine
		{
			public BaseState StartState { get; private set; }

			public BaseState MoveToPlace { get; private set; }

			public BaseState Setup { get; private set; }

			public BaseState DisableLivingCharacters { get; private set; }

			public BaseState SexStart { get; private set; }

			public BaseState SexSpeed1 { get; private set; }
			public BaseState SexSpeed2 { get; private set; }
			public BaseState SexFinish { get; private set; }
			public BaseState SexFinishIdle { get; private set; }

			public BaseState Teardown { get; private set; }

			public BaseState? CurrentState { get; set; }

			public StateMachine()
			{
				this.Teardown = new CommonSexNpcStates.Teardown();

				this.StartState = new CommonSexNpcStates.Start();
				this.SexStart = new CommonSexNpcStates.SexStart();
				this.DisableLivingCharacters = new CommonSexNpcStates.DisableLivingCharacters(this.SexStart);
				this.Setup = new CommonSexNpcStates.Setup(new SexListPrefab(2, 0), this.DisableLivingCharacters);
				this.MoveToPlace = new CommonSexNpcStates.MoveToPlace(this.Setup);

				this.SexFinishIdle = new CommonSexNpcStates.SexFinishIdle(this.Teardown);
				this.SexFinish = new CommonSexNpcStates.SexFinish(this.SexFinishIdle);
				this.SexSpeed2 = new CommonSexNpcStates.SexSpeed2(this.SexFinish);
				this.SexSpeed1 = new CommonSexNpcStates.SexSpeed1(this.SexSpeed2);
			}
		}

		public bool IsCompleted { get; private set; } = false;

		public bool ShouldStop { get; set; } = false;

		/// <summary>
		/// First NPC in the sex scene.
		/// If a Male x Female relation, this is the Male.
		/// </summary>
		public CommonStates? NpcA { get; set; }

		/// <summary>
		/// Second NPC in the sex scene.
		/// If a Male x Female relation, this is the Female.
		/// </summary>
		public CommonStates? NpcB { get; set; }

		public SexPlace? SexPlace { get; set; }

		public Vector3? SexPlacePos { get; set; }

		public float? NpcAAngle { get; set; } = null;

		public float? NpcBAngle { get; set; } = null;

		public GameObject? TmpSex { get; set; }

		public SkeletonAnimation? TmpSexAnim { get; set; }

		public StateMachine States { get; set; }

		public CommonSexNpcScript()
		{
			this.States = new StateMachine();
		}

		public void Init(CommonStates npcA, CommonStates npcB, SexPlace sexPlace)
		{
			this.NpcA = npcA;
			this.NpcB = npcB;
			this.SexPlace = sexPlace;
			this.SexPlacePos = sexPlace.transform.Find("pos")?.position;
			if (this.SexPlacePos == null)
			{
				PLogger.LogError("Sex place position not found");
				this.ShouldStop = true;
			}
		}

		public void Start()
		{
			this.ChangeState(this.States.StartState);
		}

		public void Update()
		{
			PLogger.LogDebug("CommonSexNpcScript.Update");
			if (this.NpcA?.dead != 0 || this.NpcB?.dead != 0)
				this.ShouldStop = true;

			if (this.ShouldStop)
			{
				this.ChangeState(null);
				return;
			}

			PLogger.LogDebug($"Update: {this.States.CurrentState?.GetType().Name}");
			this.States.CurrentState?.Update(this);
		}

		public void ChangeState(BaseState? state)
		{
			PLogger.LogDebug($"ChangeState: {this.States.CurrentState?.GetType().Name} -> {state?.GetType().Name}");

			this.States.CurrentState?.OnExit(this);

			if (state == null || state is Teardown) {
				if (this.States.CurrentState is Teardown)
				{
					PLogger.LogDebug("Current state is already Teardown");
					this.IsCompleted = true;
					return;
				}

				PLogger.LogDebug("Setting state to Teardown");
				state = this.States.Teardown;
			}

			this.States.CurrentState = state;

			PLogger.LogDebug($"OnEnter: {this.States.CurrentState?.GetType().Name}");
			this.States.CurrentState?.OnEnter(this);
		}
	}
}
