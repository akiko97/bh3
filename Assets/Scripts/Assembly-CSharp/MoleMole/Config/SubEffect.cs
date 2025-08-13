using FullInspector;

namespace MoleMole.Config
{
	public class SubEffect
	{
		public string prefabPath;

		public string predicate;

		[InspectorNullable]
		public EffectCreationOp onCreate;
	}
}
