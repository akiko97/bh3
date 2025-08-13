using UnityEngine;
using UnityEngine.UI;

public class MyGridLayoutGroup : GridLayoutGroup
{
	public override void CalculateLayoutInputHorizontal()
	{
		base.rectChildren.Clear();
		for (int i = 0; i < base.rectTransform.childCount; i++)
		{
			RectTransform item = base.rectTransform.GetChild(i) as RectTransform;
			base.rectChildren.Add(item);
		}
		m_Tracker.Clear();
		int num = 0;
		int num2 = 0;
		if (m_Constraint == Constraint.FixedColumnCount)
		{
			num = (num2 = m_ConstraintCount);
		}
		else if (m_Constraint == Constraint.FixedRowCount)
		{
			num = (num2 = Mathf.CeilToInt((float)base.rectChildren.Count / (float)m_ConstraintCount - 0.001f));
		}
		else
		{
			num = 1;
			num2 = Mathf.CeilToInt(Mathf.Sqrt(base.rectChildren.Count));
		}
		SetLayoutInputForAxis((float)base.padding.horizontal + (base.cellSize.x + base.spacing.x) * (float)num - base.spacing.x, (float)base.padding.horizontal + (base.cellSize.x + base.spacing.x) * (float)num2 - base.spacing.x, -1f, 0);
	}
}
