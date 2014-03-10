// <copyright file="ConfigFactory.cs">
//		Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace bReChTbOt.Config
{
    public class ConfigFactory
    {
		/// <summary>
		/// The instance
		/// </summary>
        private static ConfigFactory instance;

		/// <summary>
		/// Gets or sets the players.
		/// </summary>
		/// <value>
		/// The players.
		/// </value>
        private List<Player> Players { get; set; }

		/// <summary>
		/// Gets or sets the game settings.
		/// </summary>
		/// <value>
		/// The game settings.
		/// </value>
        private GameSettings GameSettings { get; set; }


		/// <summary>
		/// Prevents a default instance of the <see cref="ConfigFactory"/> class from being created.
		/// </summary>
        private ConfigFactory()
        {
            Players = new List<Player>();
            Players.Add(new Player() { PlayerType = PlayerType.Neutral, Name = "Neutral" });
            GameSettings = new GameSettings();
        }

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns></returns>
        [DebuggerStepThrough]
        public static ConfigFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new ConfigFactory();
            }
            return instance;
        }

		/// <summary>
		/// Gets the name of the player.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
        public Player GetPlayerByName(String name)
        {
            return Players.Where(player => player.Name == name).FirstOrDefault();
        }

		/// <summary>
		/// Sets the name of my bot.
		/// </summary>
		/// <param name="botname">The botname.</param>
        public void SetMyBotName(String botname)
        {
            Players.Add(new Player() { PlayerType = PlayerType.Me, Name = botname });
        }

		/// <summary>
		/// Sets the name of the opponent bot.
		/// </summary>
		/// <param name="botname">The botname.</param>
        public void SetOpponentBotName(String botname)
        {
            Players.Add(new Player() { PlayerType = PlayerType.Opponent, Name = botname });
        }

		/// <summary>
		/// Sets the starting armies.
		/// </summary>
		/// <param name="startingArmies">The starting armies.</param>
        public void SetStartingArmies(int startingArmies)
        {
            GameSettings.StartingArmies = startingArmies;
        }

		/// <summary>
		/// Gets the starting armies.
		/// </summary>
		/// <returns></returns>
        public int GetStartingArmies()
        {
            return GameSettings.StartingArmies;
        }

    }
}
