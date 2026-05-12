using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	public class SexPlaceScriptPlace : ScriptPlace
	{
		public SexPlace Place;

		private Vector3? CharacterPosition;

		private bool WasUserSet = false;

		public SexPlaceScriptPlace(SexPlace place) {
			this.Place = place;
			this.CharacterPosition = place.transform.Find("pos")?.position;
		}

		public override bool IsGround() {
			return false;
		}

		public override bool IsWorkplace() {
			return false;
		}

		public override bool IsSexPlace() {
			return true;
		}

		public override bool IsInUse() {
			return Place.user != null;
		}

		public override bool IsUser(GameObject user) {
			return Place.user == user;
		}

		public override bool HasObject() {
			return Place.gameObject != null;
		}

		public override GameObject GetObject() {
			return Place.gameObject;
		}

		public override void SetUser(GameObject user) {
			this.WasUserSet = true;
			Place.user = user;
		}

		public override void ClearUser() {
			if (!this.WasUserSet)
				return;

			this.WasUserSet = false;
			Place.user = null;
		}

		public override Vector3 GetCharacterPosition() {
			if (this.CharacterPosition == null) {
				return this.Place.transform.position;
			}

			return this.CharacterPosition.Value;
		}
	}
}
