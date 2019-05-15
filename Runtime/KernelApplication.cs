using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Kernel.Core
{
	[DefaultExecutionOrder(-1001)]
	public sealed class KernelApplication : MonoBehaviour
	{
		public static bool IsInitialized { get { return _instance != null; } }
		public static bool IsLoaded { get; private set; }

		private static KernelApplication _instance;

		public static List<object> Args { get; set; }

		private int _configurationsInProgress;


		private void Awake()
		{
			_instance = this;

			Args = new List<object>();

			var handler = GetComponentInChildren<IKernelHandler>();
			if (handler != null)
				handler.KernelInitialized();
		}

		private void Start()
		{
			StartCoroutine(ConfigureAsync());
		}

		private void OnDestroy()
		{
			var handler = GetComponentInChildren<IKernelHandler>();
			if (handler != null)
				handler.KernelDestroy();

			_instance = null;
		}

		private IEnumerator ConfigureAsync()
		{
			var configurations = GetComponentsInChildren<IKernelConfiguration>();
			_configurationsInProgress = configurations.Length;
			foreach (var configuration in configurations)
			{
				StartCoroutine(ConfigureAsync(configuration));
			}
			while (_configurationsInProgress > 0)
			{
				yield return null;
			}
			IsLoaded = true;

			var handler = GetComponentInChildren<IKernelHandler>();
			if (handler != null)
				handler.KernelLoaded();
		}

		private IEnumerator ConfigureAsync(IKernelConfiguration configuration)
		{
			yield return StartCoroutine(configuration.Configure());
			--_configurationsInProgress;
		}


		/// <summary>
		/// Нужно подождать, пока выгрузится предыдущая сцена, прежде чем загружать новую,
		/// иначе Locator.Reset() сбросит сервисы в новой сцене.
		/// Для загрузки новых сцен нужно использовать <see cref="KernelApplication.LoadScene"/>, в не <see cref="SceneManager.LoadScene"/>.
		/// </summary>
		/// <param name="scene">Сцена</param>
		/// <param name="args">Аргументы</param>
		public static void LoadScene(string scene, params object[] args)
		{
			Debug.Assert(_instance != null);

			_instance.StartCoroutine(_instance.LoadSceneAsyncRoutine(scene, args));
		}

		private IEnumerator LoadSceneAsyncRoutine(string scene, params object[] args)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				var s = SceneManager.GetSceneAt(i);
				if (s.name != gameObject.scene.name)
				{
					yield return SceneManager.UnloadSceneAsync(s.name);
				}
			}

			var handler = GetComponentInChildren<IKernelHandler>();
			if (handler != null)
				handler.KernelReset();

			Args = new List<object>(args);

			yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
		}
	}
}
