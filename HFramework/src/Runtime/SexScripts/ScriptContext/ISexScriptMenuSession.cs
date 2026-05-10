using HFramework.ScriptNodes;

namespace HFramework.SexScripts.ScriptContext
{
	public interface ISexScriptMenuSession
	{
		void SetOptions((string Id, string Text, MenuOption.EffectType Effect)[] options);
		void Show();
		void Hide();
		void Close();
	}
}
