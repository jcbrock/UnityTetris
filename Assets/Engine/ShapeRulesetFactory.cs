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
						Shape[] mRulesetOne = new Shape[7]; 
						mRulesetOne [0] = new Shape (UnityEngine.GameObject.Find ("square"), RotationStyles.none, 0);
						mRulesetOne [1] = new Shape (UnityEngine.GameObject.Find ("longShape"), RotationStyles.flip90, 0);
						mRulesetOne [2] = new Shape (UnityEngine.GameObject.Find ("tShape"), RotationStyles.full360, 0);
						mRulesetOne [3] = new Shape (UnityEngine.GameObject.Find ("zShapeRight"), RotationStyles.flip90, 0);
						mRulesetOne [4] = new Shape (UnityEngine.GameObject.Find ("zShapeLeft"), RotationStyles.flip90, 0);
						mRulesetOne [5] = new Shape (UnityEngine.GameObject.Find ("lShapeRight"), RotationStyles.full360, 0);
						mRulesetOne [6] = new Shape (UnityEngine.GameObject.Find ("lShapeLeft"), RotationStyles.full360, 0);						

						ShapeRuleset rulesetOne = new ShapeRuleset ();
						rulesetOne.SetRuleset (2, 7, 4, mRulesetOne);
						mRulesets.Add (rulesetOne);
				}

				public List<ShapeRuleset> GetRulesets ()
				{
						return mRulesets;
				}				
		}
}

