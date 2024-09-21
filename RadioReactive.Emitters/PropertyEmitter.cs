using System;
using System.Collections.Generic;
using RadioReactive.Core;
using RadioReactive.Core.Disposables;

namespace RadioReactive.Emitters
{
	public sealed class PropertyEmitter<T> : IEmitter<T>
	{	
#if DEBUG
		public StreamEmitter<T> Stream => _stream;
#endif
		
		public T Value
		{
			get { return _value; }
			set
			{
				if (_comparer.Equals(_value, value))
					return;
				
				_value = value;
				_stream.OnNext(value);
			}
		}
		
		private T _value;
		
		private readonly StreamEmitter<T> _stream;
		private readonly IEqualityComparer<T> _comparer;

		public PropertyEmitter(T value, IEqualityComparer<T> comparer)
		{
			_value = value;
			_stream = new StreamEmitter<T>();
			_comparer = comparer;
		}
		public PropertyEmitter(T value) : this(value, EqualityComparer<T>.Default) { }
		public PropertyEmitter() : this(default) { }
		
		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			if (!cancel.IsRequested)
				receiver.OnNext(_value);
			
			if (!cancel.IsRequested)
				_stream.Connect(receiver, cancel);
		}

		public override string ToString()
		{
			return Value.ToString();
		}

		public static implicit operator T(PropertyEmitter<T> property)
		{
			return property._value;
		}
	}
}