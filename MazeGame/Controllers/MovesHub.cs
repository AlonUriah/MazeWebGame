using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;


namespace MazeGame.Controllers
{
    [HubName("movesHub")]
    public class MovesHub : Hub
    {
        private static ConcurrentDictionary<int, string> connectedUsers =
            new ConcurrentDictionary<int, string>();

        public void Connect(int playerId)
        {
            connectedUsers[playerId] = Context.ConnectionId;
        }

        public void SendMove(int playerId, int opponentId, string move)
        {
            string recipientId = connectedUsers[opponentId];
            if (recipientId == null)
                return;
            Clients.Client(recipientId).gotMove(playerId, move);
        }
    }
}