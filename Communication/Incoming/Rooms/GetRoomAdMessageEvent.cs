using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class GetRoomAdMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().LoadingRoom <= 0 || !Session.GetHabbo().LoadingChecksPassed)
            {
                return;
            }

            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoom);

            if (Room == null)
            {
                return;
            }

            Session.ClearRoomLoading();

            ServerPacket packet = new ServerPacket(30);

            if (Room.Model.StaticFurniMap != "")
            {
                packet.AppendStringWithBreak(Room.Model.StaticFurniMap);
            }
            else
            {
                packet.AppendInt32(0);
            }

            Session.SendPacket(packet);

            if (Room.Type == "private")
            {
                List<RoomItem> FloorItems = Room.FloorItems;
                List<RoomItem> WallItems = Room.WallItems;

                ServerPacket _packet = new ServerPacket(32);
                _packet.AppendInt32(FloorItems.Count);

                foreach (RoomItem Item in FloorItems)
                {
                    Item.Serialize(_packet);
                }

                Session.SendPacket(_packet);

                ServerPacket pak = new ServerPacket(45);
                pak.AppendInt32(WallItems.Count);

                foreach (RoomItem Item in WallItems)
                {
                    Item.Serialize(pak);
                }

                Session.SendPacket(pak);
            }

            Room.AddUserToRoom(Session, Session.GetHabbo().SpectatorMode);

            List<RoomUser> UsersToDisplay = new List<RoomUser>();

            foreach (RoomUser User in Room.UserList)
            {
                if (User.IsSpectator)
                {
                    continue;
                }

                UsersToDisplay.Add(User);
            }

            ServerPacket _pak = new ServerPacket(28);
            _pak.AppendInt32(UsersToDisplay.Count);

            foreach (RoomUser User in UsersToDisplay)
            {
                User.Serialize(_pak);
            }

            Session.SendPacket(_pak);

            //GXI
            ServerPacket p472 = new ServerPacket(472);
            p472.AppendBoolean(Room.Hidewall);
            Session.SendPacket(p472);

            if (Room.Type == "public")
            {
                ServerPacket p471 = new ServerPacket(471);
                p471.AppendBoolean(false);
                p471.AppendStringWithBreak(Room.ModelName);
                p471.AppendBoolean(false);
                Session.SendPacket(p471);
            }
            else if (Room.Type == "private")
            {
                ServerPacket p471 = new ServerPacket(471);
                p471.AppendBoolean(true);
                p471.AppendUInt(Room.RoomId);

                if (Room.CheckRights(Session, true))
                {
                    p471.AppendBoolean(true);
                }
                else
                {
                    p471.AppendBoolean(false);
                }

                Session.SendPacket(p471);

                ServerPacket p454 = new ServerPacket(454);
                p454.AppendInt32(1);
                p454.AppendUInt(Room.RoomId);
                p454.AppendInt32(0);
                p454.AppendStringWithBreak(Room.Name);
                p454.AppendStringWithBreak(Room.Owner);
                p454.AppendInt32(Room.State);
                p454.AppendInt32(0);
                p454.AppendInt32(25);
                p454.AppendStringWithBreak(Room.Description);
                p454.AppendInt32(0);
                p454.AppendInt32(1);
                p454.AppendInt32(8228);
                p454.AppendInt32(Room.Category);
                p454.AppendStringWithBreak("");
                p454.AppendInt32(Room.TagCount);

                foreach (string Tag in Room.Tags)
                {
                    p454.AppendStringWithBreak(Tag);
                }

                Room.Icon.Serialize(p454);
                p454.AppendBoolean(false);
                Session.SendPacket(p454);
            }

            ServerPacket Updates = Room.SerializeStatusUpdates(true);

            if (Updates != null)
            {
                Session.SendPacket(Updates);
            }

            foreach (RoomUser User in Room.UserList)
            {
                if (User.IsSpectator)
                {
                    continue;
                }

                if (User.IsDancing)
                {
                    ServerPacket p480 = new ServerPacket(480);
                    p480.AppendInt32(User.VirtualId);
                    p480.AppendInt32(User.DanceId);
                    Session.SendPacket(p480);
                }

                if (User.IsAsleep)
                {
                    ServerPacket p486 = new ServerPacket(486);
                    p486.AppendInt32(User.VirtualId);
                    p486.AppendBoolean(true);
                    Session.SendPacket(p486);
                }

                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    ServerPacket p482 = new ServerPacket(482);
                    p482.AppendInt32(User.VirtualId);
                    p482.AppendInt32(User.CarryTimer);
                    Session.SendPacket(p482);
                }

                if (!User.IsBot)
                {
                    if (User.GetClient().GetHabbo() != null && User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent() != null && User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().CurrentEffect >= 1)
                    {
                        ServerPacket p485 = new ServerPacket(485);
                        p485.AppendInt32(User.VirtualId);
                        p485.AppendInt32(User.GetClient().GetHabbo().GetAvatarEffectsInventoryComponent().CurrentEffect);
                        Session.SendPacket(p485);
                    }
                }
            }
        }
    }
}
