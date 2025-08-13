using System;

namespace UniRx
{
	public static class Disposable
	{
		private class EmptyDisposable : IDisposable
		{
			public static EmptyDisposable Singleton = new EmptyDisposable();

			private EmptyDisposable()
			{
			}

			public void Dispose()
			{
			}
		}

		private class AnonymousDisposable : IDisposable
		{
			private bool isDisposed;

			private readonly Action dispose;

			public AnonymousDisposable(Action dispose)
			{
				this.dispose = dispose;
			}

			public void Dispose()
			{
				if (!isDisposed)
				{
					isDisposed = true;
					dispose();
				}
			}
		}

		public static readonly IDisposable Empty = EmptyDisposable.Singleton;

		public static IDisposable Create(Action disposeAction)
		{
			return new AnonymousDisposable(disposeAction);
		}
	}
}
