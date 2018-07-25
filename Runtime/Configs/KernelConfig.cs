using System;
using UnityEngine;

namespace Kernel.Core
{
	[CreateAssetMenu(menuName = "Configs/Kernel", order = Config.Order)]
	public class KernelConfig : Config
	{
		[SerializeField]
		[Tooltip("Is KernelSystem enabled?")]
		private bool _isEnabled = false;

#if UNITY_EDITOR
		[Tooltip("Play mode start scene.")]
		public UnityEditor.SceneAsset KernelScene;
#endif

		[Space]
		[SerializeField, Multiline]
		[Tooltip("Scenes match, e.g.  ^(Map|Game.*)$")]
		private string _scenesPattern = ".*";

		#region Properties
		public bool IsEnabled { get { return _isEnabled; } }
		public string ScenesPattern { get { return _scenesPattern; } }
		#endregion
	}
}
