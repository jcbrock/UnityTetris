using System;
namespace AssemblyCSharp
{
		public interface IClassicTetrisStateObserver
		{
				void notify (ClassicTetrisStateUpdate newSceneInformation);
		}
}

