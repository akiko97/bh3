namespace MoleMole
{
	public class MonoBronyaEvade : MonoBronya
	{
		public override void TriggerSkill(int skillNum)
		{
			if (skillNum == 1)
			{
				SetLocomotionBool("EvadeBackward", !GetActiveControlData().hasSteer);
			}
			base.TriggerSkill(skillNum);
		}
	}
}
