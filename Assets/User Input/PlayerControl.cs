using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		public class PlayerControl : MonoBehaviour
		{		
				private List<IInputObserver> registeredObservers = new List<IInputObserver> ();

				void Update ()
				{						
						if (Input.GetKeyDown (KeyCode.LeftArrow)) {
								NotifyObservers (KeyCode.LeftArrow);
						} else if (Input.GetKeyDown (KeyCode.RightArrow)) { 
								NotifyObservers (KeyCode.RightArrow);								
						} else if (Input.GetKeyDown (KeyCode.DownArrow)) { 
								NotifyObservers (KeyCode.DownArrow);								
						} else if (Input.GetKeyDown (KeyCode.UpArrow)) {							
								NotifyObservers (KeyCode.UpArrow);								
						} else if (Input.GetKeyDown (KeyCode.Escape)) {
								NotifyObservers (KeyCode.Escape);								
						} else if (Input.GetKeyDown (KeyCode.P)) {
								NotifyObservers (KeyCode.P);	
						} else if (Input.GetKeyDown (KeyCode.Alpha0)) {
								NotifyObservers (KeyCode.Alpha0);	
						} else if (Input.GetKeyDown (KeyCode.Minus)) {
								NotifyObservers (KeyCode.Minus);	
						} else if (Input.GetKeyDown (KeyCode.C)) {
								NotifyObservers (KeyCode.C);	
						} else if (Input.GetKeyDown (KeyCode.A)) {
								NotifyObservers (KeyCode.A);
						}
				}

				public void RegisterObserver (IInputObserver observer)
				{
						registeredObservers.Add (observer);
				}
				public void UnregisterObserver (IInputObserver observer)
				{
						registeredObservers.Remove (observer);
				}
				private void NotifyObservers (KeyCode pressedKey)
				{
						foreach (IInputObserver observer in registeredObservers) {
								observer.notify (pressedKey);
						}
				}		
		}
}


