using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoleMole
{
	[ExecuteInEditMode]
	public class MainMenuLight : MonoBehaviour
	{
		[Serializable]
		public class AngleSection
		{
			private static readonly float SECTION_ANGLE_TOLERANCE = 0.1f;

			public float start;

			public float end;

			public float incidentAngle;

			public AngleSection()
			{
			}

			public AngleSection(float start, float end, float incidentAngle)
			{
				this.start = start;
				this.end = end;
				this.incidentAngle = incidentAngle;
			}

			public bool Contain(float angle)
			{
				return angle > start - SECTION_ANGLE_TOLERANCE && angle <= end + SECTION_ANGLE_TOLERANCE;
			}
		}

		public enum Mode
		{
			Limit = 0,
			Fixed = 1,
			Section = 2
		}

		public Mode mode = Mode.Section;

		public bool isLightReferToAvatar;

		[Header("Limit Mode")]
		public Vector2 avatarIncidentAngleRange = new Vector2(-80f, 80f);

		[Header("Fixed Mode")]
		public float fixedAngle;

		[Header("Section Mode")]
		public AngleSection[] sections;

		public AngleSection[] sectionsInterpolate;

		public bool isInterpolate = true;

		public float transitionDuration;

		public float transitionSmooth = 0.3f;

		private float _avatarAngle;

		private float _incidentAngle;

		private float _lastIncidentAngle;

		private float _targetIncidentAngle;

		private int _sectionId;

		private float _incidentAngleTranistionTarget;

		private float _incidentAngleTranistionSpeed;

		private Transform _avatarTrsf;

		public void SetAvatar(Transform avatarTrsf)
		{
			_avatarTrsf = avatarTrsf;
		}

		private void OnEnable()
		{
			CheckSections(ref sections);
			CheckSections(ref sectionsInterpolate);
		}

		private void Update()
		{
			AdjustAvatarIncidentAngle();
		}

		private void GetAvatar()
		{
			if (!(_avatarTrsf != null))
			{
				BaseMonoUIAvatar baseMonoUIAvatar = UnityEngine.Object.FindObjectOfType<BaseMonoUIAvatar>();
				if (baseMonoUIAvatar != null)
				{
					_avatarTrsf = baseMonoUIAvatar.transform;
				}
			}
		}

		private void AdjustAvatarIncidentAngle()
		{
			Transform avatarTrsf = _avatarTrsf;
			GetAvatar();
			Camera main = Camera.main;
			if (_avatarTrsf != null && main != null)
			{
				if (isLightReferToAvatar)
				{
					_incidentAngle = GetIncidentAngle(_avatarTrsf, base.transform.forward);
				}
				else
				{
					_incidentAngle = GetIncidentAngle(main.transform, base.transform.forward);
				}
				if (_avatarTrsf != avatarTrsf)
				{
					_lastIncidentAngle = _incidentAngle;
				}
				RotateInXZ(0f - _incidentAngle);
				if (mode == Mode.Limit)
				{
					_incidentAngleTranistionTarget = Mathf.Clamp(_incidentAngle, avatarIncidentAngleRange.x, avatarIncidentAngleRange.y);
					_targetIncidentAngle = TransitIncidentAngle();
				}
				else if (mode == Mode.Fixed)
				{
					_incidentAngleTranistionTarget = fixedAngle;
					_targetIncidentAngle = TransitIncidentAngle();
				}
				else
				{
					_targetIncidentAngle = IncidentAngleBySection();
				}
				RotateInXZ(_targetIncidentAngle);
				_lastIncidentAngle = _targetIncidentAngle;
			}
		}

		private float GetIncidentAngle(Transform trsf, Vector3 dir)
		{
			dir = trsf.InverseTransformDirection(dir);
			return Mathf.Atan2(0f - dir.x, 0f - dir.z) * 57.29578f;
		}

		private void RotateInXZ(float angle)
		{
			base.transform.Rotate(0f, angle, 0f, Space.World);
		}

		private float IncidentAngleBySection()
		{
			_avatarAngle = GetIncidentAngle(_avatarTrsf, Camera.main.transform.forward);
			if (_avatarAngle < 0f)
			{
				_avatarAngle += 360f;
			}
			if (isInterpolate)
			{
				_sectionId = GetSection(sectionsInterpolate, _avatarAngle);
				AngleSection angleSection = sectionsInterpolate[_sectionId];
				int num = (_sectionId + 1) % sectionsInterpolate.Length;
				float num2 = _avatarAngle - angleSection.start;
				if (num2 > 360f)
				{
					num2 -= 360f;
				}
				return Mathf.LerpAngle(angleSection.incidentAngle, sectionsInterpolate[num].incidentAngle, num2 / (angleSection.end - angleSection.start));
			}
			_sectionId = GetSection(sections, _avatarAngle);
			_incidentAngleTranistionTarget = sections[_sectionId].incidentAngle;
			return TransitIncidentAngle();
		}

		private static void SortSections(AngleSection[] sections)
		{
			for (int i = 0; i < sections.Length - 1; i++)
			{
				for (int j = i + 1; j < sections.Length; j++)
				{
					if (sections[i].start > sections[j].start)
					{
						AngleSection angleSection = sections[i];
						sections[i] = sections[j];
						sections[j] = angleSection;
					}
				}
			}
		}

		private static float RegularAngle(float angle)
		{
			if (Mathf.Approximately(angle, 360f))
			{
				return angle;
			}
			angle = Mathf.DeltaAngle(0f, angle);
			if (angle < 0f)
			{
				angle += 360f;
			}
			return angle;
		}

		private void CheckSections(ref AngleSection[] sections)
		{
			if (sections == null || sections.Length == 0)
			{
				sections = new AngleSection[1];
				sections[0] = new AngleSection(0f, 360f, 0f);
			}
			AngleSection[] array = sections;
			foreach (AngleSection angleSection in array)
			{
				angleSection.start = RegularAngle(angleSection.start);
				angleSection.end = RegularAngle(angleSection.end);
				if (angleSection.start > angleSection.end)
				{
					angleSection.start -= 360f;
				}
			}
			SortSections(sections);
			AngleSection angleSection2 = null;
			List<AngleSection> list = new List<AngleSection>();
			AngleSection[] array2 = sections;
			foreach (AngleSection angleSection3 in array2)
			{
				if (angleSection2 != null && angleSection3.start > angleSection2.end + float.Epsilon)
				{
					list.Add(new AngleSection(angleSection2.end, angleSection3.start, angleSection3.incidentAngle));
				}
				list.Add(angleSection3);
				angleSection2 = angleSection3;
			}
			float num = RegularAngle(list[0].start);
			if (Mathf.Approximately(num, 0f))
			{
				num = 360f;
			}
			if (num > angleSection2.end + 0.001f)
			{
				list.Add(new AngleSection(angleSection2.end, num, angleSection2.incidentAngle));
			}
			sections = list.ToArray();
		}

		private int GetSection(AngleSection[] sections, float angle)
		{
			for (int i = 0; i < sections.Length; i++)
			{
				AngleSection angleSection = sections[i];
				if (angleSection.Contain(angle))
				{
					return i;
				}
			}
			if (sections[0].Contain(angle - 360f))
			{
				return 0;
			}
			SuperDebug.VeryImportantAssert(false, "failed to get section");
			return 0;
		}

		private float TransitIncidentAngle()
		{
			return Mathf.SmoothDampAngle(_lastIncidentAngle, _incidentAngleTranistionTarget, ref _incidentAngleTranistionSpeed, transitionSmooth);
		}
	}
}
