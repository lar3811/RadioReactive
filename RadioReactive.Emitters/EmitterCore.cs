using System;
using System.Collections.Generic;
using RadioReactive.Core;
using RadioReactive.Core.Disposables;
using RadioReactive.Emitters.Signals;

namespace RadioReactive.Emitters
{
#if DEBUG
	public
#else
	internal
#endif
	sealed class EmitterCore<T> : IEmitterCore<T>
	{
#if DEBUG
		public
#else
		private
#endif
		sealed class Connection : ICompositeDisposable
		{
			public readonly int Id;

			public IReceiver<T> Receiver;
			public EmitterCore<T> Emitter;

			public Connection(int id, IReceiver<T> receiver, EmitterCore<T> emitter)
			{
				Id = id;
				Receiver = receiver;
				Emitter = emitter;
			}
			
			public void OnEmitterDisposed()
			{
				Emitter = null;
				Receiver = null;
			}

			void IDisposable.Dispose()
			{
				if (Emitter != null)
					Emitter._connections.Remove(this);
				
				Emitter = null;
				Receiver = null;

				if (Core != null)
				{
					Core.Dispose();
					Core = null;
				}
			}

			public IDisposable Core { get; set; }
		}


		
#if DEBUG
		public List<Connection> Connections => _connections;
#endif

		private int _id = int.MinValue;

		private readonly List<Connection> _connections;

		public EmitterCore()
		{
			_connections = new List<Connection>();
		}

		public EmitterCore(int listeners)
		{
			_connections = new List<Connection>(listeners);
		}

		public void Connect(IReceiver<T> receiver, ICancel cancel)
		{
			if (cancel.IsRequested)
				return;
			
			var id = GetNextId();
			var connection = new Connection(id, receiver, this);
			_connections.Add(connection);
			cancel.Compose(connection);
		}

		public void OnCompleted()
		{
			Broadcast(new CompletionSignal<T>());
			Dispose();
		}

		public void OnError(Exception error)
		{
			Broadcast(new ErrorSignal<T>(error));
			Dispose();
		}

		public void OnNext(T value)
		{
			Broadcast(new ValueSignal<T>(@value));
		}

		private void Dispose()
		{
			for (int i = 0; i < _connections.Count; i++)
			{
				_connections[i].OnEmitterDisposed();
			}
			_connections.Clear();
			_id = int.MinValue;
		}

		private int GetNextId()
		{
			return _id++;
		}

		private void Broadcast<TSignal>(TSignal signal) where TSignal : ISignal<T>
		{
			if (_connections.Count == 0)
				return;

			var last = _connections[_connections.Count - 1];
			var next = _connections[0];
			do
			{
				var current = next;
				next = null;

				signal.Tune(current.Receiver);

				for (int i = 0; i < _connections.Count; i++)
				{
					if (_connections[i].Id > current.Id)
					{
						next = _connections[i];
						break;
					}
				}
			}
			while (next != null && next.Id <= last.Id);

			if (_connections.Count > 0)
				_id = _connections[_connections.Count - 1].Id + 1;
			else
				_id = int.MinValue;
		}
	}
}