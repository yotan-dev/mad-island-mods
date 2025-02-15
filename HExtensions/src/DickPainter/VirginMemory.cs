using HFramework.Hook;
using HFramework.ParamContainers;
using HFramework.Scenes;
using YotanModCore.Consts;

namespace HExtensions.DickPainter
{
	public class VirginMemory : HookMemory
	{
		public bool IsVirgin = false;

		public VirginMemory(string uid) : base(uid)
		{
		}

		public override void Save(IScene scene, object param)
		{
			FromToParams? fromTo = param as FromToParams?;
			if (!fromTo.HasValue || fromTo.Value.To == null)
				return;

			this.IsVirgin = fromTo.Value.To.sexInfo[SexInfoIndex.FirstSex] == -1;
		}
	}
}
