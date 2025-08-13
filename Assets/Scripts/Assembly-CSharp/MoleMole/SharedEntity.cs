using BehaviorDesigner.Runtime;

namespace MoleMole
{
	public class SharedEntity : SharedVariable<BaseMonoEntity>
	{
		public override string ToString()
		{
			return (!(mValue != null)) ? "<null entity>" : mValue.gameObject.name;
		}

		public static implicit operator SharedEntity(BaseMonoEntity value)
		{
			SharedEntity sharedEntity = new SharedEntity();
			sharedEntity.mValue = value;
			return sharedEntity;
		}
	}
}
