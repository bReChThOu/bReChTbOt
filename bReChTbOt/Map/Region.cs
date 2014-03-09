using bReChTbOt.Config;
using System.Collections.Generic;

namespace bReChTbOt.Map
{
    public class Region
    {
        public int ID { get; set; }
        public Player Player { get; set; }
        public int NbrOfArmies { get; set; }
        public List<Region> Neighbours { get; set; }
        public RegionStatus RegionStatus { get; set; }

        public Region()
        {
            Neighbours = new List<Region>();
            RegionStatus = RegionStatus.Initialized;
        }

        public void Update(Player player, int nbrOfArmies)
        {
            Player = player;
            NbrOfArmies = nbrOfArmies;
        }

    }
}
