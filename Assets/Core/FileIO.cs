using System;
namespace AssemblyCSharp
{
		//Difference between lock and mutex in C#/.Net:
		//	Lock - per AppDomain (which is within a single process)
		//	Mutex - cross process locking / synchronization
		//Good AppDomain defintion: http://stackoverflow.com/questions/574708/what-is-appdomain
		//	An AppDomain provides a layer of isolation within a process. Everything you usually think of as "per program" (static variables etc) is actually per-AppDomain.
		public class FileIO
		{
				private System.Object mMutex = new System.Object ();
		
				public string ReadFromFile (string fileName)
				{
						string text = string.Empty;
						try {
								using (System.IO.StreamReader file = new System.IO.StreamReader(fileName, true)) { 
										text = file.ReadToEnd ();
								}																
						} catch (Exception ex) {
								UnityEngine.Debug.LogError ("Error reading from file: " + ex.Message);
						}		
						return text;
				}

				public void WriteToFile (string fileName, string text)
				{						
						try {
								lock (mMutex) {										
										System.IO.File.WriteAllText (fileName, text);						
								} //unlocks mutex								
						} catch (Exception ex) {
								UnityEngine.Debug.LogError ("Error writing to file: " + ex.Message);
						}
				}
		}
}

