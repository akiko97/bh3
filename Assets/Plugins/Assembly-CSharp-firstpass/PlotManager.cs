using System.Collections.Generic;
using UnityEngine;

public class PlotManager : MonoBehaviour
{
	public class Plotter
	{
		public bool child;

		private Plotter Parent;

		private string name = string.Empty;

		private Color plotColor = Color.green;

		private Rect gridRect;

		private Texture2D grid;

		private int gridWidth = 354;

		private int gridHeight = 262;

		private float minValue;

		private float maxValue;

		private float scale;

		private int floor;

		private int top;

		private Color[] buffer;

		private int[] data;

		private int dataIndex = -1;

		private bool dataFull;

		private int zeroLine = -1;

		private Dictionary<string, Plotter> children = new Dictionary<string, Plotter>();

		public Plotter(string name, Texture2D blankGraph, Color plotColor, Plotter parent)
		{
			InitPlotterChild(name, blankGraph, parent.minValue, parent.maxValue, plotColor, parent);
		}

		public Plotter(string name, Texture2D blankGraph, float min, float max, Color plotColor, Plotter parent)
		{
			InitPlotterChild(name, blankGraph, min, max, plotColor, parent);
		}

		public Plotter(string name, Texture2D blankGraph, float min, float max, Color plotColor, Vector2 pos)
		{
			this.name = name;
			this.plotColor = plotColor;
			gridRect = new Rect(pos.x, pos.y, blankGraph.width, blankGraph.height);
			grid = new Texture2D(blankGraph.width, blankGraph.height);
			gridWidth = grid.width;
			gridHeight = grid.height;
			buffer = blankGraph.GetPixels();
			data = new int[gridWidth];
			floor = 0;
			top = gridHeight + Mathf.RoundToInt((float)gridHeight * 0.17f) + floor;
			minValue = min;
			maxValue = max;
			scale = (max - min) / (float)top;
			if (max > 0f && min < 0f)
			{
				zeroLine = (int)((0f - minValue) / scale) + floor;
			}
		}

		public void InitPlotterChild(string name, Texture2D blankGraph, float min, float max, Color plotColor, Plotter parent)
		{
			this.name = name;
			this.plotColor = plotColor;
			minValue = min;
			maxValue = max;
			gridHeight = parent.grid.height;
			gridWidth = parent.grid.width;
			data = new int[gridWidth];
			floor = 0;
			top = gridHeight + Mathf.RoundToInt((float)gridHeight * 0.17f) + floor;
			scale = (max - min) / (float)top;
			if (max > 0f && min < 0f)
			{
				zeroLine = (int)((0f - minValue) / scale) + floor;
			}
			child = true;
			Parent = parent;
			parent.AddChild(this);
		}

		public void Add(float y)
		{
			int num = floor;
			dataIndex++;
			if (dataIndex == gridWidth)
			{
				dataIndex = 0;
				dataFull = true;
			}
			num = ((y > maxValue) ? top : ((!(y < minValue)) ? ((int)((y - minValue) / scale) + floor) : floor));
			data[dataIndex] = num;
		}

		public void Draw()
		{
			grid.SetPixels(buffer);
			int num = grid.width;
			for (int num2 = dataIndex; num2 > 0; num2--)
			{
				grid.SetPixel(num, data[num2], plotColor);
				num--;
			}
			if (dataFull)
			{
				for (int num3 = gridWidth - 1; num3 > dataIndex; num3--)
				{
					grid.SetPixel(num, data[num3], plotColor);
					num--;
				}
			}
			if (zeroLine > 0)
			{
				for (int i = 0; i < gridWidth - 1; i++)
				{
					grid.SetPixel(i, zeroLine, Color.yellow);
				}
			}
			grid.Apply(false);
			if (children.Count > 0)
			{
				foreach (KeyValuePair<string, Plotter> child in children)
				{
					child.Value.DrawChild();
				}
			}
			GUI.DrawTexture(gridRect, grid);
		}

		private void DrawChild()
		{
			int num = Parent.grid.width;
			for (int num2 = dataIndex; num2 > 0; num2--)
			{
				Parent.grid.SetPixel(num, data[num2], plotColor);
				num--;
			}
			if (dataFull)
			{
				for (int num3 = gridWidth - 1; num3 > dataIndex; num3--)
				{
					Parent.grid.SetPixel(num, data[num3], plotColor);
					num--;
				}
			}
			Parent.grid.Apply(false);
		}

		public void AddChild(Plotter child)
		{
			if (!children.ContainsKey(child.name))
			{
				children.Add(child.name, child);
			}
		}
	}

	public int depth = 2;

	private Texture2D grid;

	private Dictionary<string, Plotter> plots = new Dictionary<string, Plotter>();

	private static PlotManager instance;

	public static PlotManager Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		int width = Screen.width;
		int height = 80;
		grid = new Texture2D(width, height, TextureFormat.ARGB32, false);
		Color[] array = new Color[262144];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new Color(0f, 0f, 0f, 0f);
		}
		grid.SetPixels(array);
		grid.Apply();
		instance = this;
	}

	private void OnGUI()
	{
		GUI.depth = depth;
		if (plots.Count <= 0)
		{
			return;
		}
		foreach (KeyValuePair<string, Plotter> plot in plots)
		{
			if (!plot.Value.child)
			{
				plot.Value.Draw();
			}
		}
	}

	public void PlotAdd(string plotName, float value)
	{
		if (plots.ContainsKey(plotName))
		{
			plots[plotName].Add(value);
		}
	}

	public void PlotCreate(string plotName, float min, float max, Color plotColor, Vector2 pos)
	{
		if (!plots.ContainsKey(plotName))
		{
			plots.Add(plotName, new Plotter(plotName, grid, min, max, plotColor, pos));
		}
	}

	public void PlotCreate(string plotName, float min, float max, Color plotColor, string parentName)
	{
		if (!plots.ContainsKey(plotName) && plots.ContainsKey(parentName))
		{
			plots.Add(plotName, new Plotter(plotName, grid, min, max, plotColor, plots[parentName]));
		}
	}

	public void PlotCreate(string plotName, Color plotColor, string parentName)
	{
		if (!plots.ContainsKey(plotName) && plots.ContainsKey(parentName))
		{
			plots.Add(plotName, new Plotter(plotName, grid, plotColor, plots[parentName]));
		}
	}
}
