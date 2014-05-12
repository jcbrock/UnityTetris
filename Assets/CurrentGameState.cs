using UnityEngine;
using System.Collections.Generic;

public class CurrentGameState
{
		public enum GameState
		{
				None,
				Started,
				Paused,
				Running,
				Ended
		}
	
		public struct GameStateRequestInfo
		{
				public enum Type
				{
						UpdateGameState								
				}

				public struct GameStateData
				{
						public GameState newGameState;
				};
				
				public string debugName;
				public	GameStateRequestInfo.Type type;
				public	GameStateData newGameStateData;			
		}

		private Queue<GameStateRequestInfo> requestQueue = new Queue<GameStateRequestInfo> ();
		private GameState m_CurrentGameState;
		private List<Object> m_Subscribers = new List<Object> ();

		public void UpdateQueuedRequests ()
		{						
				if (requestQueue.Count == 0)
						return;
		
				//pop request
				GameStateRequestInfo request = requestQueue.Dequeue (); //TODO - throttle?						
		
				switch (request.type) {
				case GameStateRequestInfo.Type.UpdateGameState:
						{
								HandleUpdateGameStateRequest (request);
								break;
						}
				default:
						{
								UnityEngine.Debug.LogWarning ("No type sent on GameState request... this is probably bad!");
								break;
						}
				}
		
		
		}
	
		private void HandleUpdateGameStateRequest (GameStateRequestInfo request)
		{
				//update current game state, publish to subscribers
				m_CurrentGameState = request.newGameStateData.newGameState;
				foreach (Object subscriber in m_Subscribers) {
						//publish game state
				}
		}
		
		public void UpdateGameState (GameState gameState)
		{						
				GameStateRequestInfo updateRequest;
				updateRequest.debugName = "updating";
				updateRequest.type = GameStateRequestInfo.Type.UpdateGameState;
				updateRequest.newGameStateData.newGameState = gameState;
				requestQueue.Enqueue (updateRequest);
		}
}
