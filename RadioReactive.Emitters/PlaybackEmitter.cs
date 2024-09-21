using RadioReactive.Core;

namespace RadioReactive.Emitters
{
	public sealed class PlaybackEmitter<T> : IEmitter<T>
	{
		private readonly T[] _values;

		public PlaybackEmitter(params T[] values)
		{
			_values = values;
		}
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			for (int i = 0; i < _values.Length && !cancel.IsRequested; i++) 
				receiver.OnNext(_values[i]);

			if (!cancel.IsRequested) 
				receiver.OnCompleted();
		}
	}
}