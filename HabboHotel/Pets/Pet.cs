using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.HabboHotel.Pets
{
    class Pet
    {
        public uint PetId;
        public uint OwnerId;
        public int VirtualId;

        public uint Type;
        public string Name;
        public string Race;
        public string Color;

        public int Expirience;
        public int Energy;
        public int Nutrition;

        public uint RoomId;
        public int X;
        public int Y;
        public double Z;

        public int Respect;

        public double CreationStamp;
        public bool PlacedInRoom;

        public int[] experienceLevels = new int[] { 100, 200, 400, 600, 1000, 1300, 1800, 2400, 3200, 4300, 7200, 8500, 10100, 13300, 17500, 23000, 30000, 40000, 55000, 75000 };

        public Room Room
        {
            get
            {
                if (!IsInRoom)
                {
                    return null;
                }

                return UberEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            }
        }

        public bool IsInRoom
        {
            get
            {
                return (RoomId > 0);
            }
        }

        public int Level
        {
            get
            {
                for (int level = 0; level < experienceLevels.Length; ++level)
                {
                    if (Expirience < experienceLevels[level])
                        return level + 1;
                }
                return experienceLevels.Length + 1;
            }
        }

        public int MaxLevel
        {
            get
            {
                return 20;
            }
        }

        public int ExpirienceGoal
        {
            get
            {
                return experienceLevels[Level - 1];
            }
        }

        public int MaxEnergy
        {
            get
            {
                return 100;
            }
        }

        public int MaxNutrition
        {
            get
            {
                return 150;
            }
        }

        public int Age
        {
            get
            {
                return (int)Math.Floor((UberEnvironment.GetUnixTimestamp() - CreationStamp) / 86400);
            }
        }

        public string Look
        {
            get
            {
                return Type + " " + Race + " " + Color;
            }
        }

        public string OwnerName
        {
            get
            {
                return UberEnvironment.GetGame().GetClientManager().GetNameById(OwnerId);
            }
        }

        /*
        public int BasketX
        {
        get
        {
        try
        {
        using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
        {
        dbClient.AddParamWithValue("rID", RoomId);
        int ItemX = dbClient.ReadInt32("SELECT x FROM room_items WHERE room_id = @rID AND base_item = 317");

        // 317- nest
        return ItemX;
        }
        }
        catch { return 0; } // Doesn't Exist
        }
        }

        public int BasketY
        {
        get
        {
        try
        {
        using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
        {
        dbClient.AddParamWithValue("rID", RoomId);
        int ItemY = dbClient.ReadInt32("SELECT y FROM room_items WHERE room_id = @rID AND base_item = 317");

        // 317- nest
        return ItemY;
        }
        }
        catch { return 0; } // Doesn't Exist
        }
        }
        */

        public Pet(uint PetId, uint OwnerId, uint RoomId, string Name, uint Type, string Race, string Color, int Expirience, int Energy, int Nutrition, int Respect, double CreationStamp, int X, int Y, double Z)
        {
            this.PetId = PetId;
            this.OwnerId = OwnerId;
            this.RoomId = RoomId;
            this.Name = Name;
            this.Type = Type;
            this.Race = Race;
            this.Color = Color;
            this.Expirience = Expirience;
            this.Energy = Energy;
            this.Nutrition = Nutrition;
            this.Respect = Respect;
            this.CreationStamp = CreationStamp;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.PlacedInRoom = false;
        }

        public void OnRespect()
        {
            Respect++;

            ServerPacket Message = new ServerPacket(440);
            Message.AppendUInt(PetId);
            Message.AppendInt32(Expirience + 10);
            Room.SendMessage(Message);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("petid", PetId);
                dbClient.ExecuteQuery("UPDATE user_pets SET respect = respect + 1 WHERE id = @petid LIMIT 1");
            }

            if (Expirience <= 75000)
            {
                AddExpirience(10);
            }
        }

        public void AddExpirience(int Amount)
        {
            Expirience = Expirience + Amount;

            if (Expirience >= 75000)
            {
                return;
            }

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("petid", PetId);
                dbClient.AddParamWithValue("expirience", Expirience);
                dbClient.ExecuteQuery("UPDATE user_pets SET expirience = @expirience WHERE id = @petid LIMIT 1");
            }

            if (Room != null)
            {
                ServerPacket Message = new ServerPacket(609);
                Message.AppendUInt(PetId);
                Message.AppendInt32(VirtualId);
                Message.AppendInt32(Amount);
                Room.SendMessage(Message);

                if (Expirience > ExpirienceGoal)
                {
                    // Level the pet

                    ServerPacket ChatMessage = new ServerPacket(24);
                    ChatMessage.AppendInt32(VirtualId);
                    ChatMessage.AppendStringWithBreak("*leveled up to level " + Level + " *");
                    ChatMessage.AppendInt32(0);

                    Room.SendMessage(ChatMessage);
                }
            }

        }

        public void PetEnergy(bool Add)
        {
            int MaxE;

            if (Add)
            {
                if (this.Energy == 100) // If Energy is 100, no point.
                    return;

                if (this.Energy > 85) { MaxE = this.MaxEnergy - this.Energy; } else { MaxE = 10; }

            }
            else { MaxE = 15; } // Remove Max Energy as 15

            int r = UberEnvironment.GetRandomNumber(4, MaxE);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                if (!Add)
                {
                    this.Energy = this.Energy - r;

                    if (this.Energy < 0)
                    {
                        dbClient.AddParamWithValue("pid", PetId);
                        dbClient.ExecuteQuery("UPDATE user_pets SET energy = 1 WHERE id = @pid LIMIT 1");

                        this.Energy = 1;

                        r = 1;
                    }

                    dbClient.AddParamWithValue("r", r);
                    dbClient.AddParamWithValue("petid", PetId);
                    dbClient.ExecuteQuery("UPDATE user_pets SET energy = energy - @r WHERE id = @petid LIMIT 1");

                }
                else
                {
                    dbClient.AddParamWithValue("r", r);
                    dbClient.AddParamWithValue("petid", PetId);
                    dbClient.ExecuteQuery("UPDATE user_pets SET energy = energy + @r WHERE id = @petid LIMIT 1");

                    this.Energy = this.Energy + r;
                }
            }
        }

        public void SerializeInventory(ServerPacket Message)
        {
            Message.AppendUInt(PetId);
            Message.AppendStringWithBreak(Name);
            Message.AppendStringWithBreak(Look);
            Message.AppendBoolean(false);
        }

        public ServerPacket SerializeInfo()
        {
            // IYbtmFZoefKPEY]AXdAPhPhHPh0 008 D98961SBhRPZA[lFmybad
            ServerPacket Nfo = new ServerPacket(601);
            Nfo.AppendUInt(PetId);
            Nfo.AppendStringWithBreak(Name);
            Nfo.AppendInt32(Level);
            Nfo.AppendInt32(MaxLevel);
            Nfo.AppendInt32(Expirience);
            Nfo.AppendInt32(ExpirienceGoal);
            Nfo.AppendInt32(Energy);
            Nfo.AppendInt32(MaxEnergy);
            Nfo.AppendInt32(Nutrition);
            Nfo.AppendInt32(MaxNutrition);
            Nfo.AppendStringWithBreak(Look);
            Nfo.AppendInt32(Respect);
            Nfo.AppendUInt(OwnerId);
            Nfo.AppendInt32(Age);
            Nfo.AppendStringWithBreak(OwnerName);
            return Nfo;
        }
    }
}