using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Pets;
using Uber.HabboHotel.Rooms;
using Uber.Messages;

namespace Uber.Communication.Incoming.Rooms
{
    class PlacePetMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || (!Room.AllowPets && !Room.CheckRights(Session, true)))
            {
                return;
            }

            uint PetId = Packet.PopWiredUInt();

            Pet Pet = Session.GetHabbo().GetInventoryComponent().GetPet(PetId);

            if (Pet == null || Pet.PlacedInRoom)
            {
                return;
            }

            int X = Packet.PopWiredInt32();
            int Y = Packet.PopWiredInt32();

            if (!Room.CanWalk(X, Y, 0, true))
            {
                return;
            }

            if (Room.PetCount >= UberEnvironment.GetGame().GetRoomManager().MAX_PETS_PER_ROOM)
            {
                Session.SendNotif("There are too many pets in this room. A room may only contain up to " + UberEnvironment.GetGame().GetRoomManager().MAX_PETS_PER_ROOM + " pets.");
                return;
            }

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.RoomId;

            RoomUser PetUser = Room.DeployBot(new HabboHotel.RoomBots.RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0), Pet);

            if (Room.CheckRights(Session, true))
            {
                Session.GetHabbo().GetInventoryComponent().MovePetToRoom(Pet.PetId, Room.RoomId);
            }
        }
    }
}
