using RadioReactive.Core;

namespace RadioReactive.Emitters
{
#if DEBUG
	public
#else
	internal
#endif
	interface IEmitterCore<T> : IReceiver<T>, IEmitter<T> { }
}