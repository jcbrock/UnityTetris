using System;
namespace AssemblyCSharp
{
		public class AIMoveEvaluation
		{
				public int Row;
				public int Column;
				public int NumberOfRotations;
				public float Score;
				public bool PathClear;
				public PlacementStatus Status;
				public string DebugText;				
		
				public string Print ()
				{
						string stat = string.Empty;
						switch (Status) {
						case PlacementStatus.None:
								stat = "None";
								break;
						case PlacementStatus.CollidedWithWall:
								stat = "CollidedWithWall";
								break;
						case PlacementStatus.CollidedWithShape:
								stat = "CollidedWithShape";
								break;
						case PlacementStatus.CollidedWithWallAndShape:
								stat = "CollidedWithWallAndShape";
								break;
						case PlacementStatus.NoCollision:
								stat = "NoCollision";
								break;
						}
						return "PlacementEval - row: " + Row + " column: " + Column + " score: " + Score + " numberOfRotations: " + NumberOfRotations + " pathClear: " + PathClear + " status: " + stat + " debug: " + DebugText;
				}
		}
}

