using System;
using UnityEngine;

namespace Kernel.Core
{
	[DefaultExecutionOrder(-1000)]
	public class SceneRoot : MonoBehaviour
	{

		protected void Awake()
		{
			gameObject.SetActive(false);
		}
	}
}
