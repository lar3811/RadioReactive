using RadioReactive.Core.Disposables;

namespace RadioReactive.Core
{
	public sealed class NeverCancel : ICancel
	{
		public static readonly NeverCancel Instance = new NeverCancel();
		
		public bool IsRequested => false;

		private NeverCancel() { }
			
		public void Compose(ICompositeDisposable operation) { }

		public Cancel Propagate()
		{
			return new Cancel();
		}
	}
}