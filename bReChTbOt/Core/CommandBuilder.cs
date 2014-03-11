// <copyright file="CommandBuilder.cs">
//		Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
using bReChTbOt.Config;
using bReChTbOt.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bReChTbOt.Core
{
    public class CommandBuilder
    {
		/// <summary>
		/// Outputs the starting regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
        public static void OutputStartingRegions(IEnumerable<Region> regions)
        {
            Ouput(String.Join(" ", regions.Select(region => region.ID.ToString()).ToArray())); //.ToString() needed for mono compliance
        }

		/// <summary>
		/// Outputs the army placements.
		/// </summary>
		/// <param name="placements">The placements.</param>
		public static void OutputArmyPlacements(IEnumerable<ArmyPlacement> placements)
		{
			Ouput(String.Join(",", placements.Select(placement => String.Format("{0} place_armies {1} {2}", ConfigFactory.GetInstance().GetMyBotName(), placement.Region.ID, placement.Armies)).ToArray()));
		}

		/// <summary>
		/// Outputs the army transfers.
		/// </summary>
		/// <param name="transfers">The transfers.</param>
		public static void OutputArmyTransfers(IEnumerable<ArmyTransfer> transfers)
		{
			Ouput(String.Join(",", transfers.Select(transfer => String.Format("{0} attack/transfer {1} {2} {3}", ConfigFactory.GetInstance().GetMyBotName(), transfer.SourceRegion.ID, transfer.TargetRegion.ID, transfer.Armies)).ToArray()));
		}


		/// <summary>
		/// Ouputs the specified line.
		/// </summary>
		/// <param name="line">The line.</param>
        public static void Ouput(String line)
        {
            Console.WriteLine(line);
        }
    }
}
