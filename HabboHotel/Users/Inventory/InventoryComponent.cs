using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Uber.Messages;
using Uber.HabboHotel.Pets;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Items;
using Uber.Storage;

namespace Uber.HabboHotel.Users.Inventory
{
    class InventoryComponent
    {
        private List<UserItem> InventoryItems;
        private List<Pet> InventoryPets;
        public uint UserId;

        public int ItemCount
        {
            get
            {
                return InventoryItems.Count;
            }
        }

        public int PetCount
        {
            get
            {
                return InventoryPets.Count;
            }
        }

        public InventoryComponent(uint UserId)
        {
            this.UserId = UserId;
            this.InventoryItems = new List<UserItem>();
            this.InventoryPets = new List<Pet>();
        }

        public void ClearItems()
        {
            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", UserId);
                dbClient.ExecuteQuery("DELETE FROM user_items WHERE user_id = @userid");
            }

            UpdateItems(true);
        }

        public void ClearPets()
        {
            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", UserId);
                dbClient.ExecuteQuery("DELETE FROM user_pets WHERE user_id = @userid AND room_id = 0");
            }

            UpdatePets(true);
        }

        public void LoadInventory()
        {
            this.InventoryItems.Clear();
            DataTable _items = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", UserId);
                _items = dbClient.ReadDataTable("SELECT id,base_item,extra_data FROM user_items WHERE user_id = @userid");
            }

            if (_items != null)
            {
                foreach (DataRow Row in _items.Rows)
                {
                    InventoryItems.Add(new UserItem((uint)Row["id"], (uint)Row["base_item"], (string)Row["extra_data"]));
                }
            }

            this.InventoryPets.Clear();
            DataTable _pets = null;

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("userid", UserId);
                _pets = dbClient.ReadDataTable("SELECT * FROM user_pets WHERE user_id = @userid AND room_id <= 0");
            }

            if (_pets != null)
            {
                foreach (DataRow Row in _pets.Rows)
                {
                    InventoryPets.Add(UberEnvironment.GetGame().GetCatalog().GeneratePetFromRow(Row));
                }
            }
        }

        public void UpdateItems(bool FromDatabase)
        {
            if (FromDatabase)
            {
                LoadInventory();
            }

            GetClient().SendPacket(new ServerPacket(101));
        }

        public void UpdatePets(bool FromDatabase)
        {
            if (FromDatabase)
            {
                LoadInventory();
            }

            GetClient().SendPacket(SerializePetInventory());
        }

        public Pet GetPet(uint Id)
        {
            foreach (var _pet in InventoryPets)
            {
                if (_pet.PetId == Id)
                {
                    return _pet;
                }
            }
            return null;
        }

        public UserItem GetItem(uint Id)
        {
            foreach (var _item in InventoryItems)
            {
                if (_item.Id == Id)
                {
                    return _item;
                }
            }
            return null;
        }

        public void AddItem(uint Id, uint BaseItem, string ExtraData)
        {
            InventoryItems.Add(new UserItem(Id, BaseItem, ExtraData));
            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("extra_data", ExtraData);
                dbClient.ExecuteQuery("INSERT INTO user_items (id,user_id,base_item,extra_data) VALUES ('" + Id + "','" + UserId + "','" + BaseItem + "',@extra_data)");
            }
        }

        public void AddPet(Pet Pet)
        {
            if (Pet == null)
            {
                return;
            }

            Pet.PlacedInRoom = false;

            InventoryPets.Add(Pet);

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("botid", Pet.PetId);
                dbClient.ExecuteQuery("UPDATE user_pets SET room_id = 0, x = 0, y = 0, z = 0 WHERE id = @botid LIMIT 1");
            }

            ServerPacket AddMessage = new ServerPacket(603);
            Pet.SerializeInventory(AddMessage);
            GetClient().SendPacket(AddMessage);
        }

        public bool RemovePet(uint PetId)
        {
            foreach (Pet Pet in this.InventoryPets)
            {
                if (Pet.PetId != PetId)
                {
                    continue;
                }

                this.InventoryPets.Remove(Pet);

                ServerPacket RemoveMessage = new ServerPacket(604);
                RemoveMessage.AppendUInt(PetId);
                GetClient().SendPacket(RemoveMessage);

                return true;
            }
            return false;
        }

        public void MovePetToRoom(uint PetId, uint RoomId)
        {
            if (RemovePet(PetId))
            {
                using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
                {
                    dbClient.AddParamWithValue("roomid", RoomId);
                    dbClient.AddParamWithValue("petid", PetId);
                    dbClient.ExecuteQuery("UPDATE user_pets SET room_id = @roomid, x = 0, y = 0, z = 0 WHERE id = @petid LIMIT 1");
                }
            }
        }

        public void RemoveItem(uint Id)
        {
            ServerPacket packet = new ServerPacket(99);
            packet.AppendUInt(Id);
            GetClient().SendPacket(packet);
            InventoryItems.Remove(GetItem(Id));
            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.ExecuteQuery("DELETE FROM user_items WHERE id = '" + Id + "' LIMIT 1");
            }
        }

        public ServerPacket SerializeItemInventory()
        {
            ServerPacket Message = new ServerPacket(140);
            Message.AppendInt32(this.ItemCount);
            foreach (var _item in InventoryItems)
            {
                _item.Serialize(Message, true);
            }
            Message.AppendInt32(this.ItemCount);
            return Message;
        }

        public ServerPacket SerializePetInventory()
        {
            ServerPacket Message = new ServerPacket(600);
            Message.AppendInt32(InventoryPets.Count);

            foreach (Pet Pet in InventoryPets)
            {
                Pet.SerializeInventory(Message);
            }

            return Message;
        }

        private GameClient GetClient()
        {
            return UberEnvironment.GetGame().GetClientManager().GetClientByHabbo(UserId);
        }
    }
}