using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Uber.Messages;

using Uber.HabboHotel.Rooms;
using Uber.HabboHotel.Pathfinding;

namespace Uber.HabboHotel.RoomBots
{
    class PetBot : BotAI
    {
        private int SpeechTimer;
        private int ActionTimer;
        private int EnergyTimer;

        // TO-DO: This needs cleaning up BADLY, If anyone wants to attempt cleaning up my mess, go for it (Y) (Half done - Shorty)

        public PetBot(int VirtualId)
        {
            this.SpeechTimer = new Random((VirtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 60);
            this.ActionTimer = new Random((VirtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 30 + VirtualId);
            this.EnergyTimer = new Random((VirtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 60);
        }

        private void RemovePetStatus()
        {
            RoomUser Pet = GetRoomUser();

            // Remove Status
            Pet.Statusses.Remove("sit");
            Pet.Statusses.Remove("lay");
            Pet.Statusses.Remove("snf");
            Pet.Statusses.Remove("eat");
            Pet.Statusses.Remove("ded");
            Pet.Statusses.Remove("jmp");
        }

        public override void OnSelfEnterRoom()
        {
            int randomX = UberEnvironment.GetRandomNumber(0, GetRoom().Model.MapSizeX);
            int randomY = UberEnvironment.GetRandomNumber(0, GetRoom().Model.MapSizeY);
            GetRoomUser().MoveTo(randomX, randomY);
        }

        public override void OnSelfLeaveRoom(bool Kicked) { }

        public override void OnUserEnterRoom(Rooms.RoomUser User)
        {
            if (User.GetClient().GetHabbo().Username.ToLower() == GetRoomUser().PetData.OwnerName.ToLower())
            {
                GetRoomUser().Chat(null, "*drools over master*", false);
            }
        }

        public override void OnUserLeaveRoom(GameClients.GameClient Client) { }

        #region Commands
        public override void OnUserSay(Rooms.RoomUser User, string Message)
        {
            RoomUser Pet = GetRoomUser();

            if (Message.ToLower().Equals(Pet.PetData.Name.ToLower()))
            {
                Pet.SetRot(Rotation.Calculate(Pet.X, Pet.Y, User.X, User.Y));
                return;
            }

            if (Message.ToLower().StartsWith(Pet.PetData.Name.ToLower() + " ") && User.GetClient().GetHabbo().Username.ToLower() == GetRoomUser().PetData.OwnerName.ToLower())
            {
                string Command = Message.Substring(Pet.PetData.Name.ToLower().Length + 1);

                int r = UberEnvironment.GetRandomNumber(1, 8); // Made Random

                if (Pet.PetData.Energy > 10 && r < 6 || Pet.PetData.Level > 15)
                {
                    RemovePetStatus(); // Remove Status

                    switch (Command)
                    {
                        // TODO - Level you can use the commands at...

                        #region free
                        case "free":
                            RemovePetStatus();

                            int randomX = UberEnvironment.GetRandomNumber(0, GetRoom().Model.MapSizeX);
                            int randomY = UberEnvironment.GetRandomNumber(0, GetRoom().Model.MapSizeY);

                            Pet.MoveTo(randomX, randomY);

                            Pet.PetData.AddExpirience(10); // Give XP

                            break;
                        #endregion

                        #region here
                        case "come":
                        case "here":

                            RemovePetStatus();

                            int NewX = User.X;
                            int NewY = User.Y;

                            ActionTimer = 30; // Reset ActionTimer

                            #region Rotation
                            if (User.RotBody == 4)
                            {
                                NewY = User.Y + 1;
                            }
                            else if (User.RotBody == 0)
                            {
                                NewY = User.Y - 1;
                            }
                            else if (User.RotBody == 6)
                            {
                                NewX = User.X - 1;
                            }
                            else if (User.RotBody == 2)
                            {
                                NewX = User.X + 1;
                            }
                            else if (User.RotBody == 3)
                            {
                                NewX = User.X + 1;
                                NewY = User.Y + 1;
                            }
                            else if (User.RotBody == 1)
                            {
                                NewX = User.X + 1;
                                NewY = User.Y - 1;
                            }
                            else if (User.RotBody == 7)
                            {
                                NewX = User.X - 1;
                                NewY = User.Y - 1;
                            }
                            else if (User.RotBody == 5)
                            {
                                NewX = User.X - 1;
                                NewY = User.Y + 1;
                            }
                            #endregion

                            Pet.PetData.AddExpirience(10); // Give XP

                            Pet.MoveTo(NewX, NewY);
                            break;
                        #endregion

                        #region sit
                        case "sit":
                            // Remove Status
                            RemovePetStatus();

                            Pet.PetData.AddExpirience(10); // Give XP

                            // Add Status
                            Pet.Statusses.Add("sit", Pet.Z.ToString());
                            ActionTimer = 25;
                            EnergyTimer = 10;
                            break;
                        #endregion

                        #region lay
                        case "lay":
                        case "down":
                            // Remove Status
                            RemovePetStatus();

                            // Add Status
                            Pet.Statusses.Add("lay", Pet.Z.ToString());

                            Pet.PetData.AddExpirience(10); // Give XP

                            ActionTimer = 30;
                            EnergyTimer = 5;
                            break;
                        #endregion

                        #region dead
                        case "play dead":
                        case "dead":
                            // Remove Status
                            RemovePetStatus();

                            // Add Status 
                            Pet.Statusses.Add("ded", Pet.Z.ToString());

                            Pet.PetData.AddExpirience(10); // Give XP

                            // Don't move to speak for a set amount of time.
                            SpeechTimer = 45;
                            ActionTimer = 30;

                            break;
                        #endregion

                        #region sleep
                        case "sleep":
                            // Remove Status
                            RemovePetStatus();

                            Pet.Chat(null, "ZzzZZZzzzzZzz", false);
                            Pet.Statusses.Add("lay", Pet.Z.ToString());

                            Pet.PetData.AddExpirience(10); // Give XP

                            // Don't move to speak for a set amount of time.
                            EnergyTimer = 5;
                            SpeechTimer = 30;
                            ActionTimer = 45;
                            break;
                        #endregion

                        #region jump
                        case "jump":
                            // Remove Status
                            RemovePetStatus();

                            // Add Status 
                            Pet.Statusses.Add("jmp", Pet.Z.ToString());

                            Pet.PetData.AddExpirience(10); // Give XP

                            // Don't move to speak for a set amount of time.
                            EnergyTimer = 5;
                            SpeechTimer = 10;
                            ActionTimer = 5;
                            break;
                        #endregion

                        default:
                            string[] Speech = { "*confused*", "What?", "Huh?", "What is that?", "hmm..?" };

                            Random RandomSpeech = new Random();
                            Pet.Chat(null, Speech[RandomSpeech.Next(0, Speech.Length - 1)], false);
                            break;
                    }
                    Pet.PetData.PetEnergy(false); // Remove Energy
                    Pet.PetData.PetEnergy(false); // Remove Energy

                }
                else
                {

                    RemovePetStatus(); // Remove Status

                    if (Pet.PetData.Energy < 10)
                    {
                        string[] Speech = { "ZzZzzzzz", "*sleeps*", "Tired... *sleeps*", "ZzZzZZzzzZZz", "zzZzzZzzz", "... Yawnn ..", "ZzZzzZ" };

                        Random RandomSpeech = new Random();
                        Pet.Chat(null, Speech[RandomSpeech.Next(0, Speech.Length - 1)], false);

                        Pet.Statusses.Add("lay", Pet.Z.ToString());

                        SpeechTimer = 50;
                        ActionTimer = 45;
                        EnergyTimer = 5;

                    }
                    else
                    {

                        string[] Speech = { "*sigh*", "*refuses*", " ... ", "Who do you think you are?", "you do it", "Grr!", "*laughs*", "Why?" };

                        Random RandomSpeech = new Random();
                        Pet.Chat(null, Speech[RandomSpeech.Next(0, Speech.Length - 1)], false);

                        Pet.PetData.PetEnergy(false); // Remove Energy
                    }
                }
            }
            Pet = null;
        }
        #endregion

        public override void OnUserShout(Rooms.RoomUser User, string Message) { }

        public override void OnTimerTick()
        {
            #region Speech
            if (SpeechTimer <= 0)
            {
                RoomUser Pet = GetRoomUser();

                if (Pet != null)
                {
                    Random RandomSpeech = new Random();
                    RemovePetStatus();

                    if (Pet.PetData.Type == 0 || Pet.PetData.Type == 3) // Dog & Terrier
                    {
                        string[] Speech = { "woof woof woof!!!", "hooooowl", "wooooof!", "Woof Woof!", "sit", "lay", "snf", "Woof" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    else if (Pet.PetData.Type == 1)
                    { // Cat

                        string[] Speech = { "meow", "meow...meOW", "muew..muew", "lay", "sit", "lay" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    else if (Pet.PetData.Type == 2)
                    { // Crocodile

                        string[] Speech = { "Rrrr....Grrrrrg....", "*Mellow*", "Tick tock tick....", "*feels like eating my owner*", "Nom Nom Nom", ".. Yawwnn!", "snf" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    else if (Pet.PetData.Type == 4)
                    { // Bear

                        string[] Speech = { "*pack of fresh salmon please*", "Rawrrr!", "*sniff sniff*", "Yawnnnn..", "Rawr! ... Rawrrrrr?", "snf" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    else if (Pet.PetData.Type == 5)
                    { // Pig

                        string[] Speech = { "Oink Oink..", "*Mellow*", "Sniff... Sniff..", "snf", "Oink!", "snf", "lay", "oink" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    else if (Pet.PetData.Type == 6)
                    { // Lion

                        string[] Speech = { "Rawr..", "Sniff!", "Meow..err..roar", "ugh", "Roar!", "snf", "lay", "roar" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    else if (Pet.PetData.Type == 7)
                    { // Rhino

                        string[] Speech = { "Stomp.. stomp..", "*Sniff*", "Shhhh", "yay", "Stomp!", "snf", "lay", "stomp" };

                        string rSpeech = Speech[RandomSpeech.Next(0, Speech.Length - 1)];

                        if (rSpeech.Length != 3)
                        {
                            Pet.Chat(null, rSpeech, false);
                        }
                        else
                        {
                            if (!Pet.Statusses.ContainsKey(rSpeech))
                                Pet.Statusses.Add(rSpeech, Pet.Z.ToString());
                        }

                    }
                    Pet = null;
                }
                SpeechTimer = UberEnvironment.GetRandomNumber(20, 120);
            }
            else
            {
                SpeechTimer--;
            }
            #endregion

            if (ActionTimer <= 0)
            {
                // Remove Status
                RemovePetStatus();

                int randomX = UberEnvironment.GetRandomNumber(0, GetRoom().Model.MapSizeX);
                int randomY = UberEnvironment.GetRandomNumber(0, GetRoom().Model.MapSizeY);
                GetRoomUser().MoveTo(randomX, randomY);

                ActionTimer = UberEnvironment.GetRandomNumber(15, 40 + GetRoomUser().PetData.VirtualId);
            }
            else
            {
                ActionTimer--;
            }

            if (EnergyTimer <= 0)
            {
                RemovePetStatus(); // Remove Status

                RoomUser Pet = GetRoomUser();

                Pet.PetData.PetEnergy(true); // Add Energy

                EnergyTimer = UberEnvironment.GetRandomNumber(30, 120); // 2 Min Max
            }
            else
            {
                EnergyTimer--;
            }
        }
    }
}