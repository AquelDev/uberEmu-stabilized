using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Navigators;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Rooms
{
    class SaveRoomSettingsMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            Room Room = UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);

            if (Room == null || !Room.CheckRights(Session, true))
            {
                return;
            }

            int Id = Packet.PopWiredInt32();
            string Name = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            string Description = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            int State = Packet.PopWiredInt32();
            string Password = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());
            int MaxUsers = Packet.PopWiredInt32();
            int CategoryId = Packet.PopWiredInt32();
            int TagCount = Packet.PopWiredInt32();

            List<string> Tags = new List<string>();
            StringBuilder formattedTags = new StringBuilder();

            for (int i = 0; i < TagCount; i++)
            {
                if (i > 0)
                {
                    formattedTags.Append(",");
                }

                string tag = UberEnvironment.FilterInjectionChars(Packet.PopFixedString().ToLower());

                Tags.Add(tag);
                formattedTags.Append(tag);
            }

            int AllowPets = 0;
            int AllowPetsEat = 0;
            int AllowWalkthrough = 0;
            int Hidewall = 0;

            string _AllowPets = Packet.PlainReadBytes(1)[0].ToString();
            Packet.AdvancePointer(1);

            string _AllowPetsEat = Packet.PlainReadBytes(1)[0].ToString();
            Packet.AdvancePointer(1);

            string _AllowWalkthrough = Packet.PlainReadBytes(1)[0].ToString();
            Packet.AdvancePointer(1);

            string _Hidewall = Packet.PlainReadBytes(1)[0].ToString();
            Packet.AdvancePointer(1);

            if (Name.Length < 1)
            {
                return;
            }

            if (State < 0 || State > 2)
            {
                return;
            }

            if (MaxUsers != 10 && MaxUsers != 15 && MaxUsers != 20 && MaxUsers != 25)
            {
                return;
            }

            FlatCat FlatCat = UberEnvironment.GetGame().GetNavigator().GetFlatCat(CategoryId);

            if (FlatCat == null)
            {
                return;
            }

            if (FlatCat.MinRank > Session.GetHabbo().Rank)
            {
                Session.SendNotif("You are not allowed to use this category. Your room has been moved to no category instead.");
                CategoryId = 0;
            }

            if (TagCount > 2)
            {
                return;
            }

            if (State < 0 || State > 2)
            {
                return;
            }

            if (_AllowPets == "65")
            {
                AllowPets = 1;
                Room.AllowPets = true;
            }
            else
            {
                Room.AllowPets = false;
            }

            if (_AllowPetsEat == "65")
            {
                AllowPetsEat = 1;
                Room.AllowPetsEating = true;
            }
            else
            {
                Room.AllowPetsEating = false;
            }

            if (_AllowWalkthrough == "65")
            {
                AllowWalkthrough = 1;
                Room.AllowWalkthrough = true;
            }
            else
            {
                Room.AllowWalkthrough = false;
            }

            if (_Hidewall == "65")
            {
                Hidewall = 1;
                Room.Hidewall = true;
            }
            else
            {
                Room.Hidewall = false;
            }

            Room.Name = Name;
            Room.State = State;
            Room.Description = Description;
            Room.Category = CategoryId;
            Room.Password = Password;
            Room.Tags = Tags;
            Room.UsersMax = MaxUsers;

            string formattedState = "open";

            if (Room.State == 1)
            {
                formattedState = "locked";
            }
            else if (Room.State > 1)
            {
                formattedState = "password";
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("caption", Room.Name);
                dbClient.AddParamWithValue("description", Room.Description);
                dbClient.AddParamWithValue("password", Room.Password);
                dbClient.AddParamWithValue("tags", formattedTags.ToString());
                dbClient.ExecuteQuery("UPDATE rooms SET caption = @caption, description = @description, password = @password, category = '" + CategoryId + "', state = '" + formattedState + "', tags = @tags, users_max = '" + MaxUsers + "', allow_pets = '" + AllowPets + "', allow_pets_eat = '" + AllowPetsEat + "', allow_walkthrough = '" + AllowWalkthrough + "', allow_hidewall = '" + Hidewall + "' WHERE id = '" + Room.RoomId + "' LIMIT 1");
            }

            ServerPacket packet = new ServerPacket(467);
            packet.AppendUInt(Room.RoomId);
            Session.SendPacket(packet);

            packet = new ServerPacket(456);
            packet.AppendUInt(Room.RoomId);
            Session.SendPacket(packet);

            packet = new ServerPacket(472);
            packet.AppendBoolean(Room.Hidewall);
            UberEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId).SendMessage(packet);

            RoomData Data = new RoomData();
            Data.Fill(Room);

            packet = new ServerPacket(454);
            packet.AppendBoolean(false);
            Data.Serialize(packet, false);
            packet.AppendBoolean(false);
            packet.AppendBoolean(false);
            Session.SendPacket(packet);
        }
    }
}
