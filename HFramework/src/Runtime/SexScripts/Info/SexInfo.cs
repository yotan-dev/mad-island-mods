using HFramework.SexScripts.ScriptContext;
using UnityEngine;


namespace HFramework.SexScripts.Info
{
	[Experimental]
	public class SexInfo
	{
		/// <summary>
		/// <para>
		/// Allows adjustment of duration of animation times (as long as the animation allows it)
		/// A scale of 1f means normal speed, 2f means double the duration, 0.5f means half the duration, etc.
		/// </para>
		/// <para>
		/// Animations must explicitly support it by setting "AllowDurationModifier" to true in their configuration.
		/// </para>
		/// </summary>
		/// <value></value>
		public float AnimDurationModifier { get; set; } = 1f;
	}

	public interface IHasScriptPlace
	{
		ScriptPlace Place { get; }
	}

	public interface IHasSexPos
	{
		Vector3 Pos { get; }
	}

	[Experimental]
	public class CommonSexInfo : SexInfo, IHasScriptPlace
	{
		public ScriptPlace Place { get; set; }
		public Vector3 Pos { get; set; }
	}

	public interface IHasSexType
	{
		int SexType { get; }
	}

	[Experimental]
	public class PlayerSexInfo : SexInfo, IHasSexType, IHasSexPos
	{
		public int SexType { get; set; }
		public Vector3 Pos { get; set; }
	}
}
