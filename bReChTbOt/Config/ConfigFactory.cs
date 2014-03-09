using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace bReChTbOt.Config
{
    public class ConfigFactory
    {
        private static ConfigFactory instance;

        private List<Player> Players { get; set; }
        private GameSettings GameSettings { get; set; }


        private ConfigFactory()
        {
            Players = new List<Player>();
            Players.Add(new Player() { PlayerType = PlayerType.Neutral, Name = "Neutral" });
            GameSettings = new GameSettings();
        }

        [DebuggerStepThrough]
        public static ConfigFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigFactory();
            }
            return instance;
        }

        public Player GetPlayerByName(String name)
        {
            return Players.Where(player => player.Name == name).FirstOrDefault();
        }

        public void SetMyBotName(String botname)
        {
            Players.Add(new Player() { PlayerType = PlayerType.Me, Name = botname });
        }

        public void SetOpponentBotName(String botname)
        {
            Players.Add(new Player() { PlayerType = PlayerType.Opponent, Name = botname });
        }

        public void SetStartingArmies(int startingArmies)
        {
            GameSettings.StartingArmies = startingArmies;
        }

        public int GetStartingArmies()
        {
            return GameSettings.StartingArmies;
        }

    }
}
