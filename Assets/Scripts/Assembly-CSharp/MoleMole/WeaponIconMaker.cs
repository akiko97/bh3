using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class WeaponIconMaker : MonoBehaviour
	{
		public enum WeaponType
		{
			Cannon = 0,
			Katana = 1,
			Pistol = 2
		}

		private static readonly string LONG_SWORD_NAME = "LongSword";

		private static readonly string SHORT_SWORD_NAME = "ShortSword";

		private static readonly string LEFT_PISTOL_NAME = "LeftPistol";

		private static readonly string RIGHT_PISTOL_NAME = "RightPistol";

		private static readonly string CLONE_SUFFIX = "(Clone)";

		private static readonly string BOUNDING_NAME = "Bounding";

		private static readonly string[] TYPE_PREFIX = new string[3] { "Weapon_Cannon", "Weapon_Katana", "Weapon_Pistol" };

		public int imageSize = 512;

		[Range(1f, 179f)]
		public float fov = 20f;

		public WeaponType weaponType;

		public bool wholeForKatana;

		public Camera mainCamera;

		public Camera cannonCamera;

		public Camera katanaCamera;

		public Camera pistolCamera;

		public Transform cannonHolder;

		public Transform longSwordHolder;

		public Transform shortSwordHolder;

		public Transform leftPistolHolder;

		public Transform rightPistolHolder;

		private GameObject _weaponObj;

		private List<GameObject> _objList;

		public string outputPath;

		private PostFX _postFX;

		private string _weaponName
		{
			get
			{
				if (_weaponObj != null)
				{
					return TrimSuffix(_weaponObj.name, CLONE_SUFFIX);
				}
				return null;
			}
		}

		private void Awake()
		{
			_objList = new List<GameObject>();
			_postFX = mainCamera.GetComponent<PostFX>();
		}

		public string Load(string srcPath)
		{
			return _load(srcPath);
		}

		public string Generate()
		{
			return _generate();
		}

		private string _load(string srcPath)
		{
			Screen.SetResolution(imageSize, imageSize, false);
			GameObject gameObject = Miscs.LoadResource<GameObject>(srcPath);
			if (gameObject == null)
			{
				return "Fail to load prefab at " + srcPath;
			}
			foreach (GameObject obj in _objList)
			{
				if (obj != null)
				{
					UnityEngine.Object.DestroyImmediate(obj);
				}
			}
			_objList.Clear();
			_weaponObj = UnityEngine.Object.Instantiate(gameObject);
			string text = CheckType();
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			ClearTransform(_weaponObj);
			_objList.Add(_weaponObj);
			if (weaponType == WeaponType.Cannon)
			{
				mainCamera.CopyFrom(cannonCamera);
				_weaponObj.transform.SetParent(cannonHolder, false);
				ClearTransform(_weaponObj);
				AdjustTransform(_weaponObj, cannonHolder.FindChild(BOUNDING_NAME).gameObject);
				_weaponObj.transform.SetParent(null);
				SetCannonMaterial(_weaponObj);
			}
			else if (weaponType == WeaponType.Katana)
			{
				mainCamera.CopyFrom(katanaCamera);
				GameObject gameObject2 = _weaponObj.transform.FindChild(LONG_SWORD_NAME).gameObject;
				GameObject gameObject3 = _weaponObj.transform.FindChild(SHORT_SWORD_NAME).gameObject;
				if (gameObject2 == null || gameObject3 == null)
				{
					return "Sword missing";
				}
				gameObject2.transform.SetParent(longSwordHolder, false);
				ClearTransform(gameObject2);
				if (wholeForKatana)
				{
					AdjustTransform(gameObject2, longSwordHolder.FindChild(BOUNDING_NAME).gameObject);
				}
				gameObject2.transform.SetParent(_weaponObj.transform);
				gameObject3.transform.SetParent(shortSwordHolder, false);
				ClearTransform(gameObject3);
				if (wholeForKatana)
				{
					AdjustTransform(gameObject3, longSwordHolder.FindChild(BOUNDING_NAME).gameObject);
				}
				gameObject3.transform.SetParent(_weaponObj.transform);
				SetKatanaMaterial(_weaponObj);
			}
			else
			{
				mainCamera.CopyFrom(pistolCamera);
				GameObject gameObject4 = _weaponObj.transform.FindChild(LEFT_PISTOL_NAME).gameObject;
				GameObject gameObject5 = _weaponObj.transform.FindChild(RIGHT_PISTOL_NAME).gameObject;
				if (gameObject4 == null || gameObject5 == null)
				{
					return "Missing pistol";
				}
				gameObject4.transform.SetParent(leftPistolHolder, false);
				ClearTransform(gameObject4);
				AdjustTransform(gameObject4, leftPistolHolder.FindChild(BOUNDING_NAME).gameObject);
				gameObject4.transform.SetParent(_weaponObj.transform);
				gameObject5.transform.SetParent(rightPistolHolder, false);
				ClearTransform(gameObject5);
				AdjustTransform(gameObject5, rightPistolHolder.FindChild(BOUNDING_NAME).gameObject);
				gameObject5.transform.SetParent(_weaponObj.transform);
				SetPistolMaterial(_weaponObj);
			}
			fov = mainCamera.fieldOfView;
			return null;
		}

		private string _generate()
		{
			if (_weaponObj == null)
			{
				return "Please load prefab first";
			}
			if (weaponType == WeaponType.Cannon)
			{
				SaveImage(_weaponName);
			}
			else if (weaponType == WeaponType.Katana)
			{
				GameObject gameObject = _weaponObj.transform.FindChild(LONG_SWORD_NAME).gameObject;
				GameObject gameObject2 = _weaponObj.transform.FindChild(SHORT_SWORD_NAME).gameObject;
				if (gameObject == null || gameObject2 == null)
				{
					return "Sword missing";
				}
				gameObject.SetActive(true);
				gameObject2.SetActive(false);
				SaveImage(_weaponName + "_" + LONG_SWORD_NAME);
				gameObject.SetActive(false);
				gameObject2.SetActive(true);
				SaveImage(_weaponName + "_" + SHORT_SWORD_NAME);
				gameObject.SetActive(true);
				gameObject2.SetActive(true);
			}
			else
			{
				GameObject gameObject3 = _weaponObj.transform.FindChild(LEFT_PISTOL_NAME).gameObject;
				GameObject gameObject4 = _weaponObj.transform.FindChild(RIGHT_PISTOL_NAME).gameObject;
				if (gameObject3 == null || gameObject4 == null)
				{
					return "Missing pistol";
				}
				gameObject3.SetActive(true);
				gameObject4.SetActive(false);
				SaveImage(_weaponName + "_" + LEFT_PISTOL_NAME);
				gameObject3.SetActive(false);
				gameObject4.SetActive(true);
				SaveImage(_weaponName + "_" + RIGHT_PISTOL_NAME);
				gameObject3.SetActive(true);
				gameObject4.SetActive(true);
				SaveImage(_weaponName);
			}
			return null;
		}

		private void SetCannonMaterial(GameObject obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					if (material.HasProperty("_ColorTransFactor"))
					{
						material.SetFloat("_ColorTransFactor", 1f);
					}
					if (material.HasProperty("_Opaqueness"))
					{
						material.SetFloat("_Opaqueness", 1f);
					}
				}
			}
		}

		private void SetPistolMaterial(GameObject obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				for (int j = 0; j < renderer.materials.Length; j++)
				{
					renderer.materials[j] = new Material(renderer.materials[j]);
					renderer.materials[j].name += "(Instance)";
				}
			}
		}

		private void SetKatanaMaterial(GameObject obj)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer renderer in componentsInChildren)
			{
				for (int j = 0; j < renderer.materials.Length; j++)
				{
					renderer.materials[j] = new Material(renderer.materials[j]);
					renderer.materials[j].name += "(Instance)";
				}
			}
		}

		private void MakeSingle(GameObject obj, Transform holder, bool needAdjust = false)
		{
			_objList.Add(obj);
			obj.transform.SetParent(holder, false);
			ClearTransform(obj);
			if (needAdjust)
			{
				AdjustTransform(obj, holder.FindChild(BOUNDING_NAME).gameObject);
			}
		}

		private Rect GetBoundsInScreen(GameObject obj, ref float xMinZ)
		{
			Rect result = default(Rect);
			bool flag = true;
			MeshFilter[] componentsInChildren = obj.GetComponentsInChildren<MeshFilter>(true);
			foreach (MeshFilter meshFilter in componentsInChildren)
			{
				Mesh sharedMesh = meshFilter.sharedMesh;
				for (int j = 0; j < sharedMesh.vertexCount; j++)
				{
					Vector3 position = sharedMesh.vertices[j];
					Vector3 vector = mainCamera.WorldToScreenPoint(meshFilter.transform.TransformPoint(position));
					if (flag)
					{
						result.min = vector;
						result.max = vector;
						xMinZ = vector.z;
						flag = false;
						continue;
					}
					if (result.xMin > vector.x)
					{
						result.xMin = vector.x;
						xMinZ = vector.z;
					}
					result.xMax = Mathf.Max(result.xMax, vector.x);
					result.yMin = Mathf.Min(result.yMin, vector.y);
					result.yMax = Mathf.Max(result.yMax, vector.y);
				}
			}
			return result;
		}

		private void AdjustTransform(GameObject obj, GameObject boundObj)
		{
			float xMinZ = 0f;
			Rect boundsInScreen = GetBoundsInScreen(boundObj, ref xMinZ);
			Rect boundsInScreen2 = GetBoundsInScreen(obj, ref xMinZ);
			Vector3 position = new Vector3(boundsInScreen2.xMin, boundsInScreen2.center.y, xMinZ);
			Vector3 position2 = new Vector3(boundsInScreen.xMin, boundsInScreen.center.y, xMinZ);
			obj.transform.position += mainCamera.ScreenToWorldPoint(position2) - mainCamera.ScreenToWorldPoint(position);
			boundsInScreen2 = GetBoundsInScreen(obj, ref xMinZ);
			float num = ((!(boundsInScreen2.width / boundsInScreen2.height < boundsInScreen.width / boundsInScreen.height)) ? (boundsInScreen.width / boundsInScreen2.width) : (boundsInScreen.height / boundsInScreen2.height));
			obj.transform.SetLocalScaleX(num);
			obj.transform.SetLocalScaleY(num);
			obj.transform.SetLocalScaleZ(num);
			boundsInScreen2 = GetBoundsInScreen(obj, ref xMinZ);
			position = new Vector3(boundsInScreen2.xMin, boundsInScreen2.center.y, xMinZ);
			obj.transform.position += mainCamera.ScreenToWorldPoint(position2) - mainCamera.ScreenToWorldPoint(position);
		}

		public void SetFOV(float targetFOV)
		{
			Vector3 position = _weaponObj.transform.position;
			position = mainCamera.worldToCameraMatrix.MultiplyPoint(position);
			float num = Mathf.Tan(mainCamera.fieldOfView * ((float)Math.PI / 180f) * 0.5f);
			float num2 = Mathf.Tan(targetFOV * ((float)Math.PI / 180f) * 0.5f);
			position.z *= num / num2;
			position = mainCamera.cameraToWorldMatrix.MultiplyPoint(position);
			_weaponObj.transform.position = position;
			mainCamera.fieldOfView = targetFOV;
		}

		private string CheckType()
		{
			for (int i = 0; i < TYPE_PREFIX.Length; i++)
			{
				if (_weaponName.StartsWith(TYPE_PREFIX[i]))
				{
					weaponType = (WeaponType)i;
					return null;
				}
			}
			return "Invalid weapon type";
		}

		private static void ClearTransform(GameObject obj)
		{
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.transform.localRotation = Quaternion.identity;
		}

		private IEnumerator _waitForEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
		}

		private static string TrimSuffix(string s, string suffix)
		{
			if (s.EndsWith(suffix))
			{
				return s.Substring(0, s.Length - suffix.Length);
			}
			return s;
		}

		private void SaveImage(string fileName)
		{
			RenderTextureWrapper renderTexture = GraphicsUtils.GetRenderTexture(imageSize, imageSize, 0, RenderTextureFormat.ARGB32);
			mainCamera.targetTexture = renderTexture;
			_postFX.WriteAlpha = true;
			mainCamera.Render();
			_postFX.WriteAlpha = false;
			mainCamera.targetTexture = null;
			RenderTexture.active = renderTexture;
			Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
			texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
			texture2D.Apply();
			byte[] bytes = texture2D.EncodeToPNG();
			File.WriteAllBytes(string.Format("{0}/{1}.png", outputPath, fileName), bytes);
			UnityEngine.Object.Destroy(texture2D);
			GraphicsUtils.ReleaseRenderTexture(renderTexture);
		}
	}
}
