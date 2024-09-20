namespace Gallery.GalleryScenes
{
	public interface IGalleryScene
	{
		CommonStates GetCharacter1();
		CommonStates GetCharacter2();

		void OnRapeCount();
		void OnCreampieCount();
		void OnNormalCount();
		void OnToiletCount();
		void OnDeliveryCount();
		void OnPregnantCount();
		void OnEnd();

		bool IsCharacterInScene(CommonStates character);
	}
}
