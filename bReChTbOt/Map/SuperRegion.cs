using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bReChTbOt.Map
{
    public class SuperRegion
    {
        public int ID { get; set; }
        public int Reward { get; set; }

        private List<Region> ChildRegions;

        public SuperRegion()
        {
            ChildRegions = new List<Region>();
        }

        public void AddChildRegion(Region region)
        {
            ChildRegions.Add(region);
        }
    }
}
