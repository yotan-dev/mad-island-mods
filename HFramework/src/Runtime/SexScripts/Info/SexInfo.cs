using UnityEngine;


namespace HFramework.SexScripts.Info
{
	[Experimental]
	public class SexInfo
	{

	}

	public interface IHasSexPlace
	{
		SexPlace Place { get; }
	}

	public interface IHasSexPos
	{
		Vector3 Pos { get; }
	}

	[Experimental]
	public class CommonSexInfo : SexInfo, IHasSexPlace
	{
		public SexPlace Place { get; set; }
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
