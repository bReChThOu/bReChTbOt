﻿// <copyright file="ArmyPlacement.cs">
//        Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>14/03/2014</date>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bReChTbOt.Map
{
    /// <summary>
    /// Class that represents an army transfer
    /// </summary>
    public class ArmyTransfer
    {
        /// <summary>
        /// Gets or sets the armies.
        /// </summary>
        /// <value>
        /// The armies.
        /// </value>
        public int Armies { get; set; }


        /// <summary>
        /// Gets or sets the source region.
        /// </summary>
        /// <value>
        /// The source region.
        /// </value>
        public Region SourceRegion { get; set; }


        /// <summary>
        /// Gets or sets the target region.
        /// </summary>
        /// <value>
        /// The target region.
        /// </value>
        public Region TargetRegion { get; set; }
    }
}
