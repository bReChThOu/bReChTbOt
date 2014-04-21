// <copyright file="Region.cs">
//        Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
using bReChTbOt.Config;
using System;
using System.Collections.Generic;

namespace bReChTbOt.Map
{
    /// <summary>
    /// Class that defines a Region.
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int ID { get; set; }


        /// <summary>
        /// Gets or sets the player.
        /// </summary>
        /// <value>
        /// The player.
        /// </value>
        public Player Player { get; set; }


        /// <summary>
        /// Gets or sets the number of armies.
        /// </summary>
        /// <value>
        /// The number of armies.
        /// </value>
        public int NbrOfArmies { get; set; }


        /// <summary>
        /// Gets or sets the neighbours.
        /// </summary>
        /// <value>
        /// The neighbours.
        /// </value>
        public List<Region> Neighbours { get; set; }


        /// <summary>
        /// Gets or sets the region status.
        /// </summary>
        /// <value>
        /// The region status.
        /// </value>
        public RegionStatus RegionStatus { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Region"/> class.
        /// </summary>
        public Region()
        {
            Neighbours = new List<Region>();
            RegionStatus = RegionStatus.Initialized;
        }

        /// <summary>
        /// Updates the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="nbrOfArmies">The number of armies.</param>
        public void Update(Player player, int nbrOfArmies)
        {
            Player = player;
            NbrOfArmies = nbrOfArmies;
        }

    }
}
