using RadioReactive.Core;

namespace RadioReactive.Emitters.Signals
{
#if DEBUG
	public
#else
	internal
#endif
	struct CompletionSignal<T> : ISignal<T>
	{
		public static readonly ISignal<T> Boxed = new CompletionSignal<T>();
		
		public void Tune(IReceiver<T> receiver)
		{
			receiver.OnCompleted();
		}
	}
}