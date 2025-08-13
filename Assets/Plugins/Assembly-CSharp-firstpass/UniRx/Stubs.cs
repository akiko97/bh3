using System;

namespace UniRx
{
	internal static class Stubs
	{
		public static readonly Action Nop = delegate
		{
		};

		public static readonly Action<Exception> Throw = delegate(Exception ex)
		{
			throw ex;
		};

		public static void Ignore<T>(T t)
		{
		}
	}
}
