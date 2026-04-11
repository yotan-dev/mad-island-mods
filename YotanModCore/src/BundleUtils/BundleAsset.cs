namespace YotanModCore.BundleUtils
{
	/// <summary>
	/// Wrapper class for an asset loaded from an asset bundle.
	/// </summary>
	/// <typeparam name="T">The type of asset to load.</typeparam>
	public class BundleAsset<T> where T : UnityEngine.Object
	{
		/// <summary>
		/// The name of the bundle that the asset was loaded from.
		/// </summary>
		public string BundleName { get; set; }

		/// <summary>
		/// The loaded asset.
		/// </summary>
		public T Asset { get; set; }
	}
}
