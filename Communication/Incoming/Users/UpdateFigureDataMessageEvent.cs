using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uber.HabboHotel.GameClients;
using Uber.HabboHotel.Misc;
using Uber.HabboHotel.Rooms;
using Uber.Messages;
using Uber.Storage;

namespace Uber.Communication.Incoming.Users
{
    class UpdateFigureDataMessageEvent : IPacketEvent
    {
        public void parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().MutantPenalty)
            {
                Session.SendNotif("Because of a penalty or restriction on your account, you are not allowed to change your look.");
                return;
            }

            string Gender = Packet.PopFixedString().ToUpper();
            string Look = UberEnvironment.FilterInjectionChars(Packet.PopFixedString());

            if (!AntiMutant.ValidateLook(Look, Gender))
            {
                return;
            }

            Session.GetHabbo().Look = Look;
            Session.GetHabbo().Gender = Gender.ToLower();

            using (DatabaseClient dbClient = UberEnvironment.GetDatabase().GetClient())
            {
                dbClient.AddParamWithValue("look", Look);
                dbClient.AddParamWithValue("gender", Gender);
                dbClient.ExecuteQuery("UPDATE users SET look = @look, gender = @gender WHERE id = '" + Session.GetHabbo().Id + "' LIMIT 1");
            }

            UberEnvironment.GetGame().GetAchievementManager().UnlockAchievement(Session, 1, 1);

            ServerPacket packet = new ServerPacket(266);
            packet.AppendInt32(-1);
            packet.AppendStringWithBreak(Session.GetHabbo().Look);
            packet.AppendStringWithBreak(Session.GetHabbo().Gender.ToLower());
            packet.AppendStringWithBreak(Session.GetHabbo().Motto);
            Session.SendPacket(packet);

            if (Session.GetHabbo().InRoom)
            {
                Room Room = Session.GetHabbo().CurrentRoom;

                if (Room == null)
                {
                    return;
                }

                RoomUser User = Room.GetRoomUserByHabbo(Session.GetHabbo().Id);

                if (User == null)
                {
                    return;
                }

                ServerPacket RoomUpdate = new ServerPacket(266);
                RoomUpdate.AppendInt32(User.VirtualId);
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Look);
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Gender.ToLower());
                RoomUpdate.AppendStringWithBreak(Session.GetHabbo().Motto);
                Room.SendMessage(RoomUpdate);
            }
        }
    }
}
