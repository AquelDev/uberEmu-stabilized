using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class DanceMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null)
            {
                return;
            }

            User.Unidle();

            int _danceId = Packet.PopWiredInt32();

            if (_danceId < 0 || _danceId > 4 || (!Session.GetHabbo().HasFuse("fuse_use_club_dance") && _danceId > 1))
            {
                _danceId = 0;
            }

            if (_danceId > 0 && User.CarryItemID > 0)
            {
                User.CarryItem(0);
            }

            User.DanceId = _danceId;

            ServerPacket DanceMessageEventMessage = new ServerPacket(480);
            DanceMessageEventMessage.AppendInt32(User.VirtualId);
            DanceMessageEventMessage.AppendInt32(_danceId);
            Room.SendMessage(DanceMessageEventMessage);
        }
    }
}
