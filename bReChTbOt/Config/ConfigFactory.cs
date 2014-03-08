using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bReChTbOt.Config
{
    public class ConfigFactory
    {
        private static ConfigFactory instance;

        private BotSettings MyBot { get; set; }
        private BotSettings OpponentBot { get; set; }

        private ConfigFactory()
        {
            MyBot = new BotSettings();
            OpponentBot = new BotSettings();
        }

        public static ConfigFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigFactory();
            }
            return instance;
        }

        public void SetMyBotName(String botname)
        {
            MyBot.Name = botname;
        }

        public void SetOpponentBotName(String botname)
        {
            OpponentBot.Name = botname;
        }
    }
}
