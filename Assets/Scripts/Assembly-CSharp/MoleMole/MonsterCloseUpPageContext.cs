using MoleMole.Config;
using UnityEngine;

namespace MoleMole
{
	public class MonsterCloseUpPageContext : BasePageContext
	{
		private const string IN_LEVEL_CLOSE_UP_PREFAB_PREFIX = "UI/InLevelCloseUp/CloseUp_";

		public readonly string monsterName;

		public MonsterCloseUpPageContext(string monsterName)
		{
			config = new ContextPattern
			{
				contextName = "CloseUpPageContext",
				viewPrefabPath = "UI/Menus/Page/InLevel/MonsterCloseUpPage"
			};
			this.monsterName = monsterName;
		}

		public override bool OnNotify(Notify ntf)
		{
			if (ntf.type == NotifyTypes.MonsterCloseUpEnd)
			{
				Singleton<MainUIManager>.Instance.BackPage();
			}
			return false;
		}

		protected override void BindViewCallbacks()
		{
		}

		protected override bool SetupView()
		{
			string path = "UI/InLevelCloseUp/CloseUp_" + monsterName;
			base.view.transform.DestroyChildren();
			GameObject gameObject = Object.Instantiate(Miscs.LoadResource<GameObject>(path));
			gameObject.transform.SetParent(base.view.transform, false);
			Animation component = gameObject.GetComponent<Animation>();
			component.Play();
			return false;
		}
	}
}
