using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	public class WorkplaceScriptPlace : ScriptPlace
	{
		public WorkPlace Place;

		private Vector3 CharacterPosition;

		private bool WasUserSet = false;

		public WorkplaceScriptPlace(WorkPlace place) {
			this.Place = place;
			this.CharacterPosition = place.transform.Find("pos").position;
		}

		public override bool IsGround() {
			return false;
		}

		public override bool IsWorkplace() {
			return true;
		}

		public override bool IsSexPlace() {
			return false;
		}

		public override bool IsInUse() {
			return this.Place.users[0] != null;
		}

		public override bool IsUser(GameObject user) {
			return this.Place.users[0] == user;
		}

		public override bool HasObject() {
			return false;
		}

		public override GameObject GetObject() {
			return null;
		}

		public override void SetUser(GameObject user) {
			this.WasUserSet = true;
			this.Place.users[0] = user;
		}

		public override void ClearUser() {
			if (!this.WasUserSet)
				return;

			this.WasUserSet = false;
			this.Place.users[0] = null;
		}

		public override Vector3 GetCharacterPosition() {
			return this.CharacterPosition;
		}
	}
}
