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

		public override bool HasObject() {
			return false;
		}

		public override GameObject GetObject() {
			PLogger.LogWarning("GroundScriptPlace.GetObject() called. This should not happen as Ground places are not objects! You are likely to have errors in your Sex Script.");
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
