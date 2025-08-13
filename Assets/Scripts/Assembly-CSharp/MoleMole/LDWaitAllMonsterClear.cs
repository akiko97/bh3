namespace MoleMole
{
	public class LDWaitAllMonsterClear : BaseLDEvent
	{
		private int _num;

		public LDWaitAllMonsterClear(double num = 0.0)
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

		public override void Core()
		{
			if (Singleton<MonsterManager>.Instance.MonsterCount() <= _num)
			{
				Done();
			}
		}
	}
}
