using System;
using FullSerializer;

namespace MoleMole
{
	internal class ConfigIOnLoadedConverter : fsObjectProcessor
	{
		public override bool CanProcess(Type type)
		{
			return typeof(IOnLoaded).IsAssignableFrom(type);
		}

		public override void OnAfterDeserialize(Type storageType, object instance)
		{
			IOnLoaded onLoaded = (IOnLoaded)instance;
			if (onLoaded != null)
			{
				onLoaded.OnLoaded();
			}
		}
	}
}
