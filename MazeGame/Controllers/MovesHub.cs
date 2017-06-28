using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;

namespace MazeGame.Controllers
{
    [HubName("movesHub")]
    public class MovesHub : Hub
    {
        private static ConcurrentDictionary<int, string> connectedUsers =
            new ConcurrentDictionary<int, string>();

        /// <summary>
        /// Connect to connectedUsers (concurrent dicrionary between users id and 
        /// connectionId). Key is user Id.
        /// </summary>
        /// <param name="playerId">A key to add to dictionary</param>
        public void Connect(int playerId)
        {
            // Check if user is disconnected first
            if(!connectedUsers.ContainsKey(playerId))
                connectedUsers[playerId] = Context.ConnectionId;
        }

        /// <summary>
        /// Send moves to opponent.
        /// </summary>
        /// <param name="playerId">Sender. The player that moved.</param>
        /// <param name="opponentId">Receiver. The opponent.</param>
        /// <param name="move">Movement key - UpArrow, DownArrow etc</param>
        public void SendMove(int playerId, int opponentId, string move)
        {
            // If opponent is connected
            if (!connectedUsers.ContainsKey(opponentId))
                return;

            string recipientId = connectedUsers[opponentId];
            
            // Trigger gotMove function at recipient (opponent)
            Clients.Client(recipientId).gotMove(playerId, move);
        }
    }
}