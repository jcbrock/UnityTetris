using System;
namespace AssemblyCSharp
{
		[Flags()]
		public enum GameState : int
		{
				None = 1,
				Paused = 2,
				Running = 4,
				Ended = 8				
		}
}

