using System;
using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public abstract class BaseInstancedRenderingProperty : IDisposable
	{
		public Material material;

		public int propertyID;

		public abstract BaseRenderingProperty CreateBaseRenderingProperty(string name);

		public abstract void ApplyProperty();

		public abstract void LerpStep(float t);

		public abstract void SetupTransition(BaseRenderingProperty target);

		public abstract void CopyFrom(BaseRenderingProperty target);

		public void Dispose()
		{
			UnityEngine.Object.Destroy(material);
		}
	}
}
