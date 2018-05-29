
# Execution order
- IKernelHandler.Initialized
- SceneContext.OnInitialized
- MonoBehaviour.Awake
- MonoBehaviour.OnEnable
- IKernelConfiguration.Configure
- ... (KernelApplication.Loaded)
- SceneContext.StartContext
