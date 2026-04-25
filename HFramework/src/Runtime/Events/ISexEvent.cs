namespace HFramework.Events
{
	/**
	 * Those interfaces are used to provide a unified way to handle sex events.
	 * They are used to provide a way to trigger events and to read events.
	 *
	 * They are being kept in a single file since they are very coupled to each other, at the end providing ISexEvent
	 */

	// Base non-generic interface for type erasure
	public interface ISexEventBase
	{
		string GetId();
		void TriggerWithBaseArgs(SexEventArgs args);
	}

	// Contravariant interface for write/trigger access to events
	public interface ITriggerableSexEvent<in T> where T : SexEventArgs
	{
		void Trigger(T args);
	}

	// Covariant interface for read-only access to events
	public interface IReadOnlySexEvent<out T> : ISexEventBase where T : SexEventArgs
	{
		// Can be used in covariant scenarios
	}

	// Combined interface for concrete event implementations
	public interface ISexEvent<T> : IReadOnlySexEvent<T>, ITriggerableSexEvent<T> where T : SexEventArgs, new()
	{
		// Combines both covariant and contravariant interfaces
	}
}
