namespace HFramework.ScriptNodes.Animation
{
	/// <summary>
	/// Interface for animation nodes. Contains common properties for all animation nodes.
	/// This helps patching animation nodes programatically.
	/// </summary>
	public interface IAnimationNode
	{
		/// <summary>
		/// The name of the animation to play
		/// </summary>
		string GetAnimationName();

		/// <summary>
		/// Sets the name of the animation to play
		/// </summary>
		/// <param name="animationName">The name of the animation to play</param>
		void SetAnimationName(string animationName);
	}
}
