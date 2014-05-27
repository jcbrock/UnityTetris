using System;
namespace AssemblyCSharp
{
		public interface IMenuObserver
		{
				void notify (GameState newState);
		}
}