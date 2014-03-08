using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bReChTbOt.Map
{
    public class Region
    {
        public int ID { get; set; }
        public List<Region> Neighbours { get; set; }

        public RegionStatus RegionStatus { get; set; }

        public Region()
        {
            Neighbours = new List<Region>();
            RegionStatus = RegionStatus.Initialized;
        }

    }
}
