using System;
using RadioReactive.Core.Disposables;

namespace RadioReactive.Core
{
	public sealed class Cancel : ICancel, IDisposable
	{
		public bool IsRequested => _requested;

		private IDisposable _operation;
		private bool _requested;

		public void Request()
		{
			if (_requested)
				return;
			
			_requested = true;
			
			if (_operation != null)
			{
				_operation.Dispose();
				_operation = null;
			}
		}

		public void Compose(ICompositeDisposable operation)
		{
			if (operation == null)
				return;
			
			if (_requested)
			{
				operation.Dispose();
			}
			else if (_operation == null)
			{
				_operation = operation;
			}
			else
			{
				operation.Core = _operation;
				_operation = operation;
			}
		}

		public Cancel Propagate()
		{
			var progeny = new Cancel();
			Compose(new DisposableCancel(progeny));
			return progeny;
		}

		void IDisposable.Dispose()
		{
			Request();
		}



		private sealed class DisposableCancel : ICompositeDisposable
		{
			private readonly Cancel _cancel;

			public IDisposable Core { get; set; }

			public DisposableCancel(Cancel cancel)
			{
				_cancel = cancel;
			}

			public void Dispose()
			{
				_cancel.Request();
				
				if (Core != null)
				{
					Core.Dispose();
					Core = null;
				}
			}
		}
	}
}