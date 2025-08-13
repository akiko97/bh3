using System.Collections;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarPromotionDialogContext : BaseDialogContext
	{
		private AvatarDataItem avatarData;

		private ParticleSystem _starVFX;

		private Animator _animator;

		private bool _ignoreButton = true;

		public AvatarPromotionDialogContext(AvatarDataItem avatarData)
		{
			config = new ContextPattern
			{
				contextName = "AvatarPromotionDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarPromotionDialog"
			};
			this.avatarData = avatarData;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.AnimCallBack)
			{
				return OnAnimCallBack((string)ntf.body);
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), Resume);
		}

		protected override bool SetupView()
		{
			_ignoreButton = true;
			_animator = base.view.GetComponent<Animator>();
			_animator.SetTrigger("Play");
			_animator.speed = 1f;
			AvatarStarMetaData avatarStarMetaDataByKey = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(avatarData.avatarID, avatarData.star - 1);
			AvatarStarMetaData avatarStarMetaDataByKey2 = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(avatarData.avatarID, avatarData.star);
			base.view.transform.Find("Dialog/Content/HP/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.hpAdd);
			base.view.transform.Find("Dialog/Content/HP/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey2.hpAdd);
			base.view.transform.Find("Dialog/Content/HP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.hpBase - avatarStarMetaDataByKey.hpBase + (avatarStarMetaDataByKey2.hpAdd - avatarStarMetaDataByKey.hpAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/SP/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.spAdd);
			base.view.transform.Find("Dialog/Content/SP/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey2.spAdd);
			base.view.transform.Find("Dialog/Content/SP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.spBase - avatarStarMetaDataByKey.spBase + (avatarStarMetaDataByKey2.spAdd - avatarStarMetaDataByKey.spAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/ATK/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.atkAdd);
			base.view.transform.Find("Dialog/Content/ATK/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey2.atkAdd);
			base.view.transform.Find("Dialog/Content/ATK/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.atkBase - avatarStarMetaDataByKey.atkBase + (avatarStarMetaDataByKey2.atkAdd - avatarStarMetaDataByKey.atkAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/DEF/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.dfsAdd);
			base.view.transform.Find("Dialog/Content/DEF/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey2.dfsAdd);
			base.view.transform.Find("Dialog/Content/DEF/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.dfsBase - avatarStarMetaDataByKey.dfsBase + (avatarStarMetaDataByKey2.dfsAdd - avatarStarMetaDataByKey.dfsAdd) * (float)avatarData.level);
			base.view.transform.Find("Dialog/Content/CRT/RatioBeforeNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey.crtAdd);
			base.view.transform.Find("Dialog/Content/CRT/RatioAfterNumText").GetComponent<Text>().text = string.Format("{0:N2}", avatarStarMetaDataByKey2.crtAdd);
			base.view.transform.Find("Dialog/Content/CRT/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", avatarStarMetaDataByKey2.crtBase - avatarStarMetaDataByKey.crtBase + (avatarStarMetaDataByKey2.crtAdd - avatarStarMetaDataByKey.crtAdd) * (float)avatarData.level);
			int cost = avatarData.GetCost(avatarData.star - 1);
			int maxCost = avatarData.MaxCost;
			base.view.transform.Find("Dialog/Content/COST/RatioBeforeNumText").GetComponent<Text>().text = cost.ToString();
			base.view.transform.Find("Dialog/Content/COST/RatioAfterNumText").GetComponent<Text>().text = maxCost.ToString();
			base.view.transform.Find("Dialog/Content/COST/AddNumText").GetComponent<Text>().text = string.Format("+{0}", maxCost - cost);
			SetupStar();
			return false;
		}

		public void Close()
		{
			Destroy();
		}

		private void SetupStar()
		{
			_starVFX = base.view.transform.Find("Dialog/Content/Star/StarShining").GetComponent<ParticleSystem>();
			base.view.transform.Find("Dialog/Content/Star/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[avatarData.star - 1]);
		}

		private bool OnAnimCallBack(string param)
		{
			if (param == "PromotionDialogPause")
			{
				_animator.speed = 0f;
				_ignoreButton = false;
			}
			else if (param == "StarVFX")
			{
				base.view.transform.Find("Dialog/Content/Star/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[avatarData.star]);
				_starVFX.Play();
			}
			return false;
		}

		private void Resume()
		{
			if (!_ignoreButton)
			{
				_animator.speed = 1f;
				Singleton<ApplicationManager>.Instance.StartCoroutine(DelayDestroy(1f));
				_ignoreButton = true;
			}
		}

		private IEnumerator DelayDestroy(float delay)
		{
			yield return new WaitForSeconds(delay);
			Close();
		}
	}
}
