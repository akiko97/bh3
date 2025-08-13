using System.Collections.Generic;
using UnityEngine;
using proto;

namespace MoleMole
{
	public class MonoIslandUICanvas : BaseMonoCanvas
	{
		public PlayerStatusWidgetContext playerBar;

		public IslandMainPageContext mainPageContext;

		public GameObject islandMainPage;

		public MonoIslandBuilding mission;

		public MonoIslandBuilding misc;

		public MonoIslandBuilding collect;

		public MonoIslandBuilding power;

		public MonoIslandBuilding kianaEnhance;

		public MonoIslandBuilding meiEnhance;

		public MonoIslandBuilding bronyaEnhance;

		private Dictionary<MonoIslandBuilding, CabinDataItemBase> buildingDataDict;

		private Transform _blockPanel;

		private void Awake()
		{
			Singleton<MainUIManager>.Instance.SetMainCanvas(this);
			_blockPanel = base.transform.Find("BlockPanel");
		}

		public override void Start()
		{
			buildingDataDict = new Dictionary<MonoIslandBuilding, CabinDataItemBase>();
			buildingDataDict[power] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)1);
			buildingDataDict[collect] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)3);
			buildingDataDict[misc] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)4);
			buildingDataDict[mission] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)5);
			buildingDataDict[kianaEnhance] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)2);
			buildingDataDict[meiEnhance] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)6);
			buildingDataDict[bronyaEnhance] = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType)7);
			SetupBuildings();
			playerBar = new PlayerStatusWidgetContext();
			Singleton<MainUIManager>.Instance.ShowWidget(playerBar);
			CabinDetailPageContext cabinDetailPageContext = new CabinDetailPageContext(buildingDataDict[power], true);
			cabinDetailPageContext.EnableTutorial = false;
			Singleton<MainUIManager>.Instance.ShowPage(cabinDetailPageContext);
			cabinDetailPageContext.Destroy();
			CabinOverviewPageContext cabinOverviewPageContext = new CabinOverviewPageContext(buildingDataDict[power], buildingDataDict);
			cabinOverviewPageContext.EnableTutorial = false;
			Singleton<MainUIManager>.Instance.ShowPage(cabinOverviewPageContext);
			cabinOverviewPageContext.Destroy();
			islandMainPage.SetActive(true);
			mainPageContext = new IslandMainPageContext(islandMainPage, buildingDataDict);
			Singleton<MainUIManager>.Instance.ShowPage(mainPageContext);
			mainPageContext.view.name = "IslandMainPageContext";
			GraphicsSettingData.ApplySettingConfig();
			AudioSettingData.ApplySettingConfig();
			TriggerFullScreenBlock(false);
			Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DestroyLoadingScene));
			base.Start();
		}

		public override void Update()
		{
			base.Update();
		}

		private void SetupBuildings()
		{
			SetupBuilding(power);
			SetupBuilding(mission);
			SetupBuilding(misc);
			SetupBuilding(collect);
			SetupBuilding(kianaEnhance);
			SetupBuilding(meiEnhance);
			SetupBuilding(bronyaEnhance);
		}

		private void SetupBuilding(MonoIslandBuilding building)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Expected I4, but got Unknown
			CabinDataItemBase cabinDataItemBase = buildingDataDict[building];
			int extendGrade = ((cabinDataItemBase.status != CabinStatus.UnLocked) ? 1 : cabinDataItemBase.extendGrade);
			CabinExtendGradeMetaData cabinExtendGradeMetaDataByKey = CabinExtendGradeMetaDataReader.GetCabinExtendGradeMetaDataByKey((int)cabinDataItemBase.cabinType, extendGrade);
			building.UpdateBuildingWhenExtend(cabinExtendGradeMetaDataByKey.buildingPath);
		}

		public CabinDataItemBase GetCabinDataByBuilding(MonoIslandBuilding building)
		{
			return buildingDataDict[building];
		}

		public void TriggerFullScreenBlock(bool enable)
		{
			if (_blockPanel != null && _blockPanel.gameObject != null)
			{
				_blockPanel.gameObject.SetActive(enable);
			}
		}

		public void SetBuildingEffect(MonoIslandBuilding excludeBuilding, bool enable)
		{
			foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> item in buildingDataDict)
			{
				if (item.Key != excludeBuilding)
				{
					if (enable)
					{
						item.Key.GetModel().ToUnMaskedGraphic();
					}
					else
					{
						item.Key.GetModel().ToMaskedGraphic();
					}
				}
			}
		}
	}
}
