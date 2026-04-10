public class BundleAsset<T> where T : UnityEngine.Object
{
	public string BundleName { get; set; }
	public T Asset { get; set; }
}
