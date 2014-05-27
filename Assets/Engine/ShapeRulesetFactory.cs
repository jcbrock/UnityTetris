using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		//factory returns a shape with random start position / rotation
		public class ShapeRulesetFactory
		{				 
				List<ShapeRuleset> mRulesets = new List<ShapeRuleset> ();					

				public ShapeRulesetFactory ()
				{													
						GenerateRuleset0 ();
						GenerateRuleset1 ();
						GenerateRuleset2 ();
				}

				public List<ShapeRuleset> GetRulesets ()
				{
						return mRulesets;
				}

				private void GenerateRuleset0 ()
				{
						Shape[] rulesetShapes = new Shape[7]; 
						rulesetShapes [0] = new Shape (UnityEngine.GameObject.Find ("square"), RotationStyles.none, 0);
						rulesetShapes [1] = new Shape (UnityEngine.GameObject.Find ("longShape"), RotationStyles.flip90, 0);
						rulesetShapes [2] = new Shape (UnityEngine.GameObject.Find ("tShape"), RotationStyles.full360, 0);
						rulesetShapes [3] = new Shape (UnityEngine.GameObject.Find ("zShapeRight"), RotationStyles.flip90, 0);
						rulesetShapes [4] = new Shape (UnityEngine.GameObject.Find ("zShapeLeft"), RotationStyles.flip90, 0);
						rulesetShapes [5] = new Shape (UnityEngine.GameObject.Find ("lShapeRight"), RotationStyles.full360, 0);
						rulesetShapes [6] = new Shape (UnityEngine.GameObject.Find ("lShapeLeft"), RotationStyles.full360, 0);						
			
						ShapeRuleset ruleset = new ShapeRuleset ();
						ruleset.SetRuleset (2, 7, 4, rulesetShapes);
						mRulesets.Add (ruleset);
				}

				private void GenerateRuleset1 ()
				{
						Shape[] rulesetShapes = new Shape[5]; 											
						rulesetShapes [0] = new Shape (UnityEngine.GameObject.Find ("tShape"), RotationStyles.none, 0);
						rulesetShapes [1] = new Shape (UnityEngine.GameObject.Find ("zShapeRight"), RotationStyles.none, 0);
						rulesetShapes [2] = new Shape (UnityEngine.GameObject.Find ("zShapeLeft"), RotationStyles.none, 0);
						rulesetShapes [3] = new Shape (UnityEngine.GameObject.Find ("lShapeRight"), RotationStyles.none, 0);
						rulesetShapes [4] = new Shape (UnityEngine.GameObject.Find ("lShapeLeft"), RotationStyles.none, 0);						
			
						ShapeRuleset ruleset = new ShapeRuleset ();
						ruleset.SetRuleset (2, 7, 4, rulesetShapes);
						mRulesets.Add (ruleset);
				}
				private void GenerateRuleset2 ()
				{
						Shape[] rulesetShapes = new Shape[1]; 
						rulesetShapes [0] = new Shape (UnityEngine.GameObject.Find ("zShapeRight"), RotationStyles.flip90, 0);
			
						ShapeRuleset ruleset = new ShapeRuleset ();
						ruleset.SetRuleset (2, 7, 4, rulesetShapes);
						mRulesets.Add (ruleset);
				}


		}
}

