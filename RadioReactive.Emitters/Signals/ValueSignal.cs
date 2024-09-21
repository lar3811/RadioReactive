using RadioReactive.Core;

namespace RadioReactive.Emitters.Signals
{
#if DEBUG
	public
#else
	internal
#endif
	struct ValueSignal<T> : ISignal<T>
	{
#if DEBUG
		public T Value => _value; 
#endif
		
		private readonly T _value;

		public ValueSignal(T value)
		{
			_value = value;
		}

		public void Tune(IReceiver<T> receiver)
		{
			receiver.OnNext(_value);
		}
	}
}