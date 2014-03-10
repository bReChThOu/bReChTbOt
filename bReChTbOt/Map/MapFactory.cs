// <copyright file="MapFactory.cs">
//		Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
using bReChTbOt.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace bReChTbOt.Map
{
	/// <summary>
	/// Map Factory class
	/// </summary>
    public class MapFactory
    {
		/// <summary>
		/// The instance
		/// </summary>
        private static MapFactory instance;

		/// <summary>
		/// Gets or sets the super regions.
		/// </summary>
		/// <value>
		/// The super regions.
		/// </value>
        private List<SuperRegion> SuperRegions { get; set; }

		/// <summary>
		/// Gets or sets the regions.
		/// </summary>
		/// <value>
		/// The regions.
		/// </value>
        private List<Region> Regions { get; set; }

		/// <summary>
		/// Prevents a default instance of the <see cref="MapFactory"/> class from being created.
		/// </summary>
        private MapFactory()
        {
            SuperRegions = new List<SuperRegion>();
            Regions = new List<Region>();
        }

		/// <summary>
		/// Gets the instance.
		/// </summary>
		/// <returns></returns>
        [DebuggerStepThrough]
        public static MapFactory GetInstance()
        {
            if (instance == null)
            {
                instance = new MapFactory();
            }
            return instance;
        }

		/// <summary>
		/// Adds the super region.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="reward">The reward.</param>
        public void AddSuperRegion(int id, int reward)
        {
            SuperRegions.Add(new SuperRegion() { ID = id, Reward = reward });
        }

		/// <summary>
		/// Adds the region.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="superRegionId">The super region identifier.</param>
        public void AddRegion(int id, int superRegionId)
        {
            Region region = new Region() { ID = id };
            Regions.Add(region);

            SuperRegions
                .Where(sr => sr.ID == superRegionId)
                .FirstOrDefault()
                .AddChildRegion(region);
        }

		/// <summary>
		/// Sets the region neighbors.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <param name="neighbors">The neighbors.</param>
        public void SetRegionNeighbors(int id, String[] neighbors)
        {
            List<Region> neighborregions = 
                neighbors
                    .Select(n => Regions.Where(region => region.ID == Int32.Parse(n)).FirstOrDefault())
                    .ToList();

            Regions
                .Where(region => region.ID == id)
                .FirstOrDefault()
                .Neighbours.AddRange(neighborregions);

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

		/// <summary>
		/// Calculates the super regions borders.
		/// </summary>
        public void CalculateSuperRegionsBorders()
        {
            SuperRegions.ForEach((superregion) => { CalculateSuperRegionBorders(superregion); });
        }
		/// <summary>
		/// Calculates the super region borders.
		/// </summary>
		/// <param name="superregion">The superregion.</param>
        private void CalculateSuperRegionBorders(SuperRegion superregion)
        {
            //Calculate invasion paths and border territories for each Super Region
			var invasionPaths = superregion
				.ChildRegions
				.SelectMany(region => region
									.Neighbours
									.Where(neighbor => GetSuperRegionForRegion(neighbor).ID != superregion.ID));


			var borderTerritories = superregion
			   .ChildRegions
				.Where(region => region
									.Neighbours
									.Any(neighbor => GetSuperRegionForRegion(neighbor).ID != superregion.ID));

            superregion.InvasionPaths = invasionPaths;
            superregion.BorderTerritories = borderTerritories;
        }

		/// <summary>
		/// Gets the super region for the region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns></returns>
        public SuperRegion GetSuperRegionForRegion(Region region)
        {
            return SuperRegions
                .Where(superregion => superregion.ChildRegions.Contains(region))
                .FirstOrDefault();
        }

		/// <summary>
		/// Marks the starting regions.
		/// </summary>
		/// <param name="regions">The regions.</param>
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

		/// <summary>
		/// Picks the favorite starting regions.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Clears the regions.
		/// </summary>
        public void ClearRegions()
        {
            Regions
                .ToList()
                .ForEach(
                    (region) =>
                    {
                        region.Player = new Player() { PlayerType = Config.PlayerType.Unknown };
                        region.NbrOfArmies = 0;
                    }
                );
        }

		/// <summary>
		/// Updates the region.
		/// </summary>
		/// <param name="regionid">The regionid.</param>
		/// <param name="playername">The playername.</param>
		/// <param name="nbrOfArmies">The number of armies.</param>
        public void UpdateRegion(int regionid, String playername, int nbrOfArmies)
        {
            Regions
                .Where(region => region.ID == regionid)
                .FirstOrDefault()
                .Update(ConfigFactory.GetInstance().GetPlayerByName(playername) , nbrOfArmies);
        }

		/// <summary>
		/// Places the armies.
		/// </summary>
        public IEnumerable<ArmyPlacement> PlaceArmies()
        {
            /*
             * Fase 1: Try to find the continents with the least regions and populate them with armies
             * 
             * */
            var primaryRegion = Regions
                .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
                .OrderByDescending(region => GetSuperRegionForRegion(region).Priority)
                .OrderByDescending(region => region.Neighbours.Count)
				.FirstOrDefault();

			var armyplacement = new ArmyPlacement() { Armies = ConfigFactory.GetInstance().GetStartingArmies(), Region = primaryRegion };

			List<ArmyPlacement> placements = new List<ArmyPlacement>();

			placements.Add(armyplacement);

			return placements;
        }
    }
}
