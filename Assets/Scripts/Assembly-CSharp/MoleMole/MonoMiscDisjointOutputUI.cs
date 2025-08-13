using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MoleMole
{
	public class MonoMiscDisjointOutputUI : MonoBehaviour
	{
		[SerializeField]
		private Transform itemPrefab;

		private StringBuilder _log = new StringBuilder();

		public void SetupView(StorageDataItemBase input)
		{
			ResetView();
			_log.Length = 0;
			_log.Append(string.Format("[Disjoint] {0} --> ", input.ID));
			CabinDisjointEquipmentMetaData cabinDisjointEquipmentMetaDataByKey = CabinDisjointEquipmentMetaDataReader.GetCabinDisjointEquipmentMetaDataByKey(input.ID);
			foreach (CabinDisjointEquipmentMetaData.CabinDisjointOutputItem item in cabinDisjointEquipmentMetaDataByKey.Item)
			{
				AddItem(item.ID, item.Num);
				_log.Append(string.Format("{0} x{1}, ", item.ID, item.Num));
			}
			ReturnMaterial(input);
		}

		private void AddItem(int id, int num)
		{
			StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id);
			dummyStorageDataItem.number = num;
			Transform transform = base.transform.AddChildFromPrefab(itemPrefab);
			MonoItemIconButton component = transform.GetComponent<MonoItemIconButton>();
			component.SetupView(dummyStorageDataItem);
			component.SetClickCallback(OnItemButonClick);
		}

		private void ReturnMaterial(StorageDataItemBase input)
		{
			WeaponDataItem weaponDataItem = input as WeaponDataItem;
			List<int> evoPath = WeaponMetaDataReaderExtend.GetEvoPath(weaponDataItem.ID);
			int num = 0;
			foreach (int item in evoPath)
			{
				int num2 = 0;
				if (item == weaponDataItem.ID)
				{
					int expType = weaponDataItem.GetExpType();
					int exp = weaponDataItem.exp;
					int accumulateExp = EquipmentLevelMetaDataReaderExtend.GetAccumulateExp(weaponDataItem.level, expType);
					num2 = accumulateExp + exp;
				}
				else
				{
					WeaponMetaData weaponMetaDataByKey = WeaponMetaDataReader.GetWeaponMetaDataByKey(item);
					int expType2 = weaponMetaDataByKey.expType;
					int maxLv = weaponMetaDataByKey.maxLv;
					num2 = EquipmentLevelMetaDataReaderExtend.GetAccumulateExp(maxLv, expType2);
				}
				num += num2;
			}
			float num3 = (float)Singleton<PlayerModule>.Instance.playerData.disjoin_equipment_back_exp_percent * 0.01f;
			int num4 = (int)((float)num * num3);
			int num5 = num4;
			int[] array = new int[4] { 3004, 3003, 3002, 3001 };
			int[] array2 = array;
			foreach (int num6 in array2)
			{
				float gearExpProvideBase = ItemMetaDataReader.GetItemMetaDataByKey(num6).gearExpProvideBase;
				float num7 = MaterialExpBonusMetaDataReader.GetMaterialExpBonusMetaDataByKey(num6).weaponExpBonus * 0.01f;
				int num8 = (int)(gearExpProvideBase * num7);
				int num9 = num5 / num8;
				num5 %= num8;
				if (num9 > 0)
				{
					AddItem(num6, num9);
				}
			}
		}

		private void OnItemButonClick(StorageDataItemBase item, bool selected)
		{
			UIUtil.ShowItemDetail(item, true);
		}

		private void ResetView()
		{
			base.transform.DestroyChildren();
		}
	}
}
