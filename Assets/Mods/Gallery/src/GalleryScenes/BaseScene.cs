using Gallery.SaveFile.Containers;

namespace Gallery.GalleryScenes
{
	public abstract class BaseScene: IGalleryScene
	{
		public bool RapeCounted = false;

		public bool CreampieCounted = false;

		public bool NormalCounted = false;

		public bool ToiletCounted = false;

		public bool DeliveryCounted = false;
		public bool PregnantCounted = false;

		private readonly CommonStates chara1;
		private readonly CommonStates chara2;

		private readonly GalleryChara galleryChara1;
		private readonly GalleryChara galleryChara2;

		public BaseScene(CommonStates chara1, CommonStates chara2) {
			this.chara1 = chara1;
			this.galleryChara1 = new GalleryChara(chara1);
			if (chara2 != null) {
				this.chara2 = chara2;
				this.galleryChara2 = new GalleryChara(chara2);
			}
		}

		public virtual void OnRapeCount()
		{
			this.RapeCounted = true;
		}

		public virtual void OnCreampieCount()
		{
			this.CreampieCounted = true;
		}

		public virtual void OnNormalCount()
		{
			this.NormalCounted = true;
		}

		public virtual void OnToiletCount()
		{
			this.ToiletCounted = true;
		}

		public virtual void OnDeliveryCount()
		{
			this.DeliveryCounted = true;
		}

		public virtual void OnPregnantCount()
		{
			this.PregnantCounted = true;
		}


		public virtual bool IsCharacterInScene(CommonStates character)
		{
			return chara1 == character || chara2 == character;
		}

		public virtual void OnEnd()
		{
			throw new System.NotImplementedException();
		}

		public virtual CommonStates GetCharacter1()
		{
			return this.chara1;
		}

		public virtual GalleryChara GetGaleryCharacter1()
		{
			return this.galleryChara1;
		}

		public virtual CommonStates GetCharacter2()
		{
			return this.chara2;
		}

		public virtual GalleryChara GetGaleryCharacter2()
		{
			return this.galleryChara2;
		}
	}
}