using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uber.Messages;
using Uber.HabboHotel.GameClients;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private const int HIGHEST_MESSAGE_ID = 4004;

        private GameClient Session;

        private ClientPacket Request;
        private ServerPacket Response;

        private delegate void RequestHandler();
        private RequestHandler[] RequestHandlers;

        public GameClientMessageHandler(GameClient Session)
        {
            this.Session = Session;

            RequestHandlers = new RequestHandler[HIGHEST_MESSAGE_ID];

            Response = new ServerPacket(0);
        }

        public ServerPacket GetResponse()
        {
            return Response;
        }

        public void Destroy()
        {
            Session = null;
            RequestHandlers = null;

            Request = null;
            Response = null;
        }

        public void HandleRequest(ClientPacket Request)
        {
            //UberEnvironment.GetLogging().WriteLine("[" + Session.ClientId + "] --> " + Request.Header + Request.GetBody(), Uber.Core.LogLevel.Debug);

            if (Request.Id < 0 || Request.Id > HIGHEST_MESSAGE_ID)
            {
                UberEnvironment.GetLogging().WriteLine("Warning - out of protocol request: " + Request.Header, Uber.Core.LogLevel.Warning);
                return;
            }

            if (RequestHandlers[Request.Id] == null)
            {
                return;
            }

            this.Request = Request;
            RequestHandlers[Request.Id].Invoke();
            this.Request = null;
        }

        public void SendResponse()
        {
            if (Response.Id > 0)
            {
                Session.GetConnection().SendPacket(Response);
            }
        }
    }
}
