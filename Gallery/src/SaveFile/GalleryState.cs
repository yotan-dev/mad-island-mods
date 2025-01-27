using System;
using System.IO;
using System.Xml.Serialization;
using Gallery.SaveFile.Containers;

namespace Gallery.SaveFile
{
	[Serializable]
	public class GalleryState
	{

		public static GalleryState Instance = new GalleryState();

		private static bool IsLoading = false;

		private const string FileName = "GalleryState.v2.xml";

		public int Version = 2;

		public GalleryHashSet<AssWallInteractions> AssWall = [];

		public GalleryHashSet<CharacterInteraction> Daruma = [];

		public GalleryHashSet<CharacterInteraction> Slave = [];

		public GalleryHashSet<CharacterInteraction> PlayerRaped = [];

		public GalleryHashSet<ManRapesInteraction> ManRapes = [];

		public GalleryHashSet<NpcSexInteractions> CommonSexNpc = [];

		public GalleryHashSet<ManRapesInteraction> SleepRapes = [];

		public GalleryHashSet<CommonSexPlayerInteraction> CommonSexPlayer = [];
	
		public GalleryHashSet<ToiletInteractions> Toilet = [];
	
		public GalleryHashSet<ToiletNPCInteraction> ToiletNpc = [];

		public GalleryHashSet<SelfInteraction> Delivery = [];

		public GalleryHashSet<StoryInteraction> Story = [];
		
		public GalleryHashSet<OnaniInteraction> Onani = [];

		public static void Load()
		{
			if (!File.Exists(FileName)) {
				Save();
			}

			IsLoading = true;

			var serializer = new XmlSerializer(typeof(GalleryState));
			var fileStream = new FileStream(FileName, FileMode.Open);
			Instance = (GalleryState)serializer.Deserialize(fileStream);
			fileStream.Close();

			IsLoading = false;

			PLogger.LogInfo("Gallery state loaded");
		}

		public static void Save()
		{
			if (IsLoading) {
				return;
			}

			var serializer = new XmlSerializer(typeof(GalleryState));
			TextWriter writer = new StreamWriter(FileName);
			serializer.Serialize(writer, Instance);
			writer.Close();
		}
	}
}
