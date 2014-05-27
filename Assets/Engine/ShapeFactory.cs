using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace AssemblyCSharp
{
		
		public class ShapeFactory
		{
				private ShapeRulesetFactory mRulesetFactory = new ShapeRulesetFactory ();								
				private static int mDebugId = 0;				
				
				public ShapeFactory ()
				{
				}			
	
				public Shape SpawnRandomizedTetrisShape (int rulesetOption)
				{
						if (rulesetOption < 0 || rulesetOption > mRulesetFactory.GetRulesets ().Count)
								throw new ArgumentOutOfRangeException ("Invalid ruleset specified!");
						
						ShapeRuleset ruleset = mRulesetFactory.GetRulesets () [rulesetOption];

						//Choose random shape
						int randomNumber = UnityEngine.Random.Range (0, ruleset.ShapesInRuleset.Length); //7 = number of possible shapes				
						Shape randomShapeToGenerate = ruleset.ShapesInRuleset [randomNumber];
						
						//Choose random start column
						float xStart = UnityEngine.Random.Range (ruleset.PossibleXStartPosition, ruleset.PossibleXEndPosition); //10 = length of tetris board (x)
						xStart -= (float)0.5; //offset because the middle of the shape needs to in the middle of the cell to line up in the grid
						UnityEngine.Vector3 previewPosition = new UnityEngine.Vector3 (-5.0f, (float)-0.5, 0); //this is the preview shape position. todo - fix from being magic numbers.

						//Spawn						
						Shape newShape = randomShapeToGenerate.Clone (previewPosition, xStart, (++mDebugId).ToString ());						

						//Give random rotation
						int rotation = UnityEngine.Random.Range (0, ruleset.NumberOfPossibleRotations - 1); //Rotation possiblities
						for (int i = 0; i < rotation; ++i) {
								newShape.Rotate ();
						}

						return newShape;
				}						
		}
}
