using System;

namespace Kernel.Core
{
	public interface IKernelHandler
	{
		void KernelInitialized();
		void KernelLoaded();
		void KernelReset();
		void KernelDestroy();
	}
}
