using System;

namespace UniRx
{
	public class MultipleAssignmentDisposable : IDisposable, ICancelable
	{
		private static readonly BooleanDisposable True = new BooleanDisposable(true);

		private object gate = new object();

		private IDisposable current;

		public bool IsDisposed
		{
			get
			{
				lock (gate)
				{
					return current == True;
				}
			}
		}

		public IDisposable Disposable
		{
			get
			{
				lock (gate)
				{
					IDisposable result;
					if (current == True)
					{
						IDisposable empty = UniRx.Disposable.Empty;
						result = empty;
					}
					else
					{
						result = current;
					}
					return result;
				}
			}
			set
			{
				bool flag = false;
				lock (gate)
				{
					flag = current == True;
					if (!flag)
					{
						current = value;
					}
				}
				if (flag && value != null)
				{
					value.Dispose();
				}
			}
		}

		public void Dispose()
		{
			IDisposable disposable = null;
			lock (gate)
			{
				if (current != True)
				{
					disposable = current;
					current = True;
				}
			}
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}
}
