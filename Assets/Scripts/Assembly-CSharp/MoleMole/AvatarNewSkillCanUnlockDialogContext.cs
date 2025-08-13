using MoleMole.Config;
using UnityEngine;
using UnityEngine.UI;

namespace MoleMole
{
	public class AvatarNewSkillCanUnlockDialogContext : BaseSequenceDialogContext
	{
		private const float TIMER_SPAN = 2f;

		private const string UNLOCK_NEW_SKILL_TEXT_MAP_ID = "Menu_Desc_UnlockNewSkill";

		private const string CAN_UNLOCK_NEW_SUB_SKILL_TEXT_MAP_ID = "Menu_Desc_CanUnlockNewSubSkill";

		public readonly string avatarFullName;

		public readonly string skillName;

		public readonly bool isSubSkill;

		private CanvasTimer _timer;

		public AvatarNewSkillCanUnlockDialogContext(string avatarFullName, string skillName, bool isSubSkill)
		{
			config = new ContextPattern
			{
				contextName = "AvatarNewSkillCanUnlockDialogContext",
				viewPrefabPath = "UI/Menus/Dialog/AvatarNewSkillCanUnlockDialog"
			};
			this.avatarFullName = avatarFullName;
			this.skillName = skillName;
			this.isSubSkill = isSubSkill;
			_timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(2f, 0f);
			_timer.timeUpCallback = OnTimerUp;
			_timer.StopRun();
		}

		protected override void BindViewCallbacks()
		{
			BindViewCallback(base.view.transform.Find("Btn").GetComponent<Button>(), OnBGClick);
		}

		protected override bool SetupView()
		{
			base.view.transform.Find("Btn").gameObject.SetActive(false);
			base.view.transform.Find("Dialog/Content/NewSkill/AvatarName").GetComponent<Text>().text = avatarFullName;
			base.view.transform.Find("Dialog/Content/NewSkill/SkillName").GetComponent<Text>().text = skillName;
			base.view.transform.Find("Dialog/Content/NewSkill/CanUnlockLabel").GetComponent<Text>().text = LocalizationGeneralLogic.GetText((!isSubSkill) ? "Menu_Desc_UnlockNewSkill" : "Menu_Desc_CanUnlockNewSubSkill");
			base.view.transform.Find("Dialog").GetComponent<MonoDialogHeightGrow>().PlayGrow(OnDialogBGGrowEnd);
			return false;
		}

		private void OnBGClick()
		{
			Destroy();
		}

		public override void Destroy()
		{
			_timer.Destroy();
			base.Destroy();
		}

		private void OnDialogBGGrowEnd()
		{
			base.view.transform.Find("Dialog/Content/NewSkill").GetComponent<Animation>().Play();
			base.view.transform.Find("Btn").gameObject.SetActive(true);
			Startimer();
		}

		private void Startimer()
		{
			_timer.StartRun();
		}

		private void OnTimerUp()
		{
			Destroy();
		}
	}
}
