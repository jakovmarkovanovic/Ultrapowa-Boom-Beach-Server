using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UCS.Helpers.List;

namespace UCS.Logic
{
    using static Convert;
    using static ConfigurationManager;

    internal class ClientAvatar : Avatar
    {
        internal bool AccountBanned = false;

        internal byte AccountPrivileges = 0x00;

        // Long
        internal long AllianceId = 0;

        // String
        internal string AvatarName;

        internal long CurrentHomeId;
        internal string FacebookId;
        internal string FacebookToken;
        internal string GoogleId;
        internal string GoogleToken;

        // Int
        internal int HighID;

        internal string IPAddress;
        internal DateTime LastTickSaved;
        internal int LowID;

        //Datetime
        internal DateTime m_vAccountCreationDate;

        // Int
        internal int m_vActiveLayout;

        // Boolean
        internal bool m_vAndroid;

        internal int AvatarLevel;
        internal int m_vCurrentGems;
        internal int m_vExperience = 0;

        // Byte
        internal byte m_vNameChangingLeft = 0x02;

        internal byte m_vnameChosenByUser = 0x00;
        internal int m_vScore;
        internal string Region;
        internal int ReportedTimes = 0;

        // UInt
        internal uint TutorialStepsCount = 0x0A;

        internal long UserID;
        internal string UserToken;

        public ClientAvatar()
        {
            Achievements = new List<DataSlot>();
            AchievementsUnlocked = new List<DataSlot>();
        }

        public ClientAvatar(long id, string token) : this()
        {
            Random rnd = new Random();

            this.UserID = id;
            this.HighID = (int) (id >> 32);
            this.LowID = (int) (id & 0xffffffffL);
            this.UserToken = token;
            this.CurrentHomeId = id;
            this.AvatarLevel = /*ToInt32(AppSettings["startingLevel"])*/ 65;
            this.m_vCurrentGems = ToInt32(AppSettings["startingGems"]);
            this.m_vScore = AppSettings["startingTrophies"] == "random" ? rnd.Next(1500, 2000) : ToInt32(AppSettings["startingTrophies"]);
            this.AvatarName = "Clash of Lights";

            /*SetResourceCount(CSVManager.DataTables.GetResourceByName("Gold"), ToInt32(AppSettings["startingGold"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("Elixir"), ToInt32(AppSettings["startingElixir"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("DarkElixir"), ToInt32(AppSettings["startingDarkElixir"]));
            SetResourceCount(CSVManager.DataTables.GetResourceByName("Diamonds"), ToInt32(AppSettings["startingGems"]));*/
        }

        public List<DataSlot> Achievements { get; set; }
        public List<DataSlot> AchievementsUnlocked { get; set; }

        public byte[] Encode
        {
            get
            {
                List<byte> data = new List<byte>();
                data.AddLong(this.UserID); // User ID
                data.AddLong(this.UserID); // User ID

                data.AddInt(0);
                data.AddBool(false);

                data.AddString(this.AvatarName); // Username
                data.AddString(null);
                data.AddInt(this.AvatarLevel); // Level
                data.AddInt(0); // Exp

                data.AddInt(m_vCurrentGems); // Gems
                data.AddInt(m_vCurrentGems); // Gems
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(m_vScore); // Trophies
                data.AddInt(0); 
                data.AddByte(0);
                data.AddInt(0); // League ID ?

                data.AddInt(0); // Resource Cap?

                data.AddInt(4); // Resource Count

                for (int i = 0; i < 4; i++) // Just for testing atm.
                {
                    data.AddInt(3000000 + i + 1); // Resource ID
                    data.AddInt(99999999); // Resource Count
                }

                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);

                data.AddInt(0); // Unknown - DataSlots

                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);

                data.AddInt(0); // Unknown - DataSlots

                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);

                data.AddInt(0); // Unknown
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);
                data.AddInt(0);

                data.AddHexa("00FFFFFFFF0000000000FFFFFFFFFFFFFFFFFFFFFFFF0000000000000000000000000000000D5449445F5455544F5249414C3100000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000003FFFFFFFF0000000000FFFFFFFF00000000FFFFFFFF0000000000000000000000010000000D5449445F5455544F5249414C3200000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000003FFFFFFFF0000000000FFFFFFFF00000000FFFFFFFF0000000000000000000000020000000D5449445F5455544F5249414C3300000000FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF00000003FFFFFFFF0000000000FFFFFFFF00000000FFFFFFFF00000000000000000000000000000000000000000000000000000000000000000000000000000006FFFFFFFF0000000000FFFFFFFFFFFFFFFFFFFFFFFF0000000000000000000000000000000000000000000000000000000000000000000000000000000FFFFFFFFF0000000000FFFFFFFFFFFFFFFFFFFFFFFF0000000000000000000000000000000000000000000000FFFFFFFF00000000030000000000000000FFFFFFFFFFFFFFFF00000000000000000000000000FFFFFFFF14C0B64858F751E600000000000000000000000244450000000642617965726E");

                return data.ToArray();
            }
        }

        public void LoadFromJSON(string jsonString)
        {
            JObject jsonObject = JObject.Parse(jsonString);
            this.UserID = jsonObject["avatar_id"].ToObject<long>();
            this.AvatarName = jsonObject["avatar_name"].ToObject<string>();
            this.AvatarLevel = jsonObject["avatar_level"].ToObject<int>();
            this.m_vScore = jsonObject["avatar_trophies"].ToObject<int>();
            this.m_vCurrentGems = jsonObject["gems"].ToObject<int>();
        }

        public string SaveToJSON
        {
            get
            {
                JObject jsonData = new JObject
                {
                    { "avatar_id", this.UserID},
                    { "avatar_name", this.AvatarName},
                    { "gems", this.m_vCurrentGems},
                    { "avatar_level", this.AvatarLevel},
                    { "avatar_trophies", this.m_vScore}
                };

                return JsonConvert.SerializeObject(jsonData, Formatting.Indented);
            }
        }

        public void InitializeAccountCreationDate()
        {
            m_vAccountCreationDate = DateTime.Now;
        }

        public void SetName(string name)
        {
            AvatarName = name;
            if (m_vnameChosenByUser == 0x01)
                m_vNameChangingLeft = 0x01;
            else
                m_vNameChangingLeft = 0x02;
            TutorialStepsCount = 0x0D;
        }
    }
}