using UnityEngine;

namespace MoleMole
{
	public abstract class BasePlatform
	{
		protected Transform _platformTrans;

		public MonoBasePerpStage StageOwner { get; private set; }

		public BasePlatform(MonoBasePerpStage stageOwner)
		{
			StageOwner = stageOwner;
		}

		public abstract Vector3 GetARandomPlace();

		public virtual void Core()
		{
		}
	}
}
