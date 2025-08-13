using BehaviorDesigner.Runtime;
using UnityEngine;

public class AOTLinker : MonoBehaviour
{
	public void Linker()
	{
		BehaviorManager.BehaviorTree behaviorTree = new BehaviorManager.BehaviorTree();
		BehaviorManager.TaskAddData taskAddData = new BehaviorManager.TaskAddData();
		BehaviorManager.TaskAddData.InheritedFieldValue inheritedFieldValue = new BehaviorManager.TaskAddData.InheritedFieldValue();
	}
}
