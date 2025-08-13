namespace MoleMole
{
	public class LDWaitAllMonsterLessThanSpecifiedNum : BaseLDEvent
	{
		private int _num;

		public LDWaitAllMonsterLessThanSpecifiedNum(double num = 0.0)
		{
			int num2 = Singleton<MonsterManager>.Instance.MonsterCount();
			if (num != 0.0 && (double)num2 <= num)
			{
				Done();
			}
			else if (num != 0.0)
			{
			}
			_num = (int)num;
		}

		public override void OnEvent(BaseEvent evt)
		{
			if (!(evt is EvtKilled) || Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 4)
			{
				return;
			}
			int num = 0;
			foreach (BaseMonoMonster allMonster in Singleton<MonsterManager>.Instance.GetAllMonsters())
			{
				if (allMonster.IsActive())
				{
					num++;
				}
			}
			if (num <= _num)
			{
				Done();
			}
		}
	}
}
