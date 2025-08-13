using System;
using System.Collections.Generic;
using FullInspector;

namespace MoleMole
{
	public abstract class BasePluggedActor : BaseActor
	{
		private Dictionary<Type, BaseActorPlugin> _pluginMap = new Dictionary<Type, BaseActorPlugin>();

		[ShowInInspector]
		private List<BaseActorPlugin> _pluginList = new List<BaseActorPlugin>();

		public Func<BaseEvent, bool> rejectBaseEventHandlingPredicate;

		public bool HasPlugin<T>() where T : BaseActorPlugin
		{
			return _pluginMap.ContainsKey(typeof(T));
		}

		public T GetPlugin<T>() where T : BaseActorPlugin
		{
			BaseActorPlugin value;
			_pluginMap.TryGetValue(typeof(T), out value);
			return (T)value;
		}

		public T2 GetPluginAs<T1, T2>() where T1 : BaseActorPlugin where T2 : T1
		{
			return (T2)GetPlugin<T1>();
		}

		public void AddPluginAs<T>(BaseActorPlugin plugin) where T : BaseActorPlugin
		{
			AddPlugin(plugin, typeof(T));
		}

		public void AddPlugin(BaseActorPlugin plugin)
		{
			Type type = plugin.GetType();
			AddPlugin(plugin, type);
		}

		private void AddPlugin(BaseActorPlugin plugin, Type type)
		{
			if (!_pluginMap.ContainsKey(type))
			{
				_pluginMap.Add(type, plugin);
				_pluginList.Add(plugin);
				plugin.OnAdded();
			}
		}

		public void RemovePlugin(Type type)
		{
			if (_pluginMap.ContainsKey(type))
			{
				BaseActorPlugin baseActorPlugin = _pluginMap[type];
				_pluginMap.Remove(type);
				_pluginList.Remove(baseActorPlugin);
				baseActorPlugin.OnRemoved();
			}
		}

		public void RemovePlugin<T>()
		{
			RemovePlugin(typeof(T));
		}

		public void RemovePlugin(BaseActorPlugin plugin)
		{
			RemovePlugin(plugin.GetType());
		}

		private bool OnPluginPreEvent(BaseEvent evt)
		{
			bool flag = false;
			for (int i = 0; i < _pluginList.Count; i++)
			{
				flag |= _pluginList[i].OnEvent(evt);
			}
			return flag;
		}

		private bool OnPluginPostEvent(BaseEvent evt)
		{
			bool flag = false;
			for (int i = 0; i < _pluginList.Count; i++)
			{
				flag |= _pluginList[i].OnPostEvent(evt);
			}
			return flag;
		}

		private bool OnPluginResolvedEvent(BaseEvent evt)
		{
			bool flag = false;
			for (int i = 0; i < _pluginList.Count; i++)
			{
				flag |= _pluginList[i].OnResolvedEvent(evt);
			}
			return flag;
		}

		public sealed override bool OnEvent(BaseEvent evt)
		{
			bool flag = rejectBaseEventHandlingPredicate != null && rejectBaseEventHandlingPredicate(evt);
			bool flag2 = false;
			flag2 |= OnPluginPreEvent(evt);
			if (!flag)
			{
				flag2 |= OnEventWithPlugins(evt);
			}
			flag2 |= OnPluginPostEvent(evt);
			if (flag || !flag2 || evt.requireResolve)
			{
			}
			if (!flag)
			{
				flag2 |= OnEventResolves(evt);
			}
			if (flag || !flag2 || evt.requireResolve)
			{
			}
			return flag2 | OnPluginResolvedEvent(evt);
		}

		public virtual bool OnEventWithPlugins(BaseEvent evt)
		{
			return false;
		}

		public virtual bool OnEventResolves(BaseEvent evt)
		{
			return false;
		}

		public override bool ListenEvent(BaseEvent evt)
		{
			bool flag = false;
			for (int i = 0; i < _pluginList.Count; i++)
			{
				flag |= _pluginList[i].ListenEvent(evt);
			}
			return flag;
		}

		public override void Core()
		{
			base.Core();
			for (int i = 0; i < _pluginList.Count; i++)
			{
				_pluginList[i].Core();
			}
		}

		public override void OnRemoval()
		{
			base.OnRemoval();
			for (int i = 0; i < _pluginList.Count; i++)
			{
				_pluginList[i].OnRemoved();
			}
			_pluginList.Clear();
			_pluginMap.Clear();
		}

		public List<BaseActorPlugin> GetAllPlugins()
		{
			return _pluginList;
		}
	}
}
