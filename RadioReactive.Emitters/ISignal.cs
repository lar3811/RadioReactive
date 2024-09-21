using RadioReactive.Core;

namespace RadioReactive.Emitters
{
#if DEBUG
	public
#else
	internal
#endif
	interface ISignal<out T>
	{
		void Tune(IReceiver<T> receiver);
	}
}