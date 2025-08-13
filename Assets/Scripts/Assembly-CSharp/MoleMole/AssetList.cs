using System.Collections.Generic;

namespace MoleMole
{
	public class AssetList
	{
		public class AssetItem
		{
			public uint _size;

			public string _name;

			public AssetItem(uint size, string name)
			{
				_size = size;
				_name = name;
			}
		}

		public class SizeComparer : IComparer<AssetItem>
		{
			public int Compare(AssetItem x, AssetItem y)
			{
				if (x._size > y._size)
				{
					return -1;
				}
				if (x._size < y._size)
				{
					return 1;
				}
				return 0;
			}
		}

		private SizeComparer _sizeComparer = new SizeComparer();

		private List<AssetItem> _list;

		private int _max = 10;

		public AssetList(int max = 10)
		{
			_list = new List<AssetItem>();
			_max = max;
		}

		public void TryAdd(uint size, string name)
		{
			if (_list.Count < _max)
			{
				_list.Add(new AssetItem(size, name));
				return;
			}
			Sort();
			AssetItem assetItem = _list[_list.Count - 1];
			if (size > assetItem._size)
			{
				assetItem._size = size;
				assetItem._name = name;
			}
		}

		public void Sort()
		{
			_list.Sort(_sizeComparer);
		}

		public List<AssetItem> GetList()
		{
			return _list;
		}

		public void Clear()
		{
			_list.Clear();
		}
	}
}
