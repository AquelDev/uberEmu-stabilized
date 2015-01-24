using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uber.Messages
{
    partial class GameClientMessageHandler
    {
        private void PongMessageEvent()
        {
            Session.PongOK = true;
        }

        public void RegisterGlobal()
        {
            RequestHandlers[196] = new RequestHandler(PongMessageEvent);
        }
    }
}
