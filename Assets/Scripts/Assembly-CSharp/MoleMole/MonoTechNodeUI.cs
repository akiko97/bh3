using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using proto;
using Material = UnityEngine.Material;

namespace MoleMole
{
	public class MonoTechNodeUI : MonoBehaviour
	{
		private CabinTechTreeNode _data;

		private int _x;

		private int _y;

		[SerializeField]
		private Sprite _grayBG;

		[SerializeField]
		private Sprite _halfBG;

		[SerializeField]
		private Sprite _brightBG;

		[SerializeField]
		private Material _grayMat;

		[SerializeField]
		private ParticleSystem _activationVFX;

		public void Init(CabinTechTreeNode data, int x, int y)
		{
			_data = data;
			if (_data != null)
			{
				_data.RegisterCallback(OnNodeActive);
			}
			_x = x;
			_y = y;
			SetVisible();
		}

		private void OnDestroy()
		{
			if (_data != null)
			{
				_data.UnRegisterCallback();
			}
		}

		public void OnClick()
		{
			if (!Invalid())
			{
				Singleton<MiHoYoGameData>.Instance.LocalData.SetVisited_CabinTechTreeNode(_data._metaData.ID);
				base.transform.Find("New").gameObject.SetActive(false);
				Singleton<MainUIManager>.Instance.ShowDialog(new TechTreeNodeDialogContext(_data));
			}
		}

		private void OnNodeActive()
		{
			if (_activationVFX != null && _activationVFX.gameObject != null)
			{
				Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Unlock_Tech");
				_activationVFX.gameObject.SetActive(true);
				Singleton<ApplicationManager>.Instance.StartCoroutine(StopActiveVFX());
			}
		}

		private IEnumerator StopActiveVFX()
		{
			if (!(_activationVFX == null) && !(_activationVFX.gameObject == null))
			{
				yield return new WaitForSeconds(0.1f);
				_activationVFX.playOnAwake = false;
				yield return new WaitForSeconds(1.9f);
				if (!(_activationVFX == null) && !(_activationVFX.gameObject == null))
				{
					_activationVFX.playOnAwake = true;
					_activationVFX.gameObject.SetActive(false);
				}
			}
		}

		private void SetVisible()
		{
			base.transform.gameObject.SetActive(!Invalid());
		}

		public void RefreshStatus()
		{
			if (Invalid())
			{
				return;
			}
			ClearStatusImage();
			RefreshNew();
			RefreshBorder();
			Refresh_00_Flash();
			base.transform.Find("Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(_data._metaData.Icon);
			switch (_data._status)
			{
			case TechTreeNodeStatus.Active:
				base.transform.Find("BG").GetComponent<Image>().sprite = _brightBG;
				break;
			case TechTreeNodeStatus.Lock:
			{
				base.transform.Find("Lock").gameObject.SetActive(true);
				base.transform.Find("BG").GetComponent<Image>().sprite = _grayBG;
				GrayIcon();
				List<TechTreeNodeLockInfo> lockInfo = _data.GetLockInfo();
				TechTreeNodeLockInfo techTreeNodeLockInfo = lockInfo[0];
				if (techTreeNodeLockInfo._lockType == TechTreeNodeLock.AvatarLevel || techTreeNodeLockInfo._lockType == TechTreeNodeLock.AvatarUnlock)
				{
					Transform transform = base.transform.Find("Lock/Avatar");
					transform.gameObject.SetActive(true);
					transform.GetComponent<Image>().sprite = UIUtil.GetAvatarCardIcon(_data._metaData.UnlockAvatarID);
					Transform transform2 = base.transform.Find("Lock/Level");
					transform2.gameObject.SetActive(true);
					transform2.GetComponent<Text>().text = string.Format("Lv.{0}", techTreeNodeLockInfo._needLevel);
				}
				else if (techTreeNodeLockInfo._lockType == TechTreeNodeLock.CabinLevel)
				{
					Transform transform3 = base.transform.Find("Lock/Cabin");
					transform3.gameObject.SetActive(true);
					string cabinName = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)_data._metaData.Cabin).GetCabinName();
					transform3.GetComponent<Text>().text = cabinName;
					Transform transform4 = base.transform.Find("Lock/Level");
					transform4.gameObject.SetActive(true);
					transform4.GetComponent<Text>().text = string.Format("Lv.{0}", techTreeNodeLockInfo._needLevel);
				}
				break;
			}
			case TechTreeNodeStatus.Unlock_Ready_Active:
				base.transform.Find("BG").GetComponent<Image>().sprite = _halfBG;
				break;
			case TechTreeNodeStatus.Unlock_Ban_Active:
				base.transform.Find("BG").GetComponent<Image>().sprite = _grayBG;
				GrayIcon();
				break;
			}
		}

		private void Refresh_00_Flash()
		{
			if (_data._metaData.X == 0 && _data._metaData.Y == 0)
			{
				bool active = _data._status != TechTreeNodeStatus.Active;
				base.transform.Find("Panel").gameObject.SetActive(active);
			}
		}

		private void RefreshNew()
		{
			bool active = false;
			bool flag = Singleton<MiHoYoGameData>.Instance.LocalData.IsVisited_CabinTechTreeNode(_data._metaData.ID);
			if (_data._status == TechTreeNodeStatus.Active || _data._status == TechTreeNodeStatus.Unlock_Ready_Active)
			{
				active = !flag;
			}
			base.transform.Find("New").gameObject.SetActive(active);
		}

		private void GrayIcon()
		{
			Image component = base.transform.Find("Icon").GetComponent<Image>();
			component.material = _grayMat;
			Color color = component.color;
			color.a = 0.5f;
			component.color = color;
		}

		private void ResetGrayIcon()
		{
			Image component = base.transform.Find("Icon").GetComponent<Image>();
			component.material = null;
			Color color = component.color;
			color.a = 1f;
			component.color = color;
		}

		private bool Invalid()
		{
			return _data == null;
		}

		private void ClearStatusImage()
		{
			ResetGrayIcon();
			base.transform.Find("Lock").gameObject.SetActive(false);
			base.transform.Find("Lock/Cabin").gameObject.SetActive(false);
			base.transform.Find("Lock/Level").gameObject.SetActive(false);
			base.transform.Find("Lock/Avatar").gameObject.SetActive(false);
			base.transform.Find("New").gameObject.SetActive(false);
		}

		private void RefreshBorder()
		{
			SetBorderStyle(1);
			bool flag = _data._status == TechTreeNodeStatus.Active;
			foreach (CabinTechTreeNode neibour in _data.GetNeibours())
			{
				if (neibour._metaData.X > _data._metaData.X)
				{
					base.transform.Find("Border/LineRight/1").gameObject.SetActive(false);
					base.transform.Find("Border/LineRight/2").gameObject.SetActive(!flag);
					base.transform.Find("Border/LineRight/3").gameObject.SetActive(flag);
				}
				else if (neibour._metaData.X < _data._metaData.X)
				{
					base.transform.Find("Border/LineLeft/1").gameObject.SetActive(false);
					base.transform.Find("Border/LineLeft/2").gameObject.SetActive(!flag);
					base.transform.Find("Border/LineLeft/3").gameObject.SetActive(flag);
				}
				else if (neibour._metaData.Y < _data._metaData.Y)
				{
					base.transform.Find("Border/LineTop/1").gameObject.SetActive(false);
					base.transform.Find("Border/LineTop/2").gameObject.SetActive(!flag);
					base.transform.Find("Border/LineTop/3").gameObject.SetActive(flag);
				}
				else if (neibour._metaData.Y > _data._metaData.Y)
				{
					base.transform.Find("Border/LineBottom/1").gameObject.SetActive(false);
					base.transform.Find("Border/LineBottom/2").gameObject.SetActive(!flag);
					base.transform.Find("Border/LineBottom/3").gameObject.SetActive(flag);
				}
			}
		}

		private void SetBorderStyle(int style)
		{
			base.transform.Find("Border/LineBottom/1").gameObject.SetActive(style == 1);
			base.transform.Find("Border/LineBottom/2").gameObject.SetActive(style == 2);
			base.transform.Find("Border/LineBottom/3").gameObject.SetActive(style == 3);
			base.transform.Find("Border/LineRight/1").gameObject.SetActive(style == 1);
			base.transform.Find("Border/LineRight/2").gameObject.SetActive(style == 2);
			base.transform.Find("Border/LineRight/3").gameObject.SetActive(style == 3);
			base.transform.Find("Border/LineTop/1").gameObject.SetActive(style == 1);
			base.transform.Find("Border/LineTop/2").gameObject.SetActive(style == 2);
			base.transform.Find("Border/LineTop/3").gameObject.SetActive(style == 3);
			base.transform.Find("Border/LineLeft/1").gameObject.SetActive(style == 1);
			base.transform.Find("Border/LineLeft/2").gameObject.SetActive(style == 2);
			base.transform.Find("Border/LineLeft/3").gameObject.SetActive(style == 3);
		}

		private void SetActiveBorderStyle()
		{
			SetBorderStyle(2);
			foreach (CabinTechTreeNode neibour in _data.GetNeibours())
			{
				if (neibour._status == TechTreeNodeStatus.Active)
				{
					if (neibour._metaData.X > _data._metaData.X)
					{
						base.transform.Find("Border/LineRight/2").gameObject.SetActive(false);
						base.transform.Find("Border/LineRight/3").gameObject.SetActive(true);
					}
					else if (neibour._metaData.X < _data._metaData.X)
					{
						base.transform.Find("Border/LineLeft/2").gameObject.SetActive(false);
						base.transform.Find("Border/LineLeft/3").gameObject.SetActive(true);
					}
					else if (neibour._metaData.Y < _data._metaData.Y)
					{
						base.transform.Find("Border/LineTop/2").gameObject.SetActive(false);
						base.transform.Find("Border/LineTop/3").gameObject.SetActive(true);
					}
					else if (neibour._metaData.Y > _data._metaData.Y)
					{
						base.transform.Find("Border/LineBottom/2").gameObject.SetActive(false);
						base.transform.Find("Border/LineBottom/3").gameObject.SetActive(true);
					}
				}
			}
		}
	}
}
