using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kernel.Core
{
	public abstract class SceneContext : MonoBehaviour
	{
		#region Instance
		protected static SceneContext Instance { get; private set; }
		#endregion


		protected void Awake()
		{
			if (!KernelApplication.IsInitialized)
			{
				Debug.LogError("Load <b>Kernel</b> scene and set it as <b>Active</b>");
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
			}

			if (Instance != null)
			{
				Debug.LogErrorFormat("SceneContext with type\"{0}\" already exists", Instance.GetType());
				return;
			}
			Instance = this;

			OnInitializedInternal();
			OnInitialized();
		}

		protected void Start()
		{
			if (!KernelApplication.IsLoaded)
			{
				Debug.LogError("<b>Kernel</b> not loaded");
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#endif
			}

			SceneManager.SetActiveScene(gameObject.scene);

			StartContextInternal();

			var roots = gameObject.scene
				.GetRootGameObjects()
				.Where(x => x.GetComponent<SceneRoot>() != null);

			foreach (var root in roots)
			{
				root.gameObject.SetActive(true);
			}

			StartContext();
		}

		protected void OnDestroy()
		{
			StopContextInternal();
			StopContext();

			Instance = null;
		}

		protected virtual void OnInitializedInternal() { }
		protected virtual void StartContextInternal() { }
		protected virtual void StopContextInternal() { }


		protected virtual void OnInitialized() { }
		protected virtual void StartContext() { }
		protected virtual void StopContext() { }
	}
}
