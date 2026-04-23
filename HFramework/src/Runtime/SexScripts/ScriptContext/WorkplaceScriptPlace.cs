using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	public class WorkplaceScriptPlace : ScriptPlace
	{
		public WorkPlace Place;

		private Vector3 CharacterPosition;

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
			return false;
		}

		public override bool HasObject() {
			return false;
		}

		public override GameObject GetObject() {
			return null;
		}

		public override void SetUser(GameObject user) {
			// Workplace places don't have users
		}

		public override void ClearUser() {
			// Workplace places don't have users
		}

		public override Vector3 GetCharacterPosition() {
			return this.CharacterPosition;
		}
	}
}
