using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	public class GroundScriptPlace : ScriptPlace
	{
		private Vector3? CharacterPosition { get; set; } = null;

		public GroundScriptPlace(Vector3? characterPosition = null) {
			this.CharacterPosition = characterPosition;
		}

		public override bool IsGround() {
			return true;
		}

		public override bool IsWorkplace() {
			return false;
		}

		public override bool IsSexPlace() {
			return false;
		}

		public override bool IsInUse() {
			return false;
		}

		public override bool IsUser(GameObject user) {
			return false;
		}

		public override bool HasObject() {
			return false;
		}

		public override GameObject GetObject() {
			return null;
		}

		public override void SetUser(GameObject user) {
			// Ground places don't have users
		}

		public override void ClearUser() {
			// Ground places don't have users
		}

		public override Vector3 GetCharacterPosition() {
			return this.CharacterPosition ?? Vector3.zero;
		}
	}
}
