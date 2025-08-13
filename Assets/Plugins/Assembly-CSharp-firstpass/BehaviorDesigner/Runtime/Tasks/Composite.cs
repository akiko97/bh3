using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	public abstract class Composite : ParentTask
	{
		[SerializeField]
		[Tooltip("Specifies the type of conditional abort. More information is located at http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=89.")]
		protected AbortType abortType;

		public AbortType AbortType
		{
			get
			{
				return abortType;
			}
		}
	}
}
