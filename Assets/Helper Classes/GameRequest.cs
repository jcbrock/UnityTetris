using System;
namespace AssemblyCSharp
{
		//Apparently Structs are more different than classes in C# than they are in C++
		//Their member variables default to private like classes do, (which is different than c++)		
		//Structs also can't be null
		public struct GameRequest
		{
				public enum Type
				{
						RotateShapeRequest,
						TranslateShapeRequest,
						ChangeGameStateRequest
				}
		
				public struct TranslateShapeData
				{
						public UnityEngine.Vector3 movementVector;
				};
		
				public GameRequest.Type type;						
				public TranslateShapeData translationData;		
				public GameState newGameState;						
		}
}

