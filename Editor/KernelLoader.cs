using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Kernel.Core;
using System;
using System.Collections.Generic;

namespace KernelEditor.Core
{
	[InitializeOnLoad]
	public class KernelLoader
	{
		[Serializable]
		private class ScenesSetup
		{
			public string[] Scenes;
		}

		static KernelLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private static void OnLoadingKernel()
		{
			if (KernelApplication.IsLoaded)
			{
				EditorApplication.update -= OnLoadingKernel;

				var setup = new ScenesSetup();
				var json = EditorPrefs.GetString("KernelLoader.Scenes", string.Empty);
				if (!string.IsNullOrEmpty(json))
				{
					EditorJsonUtility.FromJsonOverwrite(json, setup);
				}
				if (setup != null && setup.Scenes != null)
				{
					foreach (var s in setup.Scenes)
					{
						SceneManager.LoadScene(s, LoadSceneMode.Additive);
					}
				}
			}
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			if (state == PlayModeStateChange.ExitingEditMode)
			{
				EditorPrefs.SetString("KernelLoader.Scenes", string.Empty);
				EditorSceneManager.playModeStartScene = null;
			}
			else if (state == PlayModeStateChange.EnteredPlayMode)
			{
				// Do nothing
			}
			else
			{
				// Unused state
				return;
			}

			KernelConfig config = null;
			var configs = Resources.LoadAll<KernelConfig>(string.Empty);
			if (configs.Length == 0)
			{
				Debug.LogError("<b>KernelLoader</b>: config not exists");
				EditorApplication.isPlaying = false;
				return;
			}
			if (configs.Length > 1)
			{
				Debug.LogWarning("<b>KernelLoader</b>: multiple configs. Using first default");
				foreach (var asset in configs)
				{
					Debug.LogWarning(" * Config: " + AssetDatabase.GetAssetPath(asset));
				}
			}

			config = configs[0];

			if (!config.IsEnabled) return;

			if (config.KernelScene == null)
			{
				Debug.LogWarning("<b>KernelLoader</b>: Kernel Scene is null");
				return;
			}

			if (state == PlayModeStateChange.ExitingEditMode)
			{
				var activeSceneName = EditorSceneManager.GetActiveScene().name;
				if (!new Regex(config.ScenesPattern).IsMatch(activeSceneName)) return;

				Debug.Log("<i>Load <b>Kernel</b> scene.</i>");

				var scenes = new List<string>();
				for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
				{
					var sceneName = EditorSceneManager.GetSceneAt(i).name;
					if (sceneName == config.KernelScene.name) continue;
					scenes.Add(sceneName);
				}

				EditorPrefs.SetString("KernelLoader.Scenes", EditorJsonUtility.ToJson(new ScenesSetup { Scenes = scenes.ToArray() }));

				EditorSceneManager.playModeStartScene = config.KernelScene;
			}
			else if (state == PlayModeStateChange.EnteredPlayMode)
			{
				var scenes = EditorPrefs.GetString("KernelLoader.Scenes", string.Empty);
				if (string.IsNullOrEmpty(scenes)) return;

				EditorApplication.update += OnLoadingKernel;
			}
		}
	}
}
