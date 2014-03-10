using bReChTbOt.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        [DebuggerStepThrough]
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

			/* 
			 * Neighbors are only given once.
			 * E.g.: A has neighbour B, but the game won't tell us B has neighbour A.
			 * 
			 * We have to define that relation explicitly
			 * */
			neighborregions.ForEach(
				(neighbor) =>
				{
					neighbor.Neighbours.Add(Regions.Where(region => region.ID == id).FirstOrDefault());
				}
			);

        }

        public void CalculateSuperRegionsBorders()
        {
            SuperRegions.ForEach((superregion) => { CalculateSuperRegionBorders(superregion); });
        }
        private void CalculateSuperRegionBorders(SuperRegion superregion)
        {
            //Calculate invasion paths and border territories for each Super Region
            int invasionPaths = superregion
                .ChildRegions
                .Where(region => region
                                    .Neighbours
                                    .Any(neighbor => GetSuperRegionForRegion(neighbor).ID != superregion.ID))
                .Count();


               
            int borderTerritories = superregion
                .ChildRegions
                .Select(region => region.Neighbours)
                .Select(
                    neighbors =>
                        neighbors
                            .Where(neighbor => GetSuperRegionForRegion(neighbor).ID != superregion.ID)
                            .Any())
                .Count();
            superregion.InvasionPaths = invasionPaths;
            superregion.BorderTerritories = borderTerritories;
            if (superregion.ID == 5)
            {
                Console.WriteLine("IP: {0}   BT: {1} ", invasionPaths, borderTerritories);
            }
        }

        public SuperRegion GetSuperRegionForRegion(Region region)
        {
            return SuperRegions
                .Where(superregion => superregion.ChildRegions.Contains(region))
                .FirstOrDefault();
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
                            .RegionStatus = RegionStatus.PossibleStartingRegion;
                    }
                );
        }

        public IEnumerable<Region> PickFavoriteStartingRegions()
        {
            /*
             * One key to victory is control over continents.
             * Players that hold continents at the beginning of a turn get bonus reinforcements 
             * in an amount roughly proportional to the size of the continent 
             * 
             * Thus, the key positions on the board are the territories on the borders of continents. 
             * 
             * 
             * Fase 1: Try to find the continents with the least regions
             * 
             * */
            return Regions
                .Where(region => region.RegionStatus == RegionStatus.PossibleStartingRegion)
                .OrderByDescending(region => GetSuperRegionForRegion(region).Priority)
                .Take(6);
        }

        public void ClearRegions()
        {
            Regions
                .ToList()
                .ForEach(
                    (region) =>
                    {
                        region.Player = null;
                        region.NbrOfArmies = 0;
                    }
                );
        }

        public void UpdateRegion(int regionid, String playername, int nbrOfArmies)
        {
            Regions
                .Where(region => region.ID == regionid)
                .FirstOrDefault()
                .Update(ConfigFactory.GetInstance().GetPlayerByName(playername) , nbrOfArmies);
        }

        public void PlaceArmies()
        {
            /*
             * Fase 1: Try to find the continents with the least regions and populate them with armies
             * 
             * */
          /*  return Regions
                .Where(region => region.RegionStatus == RegionStatus.PossibleStartingRegion)
                .OrderByDescending(region => GetSuperRegionForRegion(region).Priority)
                .OrderByDescending(region => region.Neighbours.Count)


            ConfigFactory.GetInstance().StartingArmies*/
        }
    }
}
