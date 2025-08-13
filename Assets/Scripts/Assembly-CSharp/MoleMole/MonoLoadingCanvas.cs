namespace MoleMole
{
	public class MonoLoadingCanvas : BaseMonoCanvas
	{
		public void Awake()
		{
			Singleton<MainUIManager>.Instance.SetMainCanvas(this);
		}

		public override void Start()
		{
			LoadingPageContext context = new LoadingPageContext(Singleton<MainUIManager>.Instance.bDestroyUntilNotify);
			Singleton<MainUIManager>.Instance.ShowPage(context);
			base.Start();
		}

		public override void PlayVideo(CgDataItem cgDataItem)
		{
		}

		public void OnInLevelVideoBeginCallback(CgDataItem cgDataItem)
		{
		}

		public void OnInLevelVideoEndCallback(CgDataItem cgDataItem)
		{
		}
	}
}
