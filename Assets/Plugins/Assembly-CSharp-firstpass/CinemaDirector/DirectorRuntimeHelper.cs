using System;
using System.Collections.Generic;
using System.Reflection;
using CinemaSuite.Common;
using UnityEngine;

namespace CinemaDirector
{
	public static class DirectorRuntimeHelper
	{
		public static List<Type> GetAllowedTrackTypes(TrackGroup trackGroup)
		{
			TimelineTrackGenre[] array = new TimelineTrackGenre[0];
			TrackGroupAttribute[] customAttributes = ReflectionHelper.GetCustomAttributes<TrackGroupAttribute>(trackGroup.GetType(), true);
			foreach (TrackGroupAttribute trackGroupAttribute in customAttributes)
			{
				if (trackGroupAttribute != null)
				{
					array = trackGroupAttribute.AllowedTrackGenres;
					break;
				}
			}
			List<Type> list = new List<Type>();
			Type[] allSubTypes = GetAllSubTypes(typeof(TimelineTrack));
			foreach (Type type in allSubTypes)
			{
				TimelineTrackAttribute[] customAttributes2 = ReflectionHelper.GetCustomAttributes<TimelineTrackAttribute>(type, true);
				foreach (TimelineTrackAttribute timelineTrackAttribute in customAttributes2)
				{
					if (timelineTrackAttribute == null)
					{
						continue;
					}
					TimelineTrackGenre[] trackGenres = timelineTrackAttribute.TrackGenres;
					foreach (TimelineTrackGenre timelineTrackGenre in trackGenres)
					{
						TimelineTrackGenre[] array2 = array;
						foreach (TimelineTrackGenre timelineTrackGenre2 in array2)
						{
							if (timelineTrackGenre == timelineTrackGenre2)
							{
								list.Add(type);
								break;
							}
						}
					}
					break;
				}
			}
			return list;
		}

		public static List<Type> GetAllowedItemTypes(TimelineTrack timelineTrack)
		{
			CutsceneItemGenre[] array = new CutsceneItemGenre[0];
			TimelineTrackAttribute[] customAttributes = ReflectionHelper.GetCustomAttributes<TimelineTrackAttribute>(timelineTrack.GetType(), true);
			foreach (TimelineTrackAttribute timelineTrackAttribute in customAttributes)
			{
				if (timelineTrackAttribute != null)
				{
					array = timelineTrackAttribute.AllowedItemGenres;
					break;
				}
			}
			List<Type> list = new List<Type>();
			Type[] allSubTypes = GetAllSubTypes(typeof(TimelineItem));
			foreach (Type type in allSubTypes)
			{
				CutsceneItemAttribute[] customAttributes2 = ReflectionHelper.GetCustomAttributes<CutsceneItemAttribute>(type, true);
				foreach (CutsceneItemAttribute cutsceneItemAttribute in customAttributes2)
				{
					if (cutsceneItemAttribute == null)
					{
						continue;
					}
					CutsceneItemGenre[] genres = cutsceneItemAttribute.Genres;
					foreach (CutsceneItemGenre cutsceneItemGenre in genres)
					{
						CutsceneItemGenre[] array2 = array;
						foreach (CutsceneItemGenre cutsceneItemGenre2 in array2)
						{
							if (cutsceneItemGenre == cutsceneItemGenre2)
							{
								list.Add(type);
								break;
							}
						}
					}
					break;
				}
			}
			return list;
		}

		private static Type[] GetAllSubTypes(Type ParentType)
		{
			List<Type> list = new List<Type>();
			Assembly[] assemblies = ReflectionHelper.GetAssemblies();
			foreach (Assembly assembly in assemblies)
			{
				Type[] types = ReflectionHelper.GetTypes(assembly);
				foreach (Type type in types)
				{
					if (type != null && ReflectionHelper.IsSubclassOf(type, ParentType))
					{
						list.Add(type);
					}
				}
			}
			return list.ToArray();
		}

		public static List<Transform> GetAllTransformsInHierarchy(Transform parent)
		{
			List<Transform> list = new List<Transform>();
			foreach (Transform item in parent)
			{
				list.AddRange(GetAllTransformsInHierarchy(item));
				list.Add(item);
			}
			return list;
		}
	}
}
