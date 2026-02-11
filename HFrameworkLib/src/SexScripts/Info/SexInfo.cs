namespace HFramework.SexScripts.Info
{
	public abstract class SexInfo
	{

	}

	public interface IHasSexPlace
	{
		SexPlace Place { get; }
	}

	public class CommonSexInfo : SexInfo, IHasSexPlace
	{
		public SexPlace Place { get; set; }
	}

	public interface IHasSexType
	{
		int SexType { get; }
	}

	public class PlayerSexInfo : SexInfo, IHasSexType
	{
		public int SexType { get; set; }
	}
}
