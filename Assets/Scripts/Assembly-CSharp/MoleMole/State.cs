namespace MoleMole
{
	public abstract class State<T>
	{
		protected T _owner;

		public bool active { get; private set; }

		public State(T t)
		{
			_owner = t;
		}

		public virtual void Enter()
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Exit()
		{
		}

		public void SetActive(bool isActive)
		{
			active = isActive;
		}
	}
}
