using System.Collections.Generic;

namespace bReChTbOt.Map
{
    public class SuperRegion
    {
        public int ID { get; set; }
        public int Reward { get; set; }

        public List<Region> ChildRegions { get; internal set;}

        public int Priority { get; internal set; }

        public SuperRegion()
        {
            ChildRegions = new List<Region>();
        }

        public void AddChildRegion(Region region)
        {
            ChildRegions.Add(region);
            Priority = (int) 1000 / ChildRegions.Count;
        }

        
    }
}
