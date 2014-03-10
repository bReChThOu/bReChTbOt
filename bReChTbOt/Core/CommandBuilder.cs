// <copyright file="CommandBuilder.cs">
//		Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
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
		/// Ouputs the specified line.
		/// </summary>
		/// <param name="line">The line.</param>
        public static void Ouput(String line)
        {
            Console.WriteLine(line);
        }
    }
}
