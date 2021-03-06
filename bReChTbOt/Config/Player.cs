﻿// <copyright file="Player.cs">
//        Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
namespace bReChTbOt.Config
{
    /// <summary>
    /// Defines a Player
    /// </summary>
    public class Player
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }


        /// <summary>
        /// Gets or sets the type of the player.
        /// </summary>
        /// <value>
        /// The type of the player.
        /// </value>
        public PlayerType PlayerType { get; set; }

        public override string ToString()
        {
            return PlayerType.ToString();
        }
    }
}
