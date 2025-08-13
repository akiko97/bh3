using System;
using System.Collections.Generic;

namespace UniRx
{
	public static class DisposableExtensions
	{
		public static T AddTo<T>(this T disposable, ICollection<IDisposable> container) where T : IDisposable
		{
			if (disposable == null)
			{
				throw new ArgumentNullException("disposable");
			}
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			container.Add(disposable);
			return disposable;
		}
	}
}
