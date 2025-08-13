using System.Collections.Generic;
using MoleMole.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoleMole
{
	public class DetourManager
	{
		private Dictionary<uint, DetourElement> _detours;

		private string _currentLocatorName = string.Empty;

		private int _stageAreaWalkMask;

		private float _disThreshold = 0.01f;

		private float _disReachCornerThreshold = 0.5f;

		private float _getPathDisThreshold = 0.5f;

		private float _getPathTimeThreshold = 0.1f;

		private int _getPathNumPerFrame;

		private int _getPathMaxNumPerFrame = 1;

		private DetourManager()
		{
			_detours = new Dictionary<uint, DetourElement>();
			_stageAreaWalkMask = 1 << NavMesh.GetAreaFromName("Walkable");
		}

		public void RemoveDetourElement(uint id)
		{
			if (_detours.ContainsKey(id))
			{
				_detours.Remove(id);
			}
		}

		public void InitAtAwake()
		{
		}

		public void InitAtStart()
		{
		}

		public void Clear()
		{
			_detours.Clear();
		}

		public void Core()
		{
			_getPathNumPerFrame = 0;
		}

		public void LoadNavMeshRelatedLevel(string stageTypeName)
		{
			StageEntry stageEntryByName = StageData.GetStageEntryByName(stageTypeName);
			string text = stageEntryByName.GetPerpStagePrefabPath().Split('/')[1];
			if (!string.IsNullOrEmpty(_currentLocatorName))
			{
				SceneManager.UnloadScene(_currentLocatorName);
				_currentLocatorName = string.Empty;
			}
			string locatorName = GetLocatorName(stageEntryByName);
			ConfigNavMeshScenePath[] scenePaths = GlobalDataManager.metaConfig.scenePaths;
			foreach (ConfigNavMeshScenePath configNavMeshScenePath in scenePaths)
			{
				if (configNavMeshScenePath.MainSceneName == text && configNavMeshScenePath.UnitySceneName == locatorName)
				{
					SceneManager.LoadScene(configNavMeshScenePath.UnitySceneName, LoadSceneMode.Additive);
					_currentLocatorName = locatorName;
					ResetStageAreaMask(stageEntryByName);
					break;
				}
			}
		}

		private string GetLocatorName(StageEntry newStage)
		{
			string[] array = newStage.LocationPointName.Split('/');
			return array[array.Length - 1];
		}

		public void ResetStageAreaMask(StageEntry newStage)
		{
			string locatorName = GetLocatorName(newStage);
			uint result;
			bool flag = uint.TryParse(locatorName.Substring(locatorName.Length - 2), out result);
			if (!flag)
			{
				flag = uint.TryParse(locatorName.Substring(locatorName.Length - 1), out result);
			}
			int areaFromName = NavMesh.GetAreaFromName("Walkable");
			if (flag)
			{
				areaFromName = NavMesh.GetAreaFromName("StageMask" + result);
			}
			_stageAreaWalkMask = (1 << areaFromName) | (1 << NavMesh.GetAreaFromName("Walkable"));
		}

		public bool GetTargetPosition(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition, ref Vector3 targetCorner)
		{
			if (!Raycast(entity.GetRuntimeID(), sourcePosition, targetPosition))
			{
				targetCorner = targetPosition;
				return true;
			}
			return GetCornerAndCalcPathWhenNeed(entity, sourcePosition, targetPosition, ref targetCorner);
		}

		public bool RandomPosition(Vector3 sourcePosition, float maxDistance, out Vector3 targetPosition)
		{
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				Vector3 sourcePosition2 = sourcePosition + Random.insideUnitSphere * maxDistance;
				NavMeshHit hit;
				if (NavMesh.SamplePosition(sourcePosition2, out hit, 1f, _stageAreaWalkMask))
				{
					targetPosition = hit.position;
					return true;
				}
			}
			targetPosition = sourcePosition;
			return false;
		}

		private bool Raycast(uint id, Vector3 sourcePosition, Vector3 targetPosition)
		{
			NavMeshHit hit;
			bool flag = NavMesh.Raycast(sourcePosition, targetPosition, out hit, _stageAreaWalkMask);
			if (!flag)
			{
				Debug.DrawLine(sourcePosition, targetPosition, Color.red, 0.1f);
			}
			return flag;
		}

		private DetourElement FillDetourElement(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition)
		{
			uint runtimeID = entity.GetRuntimeID();
			DetourElement newDetourElement = GetNewDetourElement(entity, sourcePosition, targetPosition);
			if (newDetourElement == null)
			{
				RemoveDetourElement(runtimeID);
				return null;
			}
			if (_detours.ContainsKey(runtimeID))
			{
				_detours[runtimeID] = newDetourElement;
			}
			else
			{
				_detours.Add(runtimeID, newDetourElement);
			}
			return newDetourElement;
		}

		private DetourElement GetNewDetourElement(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition)
		{
			NavMeshPath navMeshPath = new NavMeshPath();
			bool isCompletePath = NavMesh.CalculatePath(sourcePosition, targetPosition, _stageAreaWalkMask, navMeshPath);
			for (int i = 0; i < navMeshPath.corners.Length - 1; i++)
			{
				Debug.DrawLine(navMeshPath.corners[i], navMeshPath.corners[i + 1], Color.green, 0.1f);
			}
			_getPathNumPerFrame++;
			DetourElement detourElement = new DetourElement();
			detourElement.id = entity.GetRuntimeID();
			detourElement.targetPosition = targetPosition;
			detourElement.isCompletePath = isCompletePath;
			detourElement.lastGetPathTime = Time.time;
			Vector3[] array = SimplifyPath(sourcePosition, navMeshPath);
			if (array.Length == 0)
			{
				return null;
			}
			CapsuleCollider componentInChildren = entity.GetComponentInChildren<CapsuleCollider>();
			if (componentInChildren != null)
			{
				detourElement.disReachCornerThreshold = componentInChildren.radius;
			}
			else
			{
				detourElement.disReachCornerThreshold = _disReachCornerThreshold;
			}
			detourElement.corners = array;
			detourElement.targetCornerIndex = 0u;
			return detourElement;
		}

		private Vector3[] SimplifyPath(Vector3 sourcePosition, NavMeshPath path)
		{
			List<Vector3> list = new List<Vector3>();
			Vector3[] corners = path.corners;
			for (int i = 0; i <= corners.Length - 1; i++)
			{
				Vector3 vector = corners[i] - sourcePosition;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				if (!(magnitude <= _disThreshold))
				{
					list.Add(corners[i]);
				}
			}
			return list.ToArray();
		}

		private bool GetCornerAndCalcPathWhenNeed(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition, ref Vector3 targetCorner)
		{
			uint runtimeID = entity.GetRuntimeID();
			if (_getPathNumPerFrame >= _getPathMaxNumPerFrame)
			{
				return GetTargetCorner(entity, sourcePosition, targetPosition, ref targetCorner);
			}
			DetourElement value;
			_detours.TryGetValue(runtimeID, out value);
			if (value == null)
			{
				DetourElement detourElement = FillDetourElement(entity, sourcePosition, targetPosition);
				if (detourElement != null)
				{
					targetCorner = detourElement.corners[detourElement.targetCornerIndex];
					return true;
				}
				targetCorner = targetPosition;
				return true;
			}
			bool targetCorner2 = GetTargetCorner(entity, sourcePosition, targetPosition, ref targetCorner);
			if (targetCorner2)
			{
				return targetCorner2;
			}
			float time = Time.time;
			if (time - value.lastGetPathTime <= _getPathTimeThreshold)
			{
				return targetCorner2;
			}
			DetourElement detourElement2 = FillDetourElement(entity, sourcePosition, targetPosition);
			if (detourElement2 != null)
			{
				targetCorner = detourElement2.corners[detourElement2.targetCornerIndex];
				return true;
			}
			targetCorner = targetPosition;
			return true;
		}

		private bool GetTargetCorner(BaseMonoEntity entity, Vector3 sourcePosition, Vector3 targetPosition, ref Vector3 targetCorner)
		{
			DetourElement value;
			_detours.TryGetValue(entity.GetRuntimeID(), out value);
			if (value == null)
			{
				return false;
			}
			float num = Miscs.DistancForVec3IgnoreY(targetPosition, value.targetPosition);
			if (num > _getPathDisThreshold)
			{
				_detours.Remove(entity.GetRuntimeID());
				return false;
			}
			float num2 = Miscs.DistancForVec3IgnoreY(value.corners[value.targetCornerIndex], sourcePosition);
			if (num2 <= value.disReachCornerThreshold)
			{
				if (value.targetCornerIndex == value.corners.Length - 1)
				{
					_detours.Remove(entity.GetRuntimeID());
					targetCorner = targetPosition;
					return true;
				}
				value.targetCornerIndex++;
				targetCorner = value.corners[value.targetCornerIndex];
				return true;
			}
			targetCorner = value.corners[value.targetCornerIndex];
			return true;
		}
	}
}
