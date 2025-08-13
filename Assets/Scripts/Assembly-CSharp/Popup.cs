using UnityEngine;

public class Popup
{
	private int selectedItemIndex;

	private bool isVisible;

	private bool isClicked;

	private static Popup current;

	public int List(Rect box, GUIContent[] items, GUIStyle boxStyle, GUIStyle listStyle, int xCount = 1)
	{
		if (isVisible)
		{
			Rect position = new Rect(box.x, box.y + box.height, box.width, box.height * (float)items.Length / (float)xCount);
			GUI.Box(position, string.Empty, boxStyle);
			GUI.depth--;
			selectedItemIndex = GUI.SelectionGrid(position, selectedItemIndex, items, xCount, listStyle);
			GUI.depth++;
			if (GUI.changed)
			{
				current = null;
			}
		}
		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		EventType typeForControl = Event.current.GetTypeForControl(controlID);
		if (typeForControl == EventType.MouseUp)
		{
			current = null;
		}
		if (GUI.Button(new Rect(box.x, box.y, box.width, box.height), items[selectedItemIndex]))
		{
			if (!isClicked)
			{
				current = this;
				isClicked = true;
			}
			else
			{
				isClicked = false;
			}
		}
		if (current == this)
		{
			isVisible = true;
		}
		else
		{
			isVisible = false;
			isClicked = false;
		}
		return selectedItemIndex;
	}

	public int GetSelectedItemIndex()
	{
		return selectedItemIndex;
	}
}
