using UnityEngine;

namespace MoleMole
{
	public class BaseSingleStepParser
	{
		private BaseStepController controler;

		private Transform anchor;

		private BaseStepController.Param _currentParam;

		private Vector3 _lastPosition;

		private Vector3 _lastForward;

		private float _lastEventTimer;

		public bool hasUpdatedThisFrame;

		public BaseStepController.Param param
		{
			get
			{
				return _currentParam;
			}
		}

		public BaseSingleStepParser(BaseStepController controler, Transform anchor)
		{
			this.controler = controler;
			this.anchor = anchor;
			_currentParam = default(BaseStepController.Param);
			_lastPosition = anchor.position;
			_lastForward = anchor.forward;
			_lastEventTimer = 0f;
		}

		public virtual BaseStepController.Param Parse()
		{
			hasUpdatedThisFrame = false;
			_lastEventTimer += Time.deltaTime;
			Matrix4x4 worldToCameraMatrix = Camera.main.worldToCameraMatrix;
			Matrix4x4 worldToCameraMatrix2 = Camera.main.worldToCameraMatrix;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			if (Time.deltaTime > 0f)
			{
				vector = (anchor.position - _lastPosition) / Time.deltaTime;
				Vector3 vector4 = worldToCameraMatrix.MultiplyPoint(anchor.position);
				Vector3 vector5 = worldToCameraMatrix.MultiplyPoint(_lastPosition);
				vector2 = (vector4 - vector5) / Time.deltaTime;
				vector3 = worldToCameraMatrix2.MultiplyVector(vector2);
				vector3.y = 0f;
			}
			if (_lastEventTimer > controler.spanBetweenEvents)
			{
				BaseStepController.Param currentParam = default(BaseStepController.Param);
				Vector3 velocityXZ = vector;
				velocityXZ.y = 0f;
				float anglularSpeed = 0f;
				if (Time.deltaTime > 0f)
				{
					anglularSpeed = Vector3.Angle(_lastForward, anchor.forward) / Time.deltaTime;
				}
				if (anchor.position.y < controler.contactThickness)
				{
					if (_currentParam.pattern == BaseStepController.Pattern.Static || _currentParam.pattern == BaseStepController.Pattern.Void)
					{
						if (velocityXZ.magnitude > controler.horizontalSpeedThreshold)
						{
							currentParam.pattern = BaseStepController.Pattern.Slide;
						}
						else if (vector.y > controler.verticleSpeedThreshold)
						{
							currentParam.pattern = BaseStepController.Pattern.Up;
						}
						else
						{
							currentParam.pattern = BaseStepController.Pattern.Static;
						}
					}
					else if (_currentParam.pattern == BaseStepController.Pattern.Slide)
					{
						if (velocityXZ.magnitude < controler.staticSpeedThreshold)
						{
							currentParam.pattern = BaseStepController.Pattern.Static;
						}
						else
						{
							currentParam.pattern = BaseStepController.Pattern.Slide;
						}
					}
					else if (_currentParam.pattern == BaseStepController.Pattern.Up)
					{
						if (vector.y < controler.staticSpeedThreshold)
						{
							currentParam.pattern = BaseStepController.Pattern.Down;
						}
						else
						{
							currentParam.pattern = BaseStepController.Pattern.Up;
						}
					}
					else if (_currentParam.pattern == BaseStepController.Pattern.Down)
					{
						if (vector.y > controler.verticleSpeedThreshold)
						{
							currentParam.pattern = BaseStepController.Pattern.Up;
						}
						else if (vector.y > 0f - controler.staticSpeedThreshold)
						{
							currentParam.pattern = BaseStepController.Pattern.Static;
						}
						else
						{
							currentParam.pattern = BaseStepController.Pattern.Down;
						}
					}
				}
				else if (_currentParam.pattern == BaseStepController.Pattern.Static || _currentParam.pattern == BaseStepController.Pattern.Void)
				{
					if (vector.y > controler.verticleSpeedThreshold)
					{
						currentParam.pattern = BaseStepController.Pattern.Up;
					}
					else
					{
						currentParam.pattern = BaseStepController.Pattern.Static;
					}
				}
				else if (_currentParam.pattern == BaseStepController.Pattern.Slide)
				{
					currentParam.pattern = BaseStepController.Pattern.Up;
				}
				else if (_currentParam.pattern == BaseStepController.Pattern.Up)
				{
					if (vector.y < 0f - controler.verticleSpeedThreshold)
					{
						currentParam.pattern = BaseStepController.Pattern.Down;
					}
					else
					{
						currentParam.pattern = BaseStepController.Pattern.Up;
					}
				}
				else if (_currentParam.pattern == BaseStepController.Pattern.Down)
				{
					if (vector.y > controler.verticleSpeedThreshold)
					{
						currentParam.pattern = BaseStepController.Pattern.Up;
					}
					else
					{
						currentParam.pattern = BaseStepController.Pattern.Down;
					}
				}
				if (_currentParam.pattern != currentParam.pattern)
				{
					hasUpdatedThisFrame = true;
				}
				currentParam.position = anchor.position;
				currentParam.velocityXZ = velocityXZ;
				currentParam.velocityInCam = vector2;
				float magnitude = _currentParam.velocityXZCorrectSmooth.magnitude;
				if (Vector3.Dot(_currentParam.velocityXZCorrectSmooth, vector3) > 0f)
				{
					float num = Mathf.Min(controler.vectorSmoothFactor, magnitude * 2f / (magnitude + vector3.magnitude));
					currentParam.velocityXZCorrectSmooth = _currentParam.velocityXZCorrectSmooth * num + vector3 * (1f - num);
				}
				else
				{
					currentParam.velocityXZCorrectSmooth = vector3;
				}
				float magnitude2 = _currentParam.velocityInCamSmooth.magnitude;
				float num2 = Mathf.Min(controler.vectorSmoothFactor, magnitude2 * 2f / (magnitude2 + vector2.magnitude));
				currentParam.velocityInCamSmooth = _currentParam.velocityInCamSmooth * num2 + vector2 * (1f - num2);
				currentParam.anglularSpeed = anglularSpeed;
				currentParam.toeForwardXZ = -anchor.right;
				currentParam.toeForwardXZ.y = 0f;
				_currentParam = currentParam;
			}
			_lastPosition = anchor.position;
			_lastForward = anchor.forward;
			return _currentParam;
		}
	}
}
