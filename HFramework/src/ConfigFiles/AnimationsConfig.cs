namespace HFramework.ConfigFiles
{
	public class AnimationsConfig
	{
		public string Action { get; set; }
		public string Play { get; set; }
		public string Name { get; set; }
		public int? Pose { get; set; }
		public string[] Events { get; set; } = [];
		public bool? ChangePose { get; set; } = true;
	}
}
