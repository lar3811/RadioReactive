using RadioReactive.Core.Disposables;

namespace RadioReactive.Core
{
	public interface ICancel
	{
		bool IsRequested { get; }
		void Compose(ICompositeDisposable operation);
		Cancel Propagate();
	}
}