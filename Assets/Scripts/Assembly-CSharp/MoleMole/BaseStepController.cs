using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[DisallowMultipleComponent]
	public class BaseStepController : MonoBehaviour
	{
		public enum Pattern
		{
			Void = 0,
			Static = 1,
			Down = 2,
			Up = 3,
			Slide = 4
		}

		public struct Param
		{
			public Pattern pattern;

			public Vector3 position;

			public Vector3 velocityXZ;

			public Vector3 velocityXZCorrectSmooth;

			public Vector3 velocityInCam;

			public Vector3 velocityInCamSmooth;

			public Vector3 toeForwardXZ;

			public float acceleration;

			public float anglularSpeed;

			public float cameraPitchAngle;

			public override string ToString()
			{
				return pattern.ToString();
			}
		}

		public Transform leftFootAnchor;

		public Transform rightFootAnchor;

		[Header("Foot width and height in world space")]
		public float footWidth;

		public float footLength;

		[Header("The ration of distance between foot anchor and front point to that of back point")]
		public float frontBackRatio = 1f;

		[Header("When the distance to ground is less than this thickness, consider it to be in contact")]
		public float contactThickness = 0.1f;

		[Header("The max number of state to hold in queue")]
		public int maxStateToHold = 10;

		[Header("How much smooth the smoothed velocity be?")]
		[Range(0f, 1f)]
		public float vectorSmoothFactor;

		[Header("The threshold of horizontal speed to trigger movement")]
		public float horizontalSpeedThreshold = 5f;

		[Header("The threshold of vertical speed to trigger movement")]
		public float verticleSpeedThreshold = 2f;

		[Header("The threshold of speed to trigger static")]
		public float staticSpeedThreshold = 1f;

		[Header("The threshold of angle speed to trigger event")]
		public float angleSpeedThreshold = 360f;

		[Header("The threshold of angle speed to trigger static")]
		public float staticAngleSpeedThreshold = 90f;

		[Header("Span between two events")]
		public float spanBetweenEvents = 0.5f;

		public float slideRatioThreshold = 5f;

		public Action<Param> onStepEvent;

		private BaseSingleStepParser _leftStepParser;

		private BaseSingleStepParser _rightStepParser;

		private Queue<Param> _leftStepParamQueue = new Queue<Param>();

		private Queue<Param> _rightStepParamQueue = new Queue<Param>();

		public Param leftStepParam
		{
			get
			{
				return _leftStepParser.param;
			}
		}

		public Param rightStepParam
		{
			get
			{
				return _rightStepParser.param;
			}
		}

		public Param currentLeftStepParam
		{
			get
			{
				return _leftStepParser.param;
			}
		}

		public Param currentRightStepParam
		{
			get
			{
				return _rightStepParser.param;
			}
		}

		public Queue<Param> leftStepParamQueue
		{
			get
			{
				return _leftStepParamQueue;
			}
		}

		public Queue<Param> rightStepParamQueue
		{
			get
			{
				return _rightStepParamQueue;
			}
		}

		protected virtual void Awake()
		{
			BaseMonoAvatar component = GetComponent<BaseMonoAvatar>();
			GetFootAnchorFromAvatar(ref leftFootAnchor, component, "LeftFoot");
			GetFootAnchorFromAvatar(ref rightFootAnchor, component, "RightFoot");
			_leftStepParser = new BaseSingleStepParser(this, leftFootAnchor);
			_rightStepParser = new BaseSingleStepParser(this, rightFootAnchor);
		}

		protected virtual void FixedUpdate()
		{
			_leftStepParser.Parse();
			_rightStepParser.Parse();
			if (_leftStepParser.hasUpdatedThisFrame)
			{
				_leftStepParamQueue.Enqueue(_leftStepParser.param);
			}
			if (_rightStepParser.hasUpdatedThisFrame)
			{
				_rightStepParamQueue.Enqueue(_rightStepParser.param);
			}
		}

		protected virtual void Update()
		{
			CleanParamsOutOfDate();
		}

		private void GetFootAnchorFromAvatar(ref Transform anchor, BaseMonoAvatar avatar, string name)
		{
			if (anchor == null && avatar != null)
			{
				anchor = avatar.GetAttachPoint(name);
			}
			if (anchor == null)
			{
				base.enabled = false;
			}
		}

		private void CleanParamsOutOfDate()
		{
			while (_leftStepParamQueue.Count > maxStateToHold)
			{
				_leftStepParamQueue.Dequeue();
			}
			while (_rightStepParamQueue.Count > maxStateToHold)
			{
				_rightStepParamQueue.Dequeue();
			}
		}
	}
}
