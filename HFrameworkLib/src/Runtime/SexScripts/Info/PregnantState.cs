using System;

namespace HFramework.SexScripts.Info
{
	public enum PregnantState
	{
		/// <summary>
		/// Any condition when it comes to pregnancy
		/// </summary>
		Any,
		/// <summary>
		/// Not pregnant at all (pregnancy did not start)
		/// </summary>
		NotPregnant,
		/// <summary>
		/// Pregnant (pregnancy started, with or without belly)
		/// </summary>
		Pregnant,
		/// <summary>
		/// Not ready to give birth (no belly yet) -- regardless if pregnant or not.
		/// </summary>
		NotReadyToGiveBirth,
		/// <summary>
		/// Pregnant, but not ready to give birth (no belly yet)
		/// </summary>
		PregNotReadyToGiveBirth,
		/// <summary>
		/// Ready to give birth (has belly)
		/// </summary>
		ReadyToGiveBirth,
	}
}
