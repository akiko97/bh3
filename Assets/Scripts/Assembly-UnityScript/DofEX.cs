using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/DofEX")]
public class DofEX : MonoBehaviour
{
	[Serializable]
	public enum BokehQuality
	{
		Normal = 0,
		High = 1
	}

	public bool visualizeFocus;

	public float focalLength;

	public float focalSize;

	public float aperture;

	public BokehQuality bokehQuality;

	private float blurFactor;

	private float bokehInclination;

	[HideInInspector]
	public Shader dofHdrShader;

	private Material dofHdrMaterial;

	private float focalDistance;

	private float internalBlurWidth;

	protected bool isSupported;

	private Vector4[] dofHexagon;

	private Vector4[] dofVector;

	public DofEX()
	{
		focalLength = 5f;
		focalSize = 0.05f;
		aperture = 11.5f;
		bokehQuality = BokehQuality.High;
		blurFactor = 10f;
		focalDistance = 10f;
		internalBlurWidth = 1f;
		isSupported = true;
		dofHexagon = new Vector4[7];
		dofVector = new Vector4[7];
	}

	public virtual Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
	{
		object result;
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			enabled = false;
			result = null;
		}
		else if (s.isSupported && (bool)m2Create && m2Create.shader == s)
		{
			result = m2Create;
		}
		else if (!s.isSupported)
		{
			NotSupported();
			Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not supported on this platform!");
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return (Material)result;
	}

	public virtual Material CreateMaterial(Shader s, Material m2Create)
	{
		object result;
		if (!s)
		{
			Debug.Log("Missing shader in " + ToString());
			result = null;
		}
		else if ((bool)m2Create && m2Create.shader == s && s.isSupported)
		{
			result = m2Create;
		}
		else if (!s.isSupported)
		{
			result = null;
		}
		else
		{
			m2Create = new Material(s);
			m2Create.hideFlags = HideFlags.DontSave;
			result = ((!m2Create) ? null : m2Create);
		}
		return (Material)result;
	}

	public virtual bool CheckSupport(bool needDepth)
	{
		isSupported = true;
		int result;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures)
		{
			NotSupported();
			result = 0;
		}
		else if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
		{
			NotSupported();
			result = 0;
		}
		else
		{
			if (needDepth)
			{
				GetComponent<Camera>().depthTextureMode = GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
			}
			result = 1;
		}
		return (byte)result != 0;
	}

	public virtual bool CheckSupport(bool needDepth, bool needHdr)
	{
		return CheckSupport(needDepth) ? true : false;
	}

	public virtual void ReportAutoDisable()
	{
		Debug.LogWarning("The image effect " + ToString() + " has been disabled as it's not supported on the current platform.");
	}

	public virtual bool CheckShader(Shader s)
	{
		Debug.Log("The shader " + s.ToString() + " on effect " + ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
		int result;
		if (!s.isSupported)
		{
			NotSupported();
			result = 0;
		}
		else
		{
			result = 0;
		}
		return (byte)result != 0;
	}

	public virtual void NotSupported()
	{
		enabled = false;
		isSupported = false;
	}

	public virtual bool CheckResources()
	{
		CheckSupport(true);
		dofHdrMaterial = CheckShaderAndCreateMaterial(dofHdrShader, dofHdrMaterial);
		if (!isSupported)
		{
			ReportAutoDisable();
		}
		return isSupported;
	}

	public virtual void OnEnable()
	{
		GetComponent<Camera>().depthTextureMode = GetComponent<Camera>().depthTextureMode | DepthTextureMode.Depth;
	}

	public virtual void OnDisable()
	{
		if ((bool)dofHdrMaterial)
		{
			UnityEngine.Object.DestroyImmediate(dofHdrMaterial);
		}
		dofHdrMaterial = null;
	}

	public virtual void Start()
	{
		float num = 1f;
		int num2 = default(int);
		dofHexagon[0] = new Vector4(0f, 0f, 0f, 0f);
		for (num2 = 0; num2 < 6; num2++)
		{
			float num3 = Mathf.Sin((float)(Math.PI / 3.0 * (double)num2));
			float num4 = Mathf.Cos((float)(Math.PI / 3.0 * (double)num2));
			float num5 = 0f;
			float num6 = 1f;
			float x = num4 * num5 - num3 * num6;
			float y = num3 * num5 + num4 * num6;
			dofHexagon[num2 + 1] = new Vector4(x, y, 0f, 0f) * num;
		}
	}

	public virtual float GetFocalDistance(float worldDist)
	{
		return GetComponent<Camera>().WorldToViewportPoint((worldDist - GetComponent<Camera>().nearClipPlane) * GetComponent<Camera>().transform.forward + GetComponent<Camera>().transform.position).z / (GetComponent<Camera>().farClipPlane - GetComponent<Camera>().nearClipPlane);
	}

	private void WriteCoc(RenderTexture fromTo, bool fgDilate)
	{
		dofHdrMaterial.SetTexture("_FgOverlap", null);
		Graphics.Blit(fromTo, fromTo, dofHdrMaterial, 0);
	}

	public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		if (!(aperture >= 0f))
		{
			aperture = 0f;
		}
		focalSize = Mathf.Clamp(focalSize, 0f, 2f);
		focalDistance = GetFocalDistance(focalLength);
		dofHdrMaterial.SetVector("_CurveParams", new Vector4(1f, focalSize, aperture / 10f, focalDistance));
		RenderTexture renderTexture = null;
		RenderTexture renderTexture2 = null;
		if (visualizeFocus)
		{
			WriteCoc(source, true);
			Graphics.Blit(source, destination, dofHdrMaterial, 2);
			return;
		}
		source.filterMode = FilterMode.Bilinear;
		WriteCoc(source, true);
		renderTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
		renderTexture2 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
		int num = default(int);
		float num2 = source.width;
		float num3 = source.height;
		float num4 = num2;
		float num5 = num3;
		float num6 = 1f / (float)source.width;
		float num7 = 1f / (float)source.height;
		if (bokehQuality == BokehQuality.High)
		{
			dofVector[0] = new Vector4(blurFactor * num6, blurFactor * num7, 0f, 2f);
		}
		else
		{
			dofVector[0] = new Vector4(blurFactor * num6, blurFactor * num7, 0f, 2f);
		}
		dofHdrMaterial.SetVector("blurScale", dofVector[0]);
		float f = bokehInclination + aperture / 8f;
		float num8 = Mathf.Sin(f);
		float num9 = Mathf.Cos(f);
		float num10 = 0.5f;
		if (bokehQuality == BokehQuality.High)
		{
			num10 = 0.25f;
		}
		for (num = 1; num < 7; num++)
		{
			dofVector[num] = new Vector4(num9 * dofHexagon[num].x - num8 * dofHexagon[num].y, num8 * dofHexagon[num].x + num9 * dofHexagon[num].y, 0f, 0f) * num10;
		}
		for (num = 1; num < 7; num++)
		{
			dofHdrMaterial.SetVector("dofScatter" + num.ToString(), dofVector[num]);
		}
		Graphics.Blit(source, renderTexture, dofHdrMaterial, 1);
		if (bokehQuality == BokehQuality.High)
		{
			num10 = 0.5f;
			dofVector[0] = new Vector4(blurFactor * num6, blurFactor * num7, 0f, 2f);
		}
		else
		{
			num10 = 1f;
			dofVector[0] = new Vector4(blurFactor * num6, blurFactor * num7, 0f, 1f);
		}
		dofHdrMaterial.SetVector("blurScale", dofVector[0]);
		for (num = 1; num < 7; num++)
		{
			dofVector[num] = new Vector4(num9 * dofHexagon[num].x - num8 * dofHexagon[num].y, num8 * dofHexagon[num].x + num9 * dofHexagon[num].y, 0f, 0f) * num10;
		}
		for (num = 1; num < 7; num++)
		{
			dofHdrMaterial.SetVector("dofScatter" + num.ToString(), dofVector[num]);
		}
		if (bokehQuality == BokehQuality.Normal)
		{
			Graphics.Blit(renderTexture, destination, dofHdrMaterial, 1);
		}
		else
		{
			Graphics.Blit(renderTexture, renderTexture2, dofHdrMaterial, 1);
		}
		if (bokehQuality == BokehQuality.High)
		{
			num10 = 1f;
			dofVector[0] = new Vector4(blurFactor * num6, blurFactor * num7, 0f, 1f);
			dofHdrMaterial.SetVector("blurScale", dofVector[0]);
			for (num = 1; num < 7; num++)
			{
				dofVector[num] = new Vector4(num9 * dofHexagon[num].x - num8 * dofHexagon[num].y, num8 * dofHexagon[num].x + num9 * dofHexagon[num].y, 0f, 0f) * num10;
			}
			for (num = 1; num < 7; num++)
			{
				dofHdrMaterial.SetVector("dofScatter" + num.ToString(), dofVector[num]);
			}
			Graphics.Blit(renderTexture2, destination, dofHdrMaterial, 1);
		}
		if ((bool)renderTexture)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
		if ((bool)renderTexture2)
		{
			RenderTexture.ReleaseTemporary(renderTexture2);
		}
	}

	public virtual void Main()
	{
	}
}
