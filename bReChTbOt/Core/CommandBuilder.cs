using bReChTbOt.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace bReChTbOt.Core
{
    public class CommandBuilder
    {
        public static void OutputStartingRegions(IEnumerable<Region> regions)
        {
            Ouput(String.Join(" ", regions.Select(region => region.ID.ToString()).ToArray())); //.ToString() needed for mono compliance
        }

        public static void Ouput(String line)
        {
            Console.WriteLine(line);
        }
    }
}
