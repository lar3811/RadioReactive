using System;
using RadioReactive.Core;
using RadioReactive.Emitters;
using RadioReactive.Emitters.Signals;
using NUnit.Framework;

namespace RadioReactive.TestsUtility
{
	public readonly struct StreamEmitterSnapshot<T>
	{
		public readonly StreamEmitter<T> Stream;
		public readonly EmitterCore<T> Substream;
		public readonly EmitterCore<T>.Connection[] Connections;

		public StreamEmitterSnapshot(StreamEmitter<T> stream)
		{
			Assert.NotNull(stream);
			Stream = stream;
			
			Assert.IsInstanceOf<StreamEmitter<T>.StreamingCore>(stream.Core);
			var core = (StreamEmitter<T>.StreamingCore)stream.Core;
			Substream = core.Core;
			Connections = Substream.Connections.ToArray();
		}

		public void AssertReceiverConnectionsDisposed()
		{
			for (int i = 0; i < Connections.Length; i++)
			{
				Assert.True(Connections[i].Receiver == null);
				Assert.True(!Substream.Connections.Contains(Connections[i]));
			}
		}

		public void AssertEmitterConnectionsDisposed()
		{
			for (int i = 0; i < Connections.Length; i++)
			{
				Assert.True(Connections[i].Emitter == null);
				Assert.True(!Substream.Connections.Contains(Connections[i]));
			}
		}

		public void AssertError(Exception exception)
		{
			Assert.IsInstanceOf<StreamEmitter<T>.BrokenCore>(Stream.Core);
			var core = (StreamEmitter<T>.BrokenCore)Stream.Core;
			Assert.AreSame(core.Error, exception);
			Assert.IsEmpty(Substream.Connections);
			AssertEmitterConnectionsDisposed();
		}

		public void AssertComplete()
		{
			Assert.AreSame(Stream.Core, StreamEmitter<T>.DepletedCore.Instance);
			Assert.IsEmpty(Substream.Connections);
			AssertEmitterConnectionsDisposed();
		}
	}
}