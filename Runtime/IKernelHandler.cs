using System;

namespace Kernel.Core
{
	public interface IKernelHandler
	{
		void Initialized();
		void Loaded();
		void Reset();
		void Destroy();
	}
}
