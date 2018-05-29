using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using Kernel.Core;

namespace KernelEditor.Core
{
	[InitializeOnLoad]
	public class KernelLoader
	{

		static KernelLoader()
		{
			EditorApplication.playModeStateChanged += OnPlayModeChanged;
		}

		private static void OnPlayModeChanged(PlayModeStateChange state)
		{
			var config = ConfigManager.Load<KernelConfig>();
			if (config == null)
			{
				Debug.LogError("<b>KernelLoader</b> config not exists");
				EditorApplication.isPlaying = false;
				return;
			}

			if (!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
			{
				NeedUnloadKernel = false;
				ActiveScene = EditorSceneManager.GetActiveScene().name;

				if (!new Regex(config.ScenesPattern).IsMatch(ActiveScene)) return;

				Scene scene;
				if (!IsKernelLoaded)
				{
					Debug.Log("<i>Load Kernel scene.</i>");
					var kernelPath = AssetDatabase.GetAssetPath(config.KernelScene);
					scene = EditorSceneManager.OpenScene(kernelPath, OpenSceneMode.Additive);
					if (!scene.isLoaded)
					{
						Debug.LogWarning("Cant load Kernel scene");
						EditorApplication.isPlaying = false;
					}
					else
					{
						NeedUnloadKernel = true;
					}
				}
				else
				{
					scene = EditorSceneManager.GetSceneByName(config.KernelScene.name);
				}

				if (EditorSceneManager.GetActiveScene() != scene && !EditorSceneManager.SetActiveScene(scene))
				{
					Debug.LogWarning("Activate Kernel scene is failed");
					EditorApplication.isPlaying = false;
				}
			}

			if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
			{
				var activeScene = EditorSceneManager.GetSceneByName(ActiveScene);
				if (activeScene.isLoaded && activeScene != EditorSceneManager.GetActiveScene())
				{
					if (!EditorSceneManager.SetActiveScene(activeScene))
					{
						Debug.LogWarningFormat("Activate {0} scene is failed", activeScene.name);
					}
				}

				if (NeedUnloadKernel)
				{
					var kernelScene = EditorSceneManager.GetSceneByName(config.KernelScene.name);
					EditorSceneManager.CloseScene(kernelScene, true);
				}

				ClearPrefs();
			}
		}

		private static bool IsKernelLoaded
		{
			get
			{
				var config = ConfigManager.Load<KernelConfig>();
				if (config == null)
				{
					Debug.LogError("KernelLoaderConfig not exists");
					EditorApplication.isPlaying = false;
					return false;
				}

				for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++)
				{
					var scene = EditorSceneManager.GetSceneAt(i);
					if (scene.name == config.KernelScene.name) return true;
				}
				return false;
			}
		}

		private static bool NeedUnloadKernel
		{
			get { return PlayerPrefs.GetInt("Kernel.NeedUnloadKernel", 0) != 0; }
			set { PlayerPrefs.SetInt("Kernel.NeedUnloadKernel", value ? 1 : 0); }
		}

		private static string ActiveScene
		{
			get { return PlayerPrefs.GetString("Kernel.ActiveScene", string.Empty); }
			set { PlayerPrefs.SetString("Kernel.ActiveScene", value); }
		}

		private static void ClearPrefs()
		{
			PlayerPrefs.DeleteKey("Kernel.NeedUnloadKernel");
			PlayerPrefs.DeleteKey("Kernel.ActiveScene");
		}
	}
}
