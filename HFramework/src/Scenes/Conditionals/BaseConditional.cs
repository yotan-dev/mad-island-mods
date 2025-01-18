using System.Xml.Serialization;

namespace HFramework.Scenes.Conditionals
{
	[XmlInclude(typeof(FaintCheck))]
	[XmlInclude(typeof(FriendCheck))]
	[XmlInclude(typeof(JokeCheck))]
	[XmlInclude(typeof(LibidoCheck))]
	[XmlInclude(typeof(PerfumeCheck))]
	[XmlInclude(typeof(PregnantCheck))]
	[XmlInclude(typeof(QuestProgressCheck))]
	[XmlInclude(typeof(SexTypeCheck))]
	public abstract class BaseConditional : IConditional
	{
		public abstract bool Pass(IScene scene);

		public abstract bool Pass(CommonStates from, CommonStates to);
	}
}
