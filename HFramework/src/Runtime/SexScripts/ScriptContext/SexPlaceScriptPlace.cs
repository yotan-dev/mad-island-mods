using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	public class SexPlaceScriptPlace : ScriptPlace
	{
		public SexPlace Place;

		private Vector3 CharacterPosition;

		public SexPlaceScriptPlace(SexPlace place) {
			this.Place = place;
			this.CharacterPosition = place.transform.Find("pos").position;
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

		public override bool HasObject() {
			return Place.gameObject != null;
		}

		public override GameObject GetObject() {
			return Place.gameObject;
		}

		public override void SetUser(GameObject user) {
			Place.user = user;
		}

		public override void ClearUser() {
			Place.user = null;
		}

		public override Vector3 GetCharacterPosition() {
			return this.CharacterPosition;
		}
	}
}
