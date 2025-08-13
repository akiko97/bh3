using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public static class WeaponAttach
	{
		public delegate Transform[] WeaponAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher avatar, string avatarType);

		public delegate void RuntimeWeaponAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType);

		public delegate void WeaponDetachHandler(ConfigWeaponAttach config, IWeaponAttacher avatar, string avatarType);

		public static void AttachWeaponMesh(ConfigWeapon weaponConfig, IWeaponAttacher avatar, Transform weaponProtoTrans, string avatarType)
		{
			weaponConfig.Attach.GetAttachHandler()(weaponConfig.Attach, weaponProtoTrans, avatar, avatarType);
		}

		private static Transform AttachPartToAttachPoint(IWeaponAttacher avatar, string partPath, string attachPoint, Transform protoTrans)
		{
			Transform attachPoint2 = avatar.GetAttachPoint(attachPoint);
			GameObject gameObject = protoTrans.Find(partPath).gameObject;
			if (gameObject.GetComponent<MeshFilter>() == null)
			{
				attachPoint2.GetComponent<MeshFilter>().sharedMesh = null;
				return attachPoint2;
			}
			attachPoint2.GetComponent<MeshFilter>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			attachPoint2.GetComponent<MeshRenderer>().sharedMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
			return attachPoint2;
		}

		private static void MoveAvatarAttachPoint(IWeaponAttacher avatar, string weaponAttachPointTransPath, string avatarAttachPointName, Transform protoTrans)
		{
			Transform transform = protoTrans.Find(weaponAttachPointTransPath);
			Transform attachPoint = avatar.GetAttachPoint(avatarAttachPointName);
			CopyTransformLocalProperties(transform, attachPoint);
		}

		private static void CopyTransformLocalProperties(Transform from, Transform to)
		{
			to.localPosition = from.localPosition;
			to.localRotation = from.localRotation;
			to.localScale = from.localScale;
		}

		public static void SetTransformParentAndReset(Transform child, Transform parent)
		{
			child.parent = parent;
			child.localRotation = Quaternion.identity;
			child.localPosition = Vector3.zero;
			child.localScale = Vector3.one;
		}

		private static void DetachWeaponAttachPoint(IWeaponAttacher avatar, string attachPoint)
		{
			Transform attachPoint2 = avatar.GetAttachPoint(attachPoint);
			attachPoint2.GetComponent<MeshFilter>().sharedMesh = null;
			attachPoint2.GetComponent<MeshRenderer>().sharedMaterials = new Material[0];
		}

		private static void InstantiateChildrenAndAttach(Transform weaponProtoTrans, Transform weaponTrans, bool onlyMeshChild = true)
		{
			for (int i = 0; i < weaponProtoTrans.childCount; i++)
			{
				GameObject gameObject = weaponProtoTrans.GetChild(i).gameObject;
				if (!onlyMeshChild || !(gameObject.GetComponent<MeshRenderer>() == null))
				{
					GameObject gameObject2 = Object.Instantiate(gameObject);
					gameObject2.transform.SetParent(weaponTrans, false);
					gameObject2.transform.localPosition = gameObject.transform.localPosition;
					gameObject2.transform.localRotation = gameObject.transform.localRotation;
					gameObject2.transform.localScale = gameObject.transform.localScale;
				}
			}
		}

		private static void DeleteAttachPointChildren(IWeaponAttacher avatar, string attachPoint, bool onlyMeshChild = true)
		{
			Transform attachPoint2 = avatar.GetAttachPoint(attachPoint);
			int num = 0;
			while (attachPoint2.childCount > num)
			{
				if (onlyMeshChild && attachPoint2.GetChild(num).GetComponent<MeshRenderer>() == null)
				{
					num++;
				}
				else
				{
					Object.DestroyImmediate(attachPoint2.GetChild(num).gameObject);
				}
			}
		}

		private static void SetRendererLayer(Transform attachTrans, int layer)
		{
			Renderer[] componentsInChildren = attachTrans.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.gameObject.SetLayer(layer);
			}
		}

		public static Transform[] KianaAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher kiana, string avatarType)
		{
			DeleteAttachPointChildren(kiana, "WeaponLeftHand");
			DeleteAttachPointChildren(kiana, "WeaponRightHand");
			Transform[] array = new Transform[2]
			{
				AttachPartToAttachPoint(kiana, "LeftPistol", "WeaponLeftHand", weaponProtoTrans),
				AttachPartToAttachPoint(kiana, "RightPistol", "WeaponRightHand", weaponProtoTrans)
			};
			InstantiateChildrenAndAttach(weaponProtoTrans.Find("LeftPistol"), array[0]);
			InstantiateChildrenAndAttach(weaponProtoTrans.Find("RightPistol"), array[1]);
			MoveAvatarAttachPoint(kiana, "LeftPistol/LeftGunPoint", "LeftGunPoint", weaponProtoTrans);
			MoveAvatarAttachPoint(kiana, "RightPistol/RightGunPoint", "RightGunPoint", weaponProtoTrans);
			return array;
		}

		public static void KianaRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType)
		{
			KianaWeaponAttach kianaWeaponAttach = (KianaWeaponAttach)config.Attach;
			if (!string.IsNullOrEmpty(kianaWeaponAttach.WeaponEffectPattern))
			{
				GameObject gameObject = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(kianaWeaponAttach.WeaponEffectPattern, avatar);
				GameObject gameObject2 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(kianaWeaponAttach.WeaponEffectPattern, avatar);
				SetTransformParentAndReset(gameObject.transform, avatar.GetAttachPoint("LeftGunPoint"));
				SetTransformParentAndReset(gameObject2.transform, avatar.GetAttachPoint("RightGunPoint"));
			}
		}

		public static void KianaDetachHandler(ConfigWeaponAttach config, IWeaponAttacher kiana, string avatarType)
		{
			DetachWeaponAttachPoint(kiana, "WeaponLeftHand");
			DetachWeaponAttachPoint(kiana, "WeaponRightHand");
			DeleteAttachPointChildren(kiana, "WeaponLeftHand");
			DeleteAttachPointChildren(kiana, "WeaponRightHand");
		}

		public static Transform[] MeiAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher mei, string avatarType)
		{
			Transform[] array;
			switch (avatarType)
			{
			case "Mei_C1_DH":
			case "Mei_C3_WS":
				DeleteAttachPointChildren(mei, "WeaponRightHand", false);
				array = new Transform[1] { AttachPartToAttachPoint(mei, "LongSword", "WeaponRightHand", weaponProtoTrans) };
				InstantiateChildrenAndAttach(weaponProtoTrans.Find("LongSword"), array[0], false);
				break;
			case "Mei_C2_CK":
			case "Mei_C4_LD":
				DeleteAttachPointChildren(mei, "WeaponLeftHand", false);
				DeleteAttachPointChildren(mei, "WeaponRightHand", false);
				array = new Transform[2]
				{
					AttachPartToAttachPoint(mei, "ShortSword", "WeaponLeftHand", weaponProtoTrans),
					AttachPartToAttachPoint(mei, "ShortSword", "WeaponRightHand", weaponProtoTrans)
				};
				InstantiateChildrenAndAttach(weaponProtoTrans.Find("ShortSword"), array[0], false);
				InstantiateChildrenAndAttach(weaponProtoTrans.Find("ShortSword"), array[1], false);
				break;
			default:
				array = new Transform[0];
				break;
			}
			return array;
		}

		public static void MeiRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType)
		{
			MeiWeaponAttach meiWeaponAttach = (MeiWeaponAttach)config.Attach;
			if (!string.IsNullOrEmpty(meiWeaponAttach.WeaponEffectPattern))
			{
				switch (avatarType)
				{
				case "Mei_C1_DH":
				case "Mei_C3_WS":
				{
					GameObject gameObject3 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(meiWeaponAttach.WeaponEffectPattern, avatar);
					SetTransformParentAndReset(gameObject3.transform, avatar.GetAttachPoint("WeaponRightHand"));
					break;
				}
				case "Mei_C2_CK":
				case "Mei_C4_LD":
				{
					GameObject gameObject = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(meiWeaponAttach.WeaponEffectPattern, avatar);
					GameObject gameObject2 = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(meiWeaponAttach.WeaponEffectPattern, avatar);
					SetTransformParentAndReset(gameObject.transform, avatar.GetAttachPoint("WeaponLeftHand"));
					SetTransformParentAndReset(gameObject2.transform, avatar.GetAttachPoint("WeaponRightHand"));
					break;
				}
				}
			}
		}

		public static void MeiDetachHandler(ConfigWeaponAttach config, IWeaponAttacher mei, string avatarType)
		{
			switch (avatarType)
			{
			case "Mei_C1_DH":
			case "Mei_C3_WS":
				DetachWeaponAttachPoint(mei, "WeaponRightHand");
				DeleteAttachPointChildren(mei, "WeaponRightHand");
				break;
			case "Mei_C2_CK":
			case "Mei_C4_LD":
				DetachWeaponAttachPoint(mei, "WeaponLeftHand");
				DetachWeaponAttachPoint(mei, "WeaponRightHand");
				DeleteAttachPointChildren(mei, "WeaponLeftHand");
				DeleteAttachPointChildren(mei, "WeaponRightHand");
				break;
			}
		}

		public static Transform[] BronyaAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher bronya, string avatarType)
		{
			Transform[] array = new Transform[2];
			DeleteAttachPointChildren(bronya, "WeaponLeftArmIn");
			DeleteAttachPointChildren(bronya, "WeaponLeftArmOut");
			array[0] = AttachPartToAttachPoint(bronya, "GunIn", "WeaponLeftArmIn", weaponProtoTrans);
			array[1] = AttachPartToAttachPoint(bronya, "GunOut", "WeaponLeftArmOut", weaponProtoTrans);
			InstantiateChildrenAndAttach(weaponProtoTrans.Find("GunIn"), array[0]);
			InstantiateChildrenAndAttach(weaponProtoTrans.Find("GunOut"), array[1]);
			Material[] sharedMaterials = bronya.gameObject.transform.Find("MC_Body").GetComponent<Renderer>().sharedMaterials;
			Material material = null;
			Material[] array2 = sharedMaterials;
			foreach (Material material2 in array2)
			{
				if (material2.shader.name == "miHoYo/Character/Machine")
				{
					material = material2;
					break;
				}
			}
			if (material != null)
			{
				BronyaCopyMcBodyMaterial(material, array[0]);
				BronyaCopyMcBodyMaterial(material, array[1]);
			}
			int layer = LayerMask.NameToLayer("Weapon");
			SetRendererLayer(array[0], layer);
			SetRendererLayer(array[1], layer);
			MoveAvatarAttachPoint(bronya, "GunIn/GunPointAttach", "GunPoint", weaponProtoTrans);
			bronya.gameObject.GetComponent<Animator>().Rebind();
			return array;
		}

		private static void BronyaCopyMcBodyMaterial(Material targetMaterial, Transform attachTrans)
		{
			Color color = targetMaterial.GetColor("_Color");
			Color color2 = targetMaterial.GetColor("_OutlineColor");
			Texture texture = targetMaterial.GetTexture("_SPTex");
			Texture texture2 = targetMaterial.GetTexture("_SPNoiseTex");
			float value = targetMaterial.GetFloat("_SPNoiseScaler");
			float value2 = targetMaterial.GetFloat("_SPIntensity");
			float value3 = targetMaterial.GetFloat("_SPTransition");
			Color color3 = targetMaterial.GetColor("_SPTransitionColor");
			Color color4 = targetMaterial.GetColor("_SPOutlineColor");
			Renderer[] componentsInChildren = attachTrans.GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in componentsInChildren)
			{
				Material[] array = ((!Application.isPlaying) ? renderer.sharedMaterials : renderer.materials);
				Material[] array2 = array;
				foreach (Material material in array2)
				{
					if (material.shader.name == "miHoYo/Character/Machine")
					{
						material.SetColor("_Color", color);
						material.SetColor("_OutlineColor", color2);
						material.SetTexture("_SPTex", texture);
						material.SetTexture("_SPNoiseTex", texture2);
						material.SetFloat("_SPNoiseScaler", value);
						material.SetFloat("_SPIntensity", value2);
						material.SetFloat("_SPTransition", value3);
						material.SetColor("_SPTransitionColor", color3);
						material.SetColor("_SPOutlineColor", color4);
					}
				}
			}
		}

		public static void BronyaRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity avatar, string avatarType)
		{
			BronyaWeaponAttach bronyaWeaponAttach = (BronyaWeaponAttach)config.Attach;
			if (!string.IsNullOrEmpty(bronyaWeaponAttach.WeaponEffectPattern))
			{
				GameObject gameObject = Singleton<EffectManager>.Instance.CreateGroupedEffectPattern(bronyaWeaponAttach.WeaponEffectPattern, avatar);
				SetTransformParentAndReset(gameObject.transform, avatar.GetAttachPoint("GunPoint"));
			}
		}

		public static void BronyaDetachHandler(ConfigWeaponAttach config, IWeaponAttacher bronya, string avatarType)
		{
			DetachWeaponAttachPoint(bronya, "WeaponLeftArmIn");
			DetachWeaponAttachPoint(bronya, "WeaponLeftArmOut");
			DeleteAttachPointChildren(bronya, "WeaponLeftArmIn");
			DeleteAttachPointChildren(bronya, "WeaponLeftArmOut");
		}

		public static Transform[] HimekoAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher himeko, string avatarType)
		{
			DeleteAttachPointChildren(himeko, "WeaponRightHand");
			Transform[] array = new Transform[1] { AttachPartToAttachPoint(himeko, "Sword", "WeaponRightHand", weaponProtoTrans) };
			InstantiateChildrenAndAttach(weaponProtoTrans.Find("Sword"), array[0]);
			return array;
		}

		public static void HimekoRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity himeko, string avatarType)
		{
		}

		public static void HimekoDetachHandler(ConfigWeaponAttach config, IWeaponAttacher himeko, string avatarType)
		{
			DetachWeaponAttachPoint(himeko, "WeaponRightHand");
			DeleteAttachPointChildren(himeko, "WeaponRightHand");
		}

		public static Transform[] FukaAttachHandler(ConfigWeaponAttach config, Transform weaponProtoTrans, IWeaponAttacher fuka, string avatarType)
		{
			DeleteAttachPointChildren(fuka, "WeaponTail");
			Transform[] array = new Transform[1] { AttachPartToAttachPoint(fuka, "Tail", "WeaponTail", weaponProtoTrans) };
			InstantiateChildrenAndAttach(weaponProtoTrans.Find("Tail"), array[0]);
			return array;
		}

		public static void FukaRuntimeAttachHandler(ConfigWeapon config, Transform weaponProtoTrans, BaseMonoAnimatorEntity fuka, string avatarType)
		{
		}

		public static void FukaDetachHandler(ConfigWeaponAttach config, IWeaponAttacher fuka, string avatarType)
		{
			DetachWeaponAttachPoint(fuka, "WeaponTail");
			DeleteAttachPointChildren(fuka, "WeaponTail");
		}
	}
}
