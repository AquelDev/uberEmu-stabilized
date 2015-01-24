using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Pathfinding;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Help
{
    class CallGuideBotMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }
            foreach (var _user in Room.UserList)
            {
                if (_user.IsBot && _user.BotData.AiType == "guide")
                {
                    ServerPacket packet = new ServerPacket(33);
                    packet.AppendInt32(4009);
                    Session.SendPacket(packet);
                }
            }

            if (Session.GetHabbo().CalledGuideBot)
            {
                ServerPacket packet = new ServerPacket(33);
                packet.AppendInt32(4010);
                Session.SendPacket(packet);
                return;
            }

            RoomUser NewUser = Room.DeployBot(UberEnvironment.GetGame().GetBotManager().GetBot(55));
            NewUser.SetPos(Room.Model.DoorX, Room.Model.DoorY, Room.Model.DoorZ);
            NewUser.UpdateNeeded = true;

            RoomUser RoomOwner = Room.GetRoomUserByHabbo(Room.Owner);

            if (RoomOwner != null)
            {
                NewUser.MoveTo(RoomOwner.Coordinate);
                NewUser.SetRot(Rotation.Calculate(NewUser.X, NewUser.Y, RoomOwner.X, RoomOwner.Y));
            }

            UberEnvironment.GetGame().GetAchievementManager().UnlockAchievement(Session, 6, 1);
            Session.GetHabbo().CalledGuideBot = true;
        }
    }
}
