using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	[TaskCategory("Monster")]
	public class GetMonsterHP : Action
	{
		public SharedFloat HPRatio;

		public override void OnAwake()
		{
		}

		public override void OnStart()
		{
		}

		public override TaskStatus OnUpdate()
		{
			BaseMonoMonster component = GetComponent<BaseMonoMonster>();
			MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(component.GetRuntimeID());
			HPRatio.SetValue((float)actor.HP / (float)actor.maxHP);
			return TaskStatus.Success;
		}
	}
}
