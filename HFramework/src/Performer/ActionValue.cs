namespace HFramework.Performer
{
	public record ActionValue(PlayType PlayType, string AnimationName, string[] Events, bool CanChangePose = true);
}
