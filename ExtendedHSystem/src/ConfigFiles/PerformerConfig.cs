namespace ExtendedHSystem.ConfigFiles
{
	public class PerformerConfig
	{
		public string Id { get; set; }

		public PrefabConfig Prefab { get; set; }

		public string[] Actors { get; set; }

		public ConditionsConfig[] Conditions { get; set; }

		public AnimationsConfig[] Animations { get; set; }
	}
}