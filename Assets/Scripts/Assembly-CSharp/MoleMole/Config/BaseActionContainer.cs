using System;
using FullInspector;

namespace MoleMole.Config
{
	public abstract class BaseActionContainer
	{
		[NonSerialized]
		[ShowInInspector]
		public int localID = -1;

		public abstract ConfigAbilityAction[][] GetAllSubActions();
	}
}
