using UnityEngine;

namespace HFramework.SexScripts.ScriptContext
{
	public abstract class ScriptPlace
	{
		public abstract bool IsGround();
		public abstract bool IsWorkplace();
		public abstract bool IsSexPlace();
		public abstract bool IsInUse();
		public abstract bool HasObject();
		public abstract GameObject GetObject();
		public abstract void SetUser(GameObject user);
		public abstract void ClearUser();
		public abstract Vector3 GetCharacterPosition();
	}
}
