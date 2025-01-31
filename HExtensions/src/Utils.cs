using UnityEngine;

namespace HExtensions
{
	public static class Utils
	{
		/// <summary>
		/// Makes an alpha blending of overlayColor over baseColor, where overlayColor is alpha trnsparent
		/// </summary>
		/// <param name="baseColor"></param>
		/// <param name="overlayColor"></param>
		/// <param name="overlayAlpha">between 0 (transparent) and 1 (opaque)</param>
		/// <returns></returns>
		public static Color32 AlphaBlend(Color32 baseColor, Color32 overlayColor, float overlayAlpha)
		{
			return new Color32(
				(byte) (overlayAlpha * overlayColor.r + (1 - overlayAlpha) * baseColor.r),
				(byte) (overlayAlpha * overlayColor.g + (1 - overlayAlpha) * baseColor.g),
				(byte) (overlayAlpha * overlayColor.b + (1 - overlayAlpha) * baseColor.b),
				255
			);
		}
	}
}
