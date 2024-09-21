using System;

namespace RadioReactive.Core.Disposables
{
	public interface ICompositeDisposable : IDisposable
	{
		IDisposable Core { get; set; }
	}
}