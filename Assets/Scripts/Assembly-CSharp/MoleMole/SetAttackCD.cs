using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace MoleMole
{
	public class SetAttackCD : Action
	{
		[Tooltip("The value to set the SharedFloat to")]
		public SharedFloat targetValue;

		[Tooltip("CD Ratio")]
		public SharedFloat CDRatio;

		[RequiredField]
		[Tooltip("The SharedFloat to set")]
		public SharedFloat targetVariable;

		public override TaskStatus OnUpdate()
		{
			BaseMonoAnimatorEntity component = GetComponent<BaseMonoAnimatorEntity>();
			float num = targetValue.Value;
			if (CDRatio.Value != 0f)
			{
				num *= CDRatio.Value;
			}
			targetVariable.SetValue(num * component.GetProperty("AI_AttackCDRatio"));
			return TaskStatus.Success;
		}

		public override void OnReset()
		{
			targetValue = 0f;
			targetVariable = 0f;
		}
	}
}
