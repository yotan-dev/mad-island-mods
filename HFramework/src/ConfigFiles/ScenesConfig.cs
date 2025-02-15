using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HFramework.ConfigFiles
{
	[Serializable]
	public class ScenesConfig
	{
		[XmlArrayItem("Scene")]
		public List<SceneConfig> Scenes { get; set; }

		public ScenesConfig()
		{
			this.Scenes = new List<SceneConfig>();
		}
	}
}
