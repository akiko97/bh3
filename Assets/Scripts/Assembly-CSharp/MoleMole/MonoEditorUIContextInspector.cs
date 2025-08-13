using System.Collections.Generic;
using FullInspector;
using UnityEngine;

namespace MoleMole
{
	[fiInspectorOnly]
	public class MonoEditorUIContextInspector : MonoBehaviour
	{
		[InspectorCollapsedFoldout]
		public List<BasePageContext> pageContextStack = new List<BasePageContext>();

		[InspectorCollapsedFoldout]
		public ViewCache pageCache;

		[InspectorCollapsedFoldout]
		public ViewCache diaglogCache;

		[InspectorCollapsedFoldout]
		public ViewCache widgetCache;
	}
}
