using System;
using System.IO;
using System.Xml.Serialization;
using HFramework.Scenes;
using Gallery.SaveFile.Containers;

namespace Gallery.SaveFile
{
	[Serializable]
	public class GalleryState
	{

		public static GalleryState Instance = new GalleryState();

		private static bool IsLoading = false;

		private const string FileName = "GalleryState.xml";

		public int Version = 1;

		public GalleryHashSet<AssWallInteractions> AssWall = new ();

		public GalleryHashSet<CharacterInteraction> Daruma = new ();

		public GalleryHashSet<CharacterInteraction> Slave = new ();

		public GalleryHashSet<CharacterInteraction> PlayerRaped = new();

		public GalleryHashSet<ManRapesInteraction> ManRapes = new();

		public GalleryHashSet<NpcSexInteractions> CommonSexNpc = new();

		public GalleryHashSet<SexTypeInteraction<ManRapeSleepState>> SleepRapes = new();

		public GalleryHashSet<CommonSexPlayerInteraction> CommonSexPlayer = new ();
	
		public GalleryHashSet<ToiletInteractions> Toilet = new ();
	
		public GalleryHashSet<ToiletNPCInteraction> ToiletNpc = new ();

		public GalleryHashSet<SelfInteraction> Delivery = new ();

		public GalleryHashSet<StoryInteraction> Story = new ();
		
		public GalleryHashSet<OnaniInteraction> Onani = new ();

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
