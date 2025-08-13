using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[Serializable]
	public class MaterialGroup : IDisposable
	{
		[Serializable]
		public class RendererMaterials
		{
			public static Material[] EMPTY_MATERIALS = new Material[0];

			public Material[] materials = EMPTY_MATERIALS;

			public MaterialColorModifier[] colorModifiers = new MaterialColorModifier[0];

			[NonSerialized]
			public bool skipped;
		}

		public string groupName;

		public RendererMaterials[] entries;

		private MaterialGroup _instancedGroup;

		public MaterialGroup(string groupName)
		{
			this.groupName = groupName;
			entries = new RendererMaterials[0];
		}

		public MaterialGroup(string groupName, Renderer[] renderers)
		{
			this.groupName = groupName;
			entries = new RendererMaterials[renderers.Length];
			for (int i = 0; i < entries.Length; i++)
			{
				Renderer renderer = renderers[i];
				entries[i] = new RendererMaterials();
				if (renderer is MeshRenderer || renderer is SkinnedMeshRenderer)
				{
					entries[i].materials = renderers[i].materials;
				}
				else
				{
					entries[i].skipped = true;
				}
			}
			List<Material> list = new List<Material>();
			for (int j = 0; j < entries.Length; j++)
			{
				RendererMaterials rendererMaterials = entries[j];
				rendererMaterials.colorModifiers = new MaterialColorModifier[rendererMaterials.materials.Length];
				for (int k = 0; k < rendererMaterials.materials.Length; k++)
				{
					Material material = rendererMaterials.materials[k];
					bool flag = false;
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].name == material.name)
						{
							UnityEngine.Object.Destroy(material);
							rendererMaterials.materials[k] = list[l];
							rendererMaterials.colorModifiers[k] = new MaterialColorModifier();
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list.Add(material);
						rendererMaterials.colorModifiers[k] = new MaterialColorModifier(rendererMaterials.materials[k]);
					}
				}
			}
			_instancedGroup = this;
		}

		public void ApplyTo(Renderer[] renderers)
		{
			for (int i = 0; i < entries.Length; i++)
			{
				if (!entries[i].skipped)
				{
					renderers[i].materials = entries[i].materials;
				}
			}
		}

		public MaterialGroup GetInstancedMaterialGroup()
		{
			if (_instancedGroup != null)
			{
				return _instancedGroup;
			}
			_instancedGroup = new MaterialGroup(string.Format("{0} (Instanced)", groupName));
			_instancedGroup.entries = new RendererMaterials[entries.Length];
			int num = 0;
			for (int i = 0; i < entries.Length; i++)
			{
				RendererMaterials rendererMaterials = entries[i];
				Material[] array = new Material[rendererMaterials.materials.Length];
				MaterialColorModifier[] array2 = new MaterialColorModifier[rendererMaterials.materials.Length];
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = new Material(rendererMaterials.materials[j]);
					array[j].name = string.Format("{0} #{1}", rendererMaterials.materials[j].name, num++);
					array2[j] = new MaterialColorModifier(array[j]);
				}
				_instancedGroup.entries[i] = new RendererMaterials();
				_instancedGroup.entries[i].materials = array;
				_instancedGroup.entries[i].colorModifiers = array2;
				_instancedGroup.entries[i].skipped = rendererMaterials.skipped;
			}
			return _instancedGroup;
		}

		public Material[] GetAllMaterials()
		{
			int num = 0;
			for (int i = 0; i < entries.Length; i++)
			{
				num += entries[i].materials.Length;
			}
			Material[] array = new Material[num];
			int num2 = 0;
			for (int j = 0; j < entries.Length; j++)
			{
				entries[j].materials.CopyTo(array, num2);
				num2 += entries[j].materials.Length;
			}
			return array;
		}

		private void DestroyInstancedMaterials()
		{
			for (int i = 0; i < entries.Length; i++)
			{
				for (int j = 0; j < entries[i].materials.Length; j++)
				{
					UnityEngine.Object.Destroy(entries[i].materials[j]);
				}
			}
		}

		public void Dispose()
		{
			if (_instancedGroup != null)
			{
				_instancedGroup.DestroyInstancedMaterials();
			}
		}

		public void ApplyColorModifiers()
		{
			for (int i = 0; i < entries.Length; i++)
			{
				MaterialColorModifier[] colorModifiers = entries[i].colorModifiers;
				for (int j = 0; j < colorModifiers.Length; j++)
				{
					colorModifiers[j].ApplyAndReset();
				}
			}
		}
	}
}
