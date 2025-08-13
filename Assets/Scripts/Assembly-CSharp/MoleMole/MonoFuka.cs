namespace MoleMole
{
	public class MonoFuka : BaseMonoAvatar
	{
		protected const string ATTACK_INDEX_NAME = "_AttackIndex";

		public override void Awake()
		{
			base.Awake();
		}

		public override void SetTrigger(string name)
		{
			if (name == "TriggerAttack")
			{
				SetLocomotionInteger("_AttackIndex", 0);
			}
			else if (name == "TriggerSkill_2")
			{
				name = "TriggerAttack";
				SetLocomotionInteger("_AttackIndex", 2);
			}
			base.SetTrigger(name);
		}

		public void SetPoseLeft()
		{
			SetLocomotionBool("_IsLeft", true);
		}

		public void SetPoseRight()
		{
			SetLocomotionBool("_IsLeft", false);
		}

		public void SetStateAir()
		{
			SetLocomotionBool("_IsAir", true);
		}

		public void SetStateGround()
		{
			SetLocomotionBool("_IsAir", false);
		}
	}
}
