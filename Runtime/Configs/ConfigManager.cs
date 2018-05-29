using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Core
{
	public static class ConfigManager
	{
		private static string _path = "Configs";
		private static Dictionary<Type, Config> _cachedConfigs;


		public static string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		public static T Load<T>() where T : Config
		{
			if (_cachedConfigs == null) LoadConfigs();

			var type = typeof(T);
			if (!_cachedConfigs.ContainsKey(type))
			{
				return default(T);
			}
			return _cachedConfigs[type] as T;
		}

		private static void LoadConfigs()
		{
			_cachedConfigs = new Dictionary<Type, Config>();
			var configs = Resources.LoadAll<Config>(Path);
			foreach (var config in configs)
			{
				var type = config.GetType();
				if (_cachedConfigs.ContainsKey(type))
				{
					Debug.LogErrorFormat("Config with type \"{0}\" already loaded, skipping...", type);
					continue;
				}
				_cachedConfigs.Add(type, config);
			}
		}
	}
}
