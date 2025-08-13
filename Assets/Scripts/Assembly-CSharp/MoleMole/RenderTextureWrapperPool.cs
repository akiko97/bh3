using System.Collections.Generic;

namespace MoleMole
{
	public class RenderTextureWrapperPool
	{
		private List<RenderTextureWrapper> _usedList = new List<RenderTextureWrapper>();

		private List<RenderTextureWrapper> _availableList = new List<RenderTextureWrapper>();

		public List<RenderTextureWrapper> usedList
		{
			get
			{
				return _usedList;
			}
		}

		public int GetUsedCount()
		{
			return _usedList.Count;
		}

		public RenderTextureWrapper GetItem()
		{
			RenderTextureWrapper renderTextureWrapper;
			if (_availableList.Count > 0)
			{
				renderTextureWrapper = _availableList[0];
				_availableList.RemoveAt(0);
			}
			else
			{
				renderTextureWrapper = new RenderTextureWrapper();
			}
			_usedList.Add(renderTextureWrapper);
			return renderTextureWrapper;
		}

		public void ReleaseItem(RenderTextureWrapper item)
		{
			if (item != null)
			{
				item.__Release();
				_usedList.Remove(item);
				_availableList.Add(item);
			}
		}
	}
}
