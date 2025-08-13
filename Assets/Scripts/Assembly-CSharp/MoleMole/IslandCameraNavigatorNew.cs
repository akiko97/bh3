using System;
using UnityEngine;

namespace MoleMole
{
	public class IslandCameraNavigatorNew : MonoBehaviour
	{
		public float ParallexRange = 2.5f;

		public float ParallexSensitivity = 0.05f;

		public float ParallexBoundHardness = 0.5f;

		public float _gyroSpeed = 5f;

		public float _gyroVerticleRadius = 5f;

		public float _gyroHorizonRadius = 10f;

		public float _verticleAngleLimit = 30f;

		public float _horizonAngleLimit = 30f;

		public float _lerpOffsetZoomInDuration = 0.5f;

		public float _lerpOffsetZoomOutDuration = 1f;

		private Quaternion baseAttitude;

		private Vector3 _gyroOffset = Vector3.zero;

		private MonoIslandCameraSM _cameraSM;

		private Quaternion _gyroRot = Quaternion.identity;

		private Quaternion referenceAttitude = Quaternion.identity;

		private float _xOffset;

		private float _yOffset;

		private bool _bDir = true;

		private Vector3 _orentation;

		private bool _bEnable;

		private bool _bIsLerpingOffset;

		private float _startTimeLerp;

		private Vector3 _offsetWhenLerping;

		private Vector3 _vLerpFrom;

		private Vector3 _vLerpTo;

		private void Start()
		{
			Input.gyro.enabled = GraphicsSettingData.IsEnableGyroscope();
			_bEnable = true;
			_cameraSM = GetComponent<MonoIslandCameraSM>();
			baseAttitude = GetInputGyroAttitude();
			referenceAttitude = base.transform.rotation;
			_gyroRot = base.transform.rotation;
		}

		private Quaternion GetInputGyroAttitude()
		{
			return Input.gyro.attitude;
		}

		public void SetEnable(bool enable)
		{
			_bEnable = enable;
			_bIsLerpingOffset = true;
			_startTimeLerp = Time.time;
			if (!enable)
			{
				_vLerpFrom = _gyroOffset;
				_vLerpTo = Vector3.zero;
			}
		}

		public Vector3 GetOffset()
		{
			if (_bIsLerpingOffset)
			{
				float num = ((!_bEnable) ? _lerpOffsetZoomInDuration : _lerpOffsetZoomOutDuration);
				float num2 = (Time.time - _startTimeLerp) / num;
				if (num2 >= 1f)
				{
					_bIsLerpingOffset = false;
					if (_bEnable)
					{
						_offsetWhenLerping = _gyroOffset;
					}
					else
					{
						_offsetWhenLerping = _vLerpTo;
						_gyroOffset = _vLerpTo;
					}
				}
				else
				{
					float t = 2f * num2 - num2 * num2;
					if (_bEnable)
					{
						_offsetWhenLerping = Vector3.Lerp(Vector3.zero, _gyroOffset, t);
					}
					else
					{
						_offsetWhenLerping = Vector3.Lerp(_vLerpFrom, _vLerpTo, t);
					}
				}
				return _offsetWhenLerping;
			}
			return _gyroOffset;
		}

		private void FixedUpdate()
		{
			if (_bEnable)
			{
				Quaternion quaternion = referenceAttitude * ConvertRotation(Quaternion.Inverse(baseAttitude) * GetInputGyroAttitude());
				_orentation = CheckAulerAngle((Quaternion.Inverse(_gyroRot) * quaternion).eulerAngles);
				_yOffset = Mathf.Sin(_orentation.x * ((float)Math.PI / 180f)) * _gyroVerticleRadius;
				_xOffset = Mathf.Sin(_orentation.y * ((float)Math.PI / 180f)) * _gyroHorizonRadius;
				_gyroOffset = _cameraSM.GetCameraBasePos().up * _yOffset;
				_gyroOffset += _cameraSM.GetCameraBasePos().right * _xOffset;
				if (Quaternion.Angle(GetInputGyroAttitude(), baseAttitude) > ParallexRange)
				{
					baseAttitude = Quaternion.Slerp(baseAttitude, GetInputGyroAttitude(), ParallexBoundHardness);
				}
			}
		}

		private Vector3 CheckAulerAngle(Vector3 vInput)
		{
			Vector3 result = vInput;
			if (result.x < 180f)
			{
				result.x = Mathf.Clamp(result.x, 0f, _verticleAngleLimit);
			}
			else
			{
				result.x = Mathf.Clamp(result.x, 360f - _verticleAngleLimit, 360f);
			}
			if (result.y < 180f)
			{
				result.y = Mathf.Clamp(result.y, 0f, _horizonAngleLimit);
			}
			else
			{
				result.y = Mathf.Clamp(result.y, 360f - _horizonAngleLimit, 360f);
			}
			return result;
		}

		private static Quaternion ConvertRotation(Quaternion q)
		{
			return new Quaternion(q.x, q.y, 0f - q.z, 0f - q.w);
		}

		private void Simulate()
		{
			float num = ((!_bDir) ? (0f - _gyroSpeed) : _gyroSpeed);
			Vector3 vector = _cameraSM.GetCameraBasePos().up * num * Time.deltaTime;
			if ((_gyroOffset + vector).magnitude > _gyroVerticleRadius)
			{
				_bDir = !_bDir;
			}
			else
			{
				_gyroOffset += vector;
			}
		}
	}
}
