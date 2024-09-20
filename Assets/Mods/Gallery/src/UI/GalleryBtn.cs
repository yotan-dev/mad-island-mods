using YotanModCore;
using UnityEngine;

namespace Gallery.UI
{
	public class GalleryBtn : MonoBehaviour
	{
		public void OnClick()
		{
			// Removes the character edit (not sure why, but this is done by original preview button)
			Managers.mn.gameMN.DestroyEdit();

			Plugin.InGallery = true;
			
			// Use the preview scene since creating a new one is too hard
			Managers.mn.sceneSC.SceneChange("gallery_01");
		}
	}

}