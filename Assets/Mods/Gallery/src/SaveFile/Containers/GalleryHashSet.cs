using System.Collections.Generic;

namespace Gallery.SaveFile.Containers
{
	public class GalleryHashSet<T> : HashSet<T>
	{
		public new bool Add(T val) {
			if (Plugin.InGallery) {
				GalleryLogger.LogDebug("Not saving because it is in gallery mode");
				return false;
			}

			var res = base.Add(val);
			if (res) {
				GalleryState.Save();
			}
		
			return res;
		}

		public new bool Remove(T val) {
			var res = base.Remove(val);
			if (res) {
				GalleryState.Save();
			}

			return res;
		}
	}
}