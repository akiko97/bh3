namespace BehaviorDesigner.Runtime.Tasks
{
	public abstract class BaseSwitch : Composite
	{
		protected int _currentChildIndex;

		protected TaskStatus _executionStatus;

		public bool InteruptChildOnSwitchChange;

		public override int CurrentChildIndex()
		{
			_currentChildIndex = CalculateChildIndex();
			return _currentChildIndex;
		}

		public override bool CanExecute()
		{
			return _executionStatus != TaskStatus.Success && _executionStatus != TaskStatus.Failure;
		}

		public override void OnChildStarted()
		{
			if (InteruptChildOnSwitchChange)
			{
				_executionStatus = TaskStatus.Running;
			}
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			_executionStatus = childStatus;
		}

		public override void OnConditionalAbort(int childIndex)
		{
			_currentChildIndex = CalculateChildIndex();
			_executionStatus = TaskStatus.Inactive;
		}

		public override void OnEnd()
		{
			_executionStatus = TaskStatus.Inactive;
		}

		public override bool CanReevaluate()
		{
			return InteruptChildOnSwitchChange;
		}

		public override bool OnReevaluationStarted()
		{
			if (_executionStatus == TaskStatus.Inactive)
			{
				return false;
			}
			int num = CalculateChildIndex();
			if (num != _currentChildIndex)
			{
				BehaviorManager.instance.Interrupt(base.Owner, children[_currentChildIndex], this);
				_currentChildIndex = num;
				return true;
			}
			return false;
		}

		public override void OnReevaluationEnded(TaskStatus status)
		{
		}

		protected abstract int CalculateChildIndex();
	}
}
