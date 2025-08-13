using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	public class MonoWall : BaseMonoDynamicObject
	{
		private class CollisionEntry
		{
			public Collider collider;

			public float timer;

			public int patternIx;

			public List<MonoEffect> effectLs;
		}

		public const float FADE_DURATION = 0.5f;

		private bool _isToBeRemoved;

		private LayerMask _collisionMask;

		[Header("Collided entity will have this effect pattern on the wall.")]
		public string CollisionEffectPattern;

		private List<CollisionEntry> _collisions = new List<CollisionEntry>();

		public override bool IsActive()
		{
			return !_isToBeRemoved;
		}

		public override bool IsToBeRemove()
		{
			return _isToBeRemoved;
		}

		public void SetCollisionMask(LayerMask mask)
		{
			_collisionMask = mask;
		}

		public override void SetDied()
		{
			base.SetDied();
			_isToBeRemoved = true;
			Singleton<EffectManager>.Instance.ClearEffectsByOwner(_runtimeID);
		}

		protected override void Update()
		{
			base.Update();
			for (int i = 0; i < _collisions.Count; i++)
			{
				CollisionEntry collisionEntry = _collisions[i];
				if (collisionEntry == null)
				{
					continue;
				}
				collisionEntry.timer -= Time.deltaTime * TimeScale;
				if (collisionEntry.collider == null || collisionEntry.timer <= 0f)
				{
					Singleton<EffectManager>.Instance.SetDestroyIndexedEffectPattern(collisionEntry.patternIx);
					_collisions[i] = null;
					continue;
				}
				for (int j = 0; j < collisionEntry.effectLs.Count; j++)
				{
					collisionEntry.effectLs[j].transform.position = GetCollisoinPointOnWall(collisionEntry.collider.transform);
					collisionEntry.effectLs[j].transform.forward = base.transform.forward;
				}
			}
		}

		private Vector3 GetCollisoinPointOnWall(Transform targetTransform)
		{
			Vector3 position = targetTransform.position;
			Vector3 vector = position - base.transform.position;
			vector.y = 0f;
			Vector3 vector2 = Vector3.Project(vector, base.transform.right);
			Vector3 result = base.transform.position + vector2;
			result.y = 0f;
			return result;
		}

		private void CreateOrRefreshCollisionEntry(Collision collision)
		{
			for (int i = 0; i < _collisions.Count; i++)
			{
				CollisionEntry collisionEntry = _collisions[i];
				if (collisionEntry != null && collisionEntry.collider == collision.collider)
				{
					collisionEntry.timer = 0.5f;
					return;
				}
			}
			int patternIx = Singleton<EffectManager>.Instance.CreateIndexedEntityEffectPattern(CollisionEffectPattern, GetCollisoinPointOnWall(collision.collider.transform), base.transform.forward, Vector3.one, this);
			List<MonoEffect> indexedEntityEffectPattern = Singleton<EffectManager>.Instance.GetIndexedEntityEffectPattern(patternIx);
			int index = _collisions.SeekAddPosition();
			_collisions[index] = new CollisionEntry
			{
				collider = collision.collider,
				timer = 0.5f,
				patternIx = patternIx,
				effectLs = indexedEntityEffectPattern
			};
		}

		private void OnCollisionEnter(Collision collision)
		{
			if ((_collisionMask.value & (1 << collision.gameObject.layer)) != 0)
			{
				CreateOrRefreshCollisionEntry(collision);
			}
		}

		private void OnCollisionStay(Collision collision)
		{
			if ((_collisionMask.value & (1 << collision.gameObject.layer)) != 0)
			{
				CreateOrRefreshCollisionEntry(collision);
			}
		}

		private void OnCollisionExit(Collision collision)
		{
			if ((_collisionMask.value & (1 << collision.gameObject.layer)) == 0)
			{
				return;
			}
			for (int i = 0; i < _collisions.Count; i++)
			{
				if (_collisions[i] != null && _collisions[i].collider == collision.collider)
				{
					_collisions[i].timer = 0f;
				}
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (Singleton<EffectManager>.Instance == null)
			{
				return;
			}
			for (int i = 0; i < _collisions.Count; i++)
			{
				if (_collisions[i] != null)
				{
					Singleton<EffectManager>.Instance.SetDestroyImmediatelyIndexedEffectPattern(_collisions[i].patternIx);
				}
			}
		}
	}
}
