using UnityEngine;

namespace MoleMole
{
	public class AsyncAssetRequst
	{
		public AsyncOperation operation;

		public object asset
		{
			get
			{
				if (operation is ResourceRequest)
				{
					return ((ResourceRequest)operation).asset;
				}
				if (operation is AssetBundleRequest)
				{
					return ((AssetBundleRequest)operation).asset;
				}
				return null;
			}
		}

		public AsyncAssetRequst(AsyncOperation operation)
		{
			this.operation = operation;
		}
	}
}
