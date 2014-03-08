using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bReChTbOt.Map
{
    public class MapFactory
    {
        private static MapFactory instance;

        private List<SuperRegion> SuperRegions { get; set; }
        private List<Region> Regions { get; set; }

        private MapFactory()
        {
            SuperRegions = new List<SuperRegion>();
            Regions = new List<Region>();
        }

        public static MapFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new MapFactory();
            }
            return instance;
        }

        public void AddSuperRegion(int id, int reward)
        {
            SuperRegions.Add(new SuperRegion() { ID = id, Reward = reward });
        }

        public void AddRegion(int id, int superRegionId)
        {
            Region region = new Region() { ID = id };
            Regions.Add(region);

            SuperRegions
                .Where(sr => sr.ID == superRegionId)
                .FirstOrDefault()
                .AddChildRegion(region);
        }

        public void SetRegionNeighbors(int id, String[] neighbors)
        {
            List<Region> neighborregions = 
                neighbors
                    .Select(n => Regions.Where(region => region.ID == Int32.Parse(n)).FirstOrDefault())
                    .ToList();

            Regions
                .Where(region => region.ID == id)
                .FirstOrDefault()
                .Neighbours = neighborregions;
        }

        public void MarkStartingRegions(String[] regions)
        {
            regions
                .ToList()
                .ForEach(
                    (r) =>
                    {
                        Regions
                            .Where(region => region.ID == Int32.Parse(r))
                            .FirstOrDefault()
                            .RegionStatus = RegionStatus.Starting;
                    }
                );
        }
    }
}
