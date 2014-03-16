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

		public void UpdateRegions(IEnumerable<ArmyPlacement> placements)
		{
			placements.ToList().ForEach(
				(placement) => 
				{
					UpdateRegion(placement.Region.ID, ConfigFactory.GetInstance().GetMyBotName(), placement.Region.NbrOfArmies + placement.Armies);
				}
			);
		}

		/// <summary>
		/// Places the armies.
		/// </summary>
        public IEnumerable<ArmyPlacement> PlaceArmies()
        {
			List<ArmyPlacement> placements = new List<ArmyPlacement>();
			int startingArmies = ConfigFactory.GetInstance().GetStartingArmies();

			var primaryRegion = Regions
				   .Where(region => region.NbrOfArmies < 100)
				   .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
				   .OrderByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Opponent))
				   .ThenByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Neutral))
				   .ThenBy(region => (GetSuperRegionForRegion(region).ChildRegions.Count(child => child.Player.PlayerType == PlayerType.Me)))
				   .ThenBy(region => region.NbrOfArmies)
				   .FirstOrDefault();

			if (startingArmies == 5)
			{
				var armyplacement = new ArmyPlacement() { Armies = 5, Region = primaryRegion };
				placements.Add(armyplacement);
			}
			if (startingArmies >= 7 && startingArmies <= 9)
			{
				var secundaryRegion = Regions
					.Where(region => GetSuperRegionForRegion(region) != GetSuperRegionForRegion(primaryRegion))
				   .Where(region => region.NbrOfArmies < 100)
				   .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
				   .OrderByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Opponent))
				   .ThenByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Neutral))
				   .ThenBy(region => (GetSuperRegionForRegion(region).ChildRegions.Count(child => child.Player.PlayerType == PlayerType.Me)))
				   .ThenBy(region => region.NbrOfArmies)
				   .FirstOrDefault();
				var armyplacement = new ArmyPlacement() { Armies = 5, Region = primaryRegion };
				placements.Add(armyplacement);
				armyplacement = new ArmyPlacement() { Armies = startingArmies - 5, Region = secundaryRegion };
				placements.Add(armyplacement);
			}
			if (startingArmies > 9 && startingArmies <= 18)
			{
				var secundaryRegion = Regions
					.Where(region => GetSuperRegionForRegion(region) != GetSuperRegionForRegion(primaryRegion))
				   .Where(region => region.NbrOfArmies < 100)
				   .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
				   .OrderByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Opponent))
				   .ThenByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Neutral))
				   .ThenBy(region => (GetSuperRegionForRegion(region).ChildRegions.Count(child => child.Player.PlayerType == PlayerType.Me)))
				   .ThenBy(region => region.NbrOfArmies)
				   .FirstOrDefault();

				var armyplacement = new ArmyPlacement() { Armies = 9, Region = primaryRegion };
				placements.Add(armyplacement);
				armyplacement = new ArmyPlacement() { Armies = startingArmies - 9, Region = secundaryRegion };
				placements.Add(armyplacement);
			}
			if (startingArmies > 18)
			{
				var secundaryRegion = Regions
					.Where(region => GetSuperRegionForRegion(region) != GetSuperRegionForRegion(primaryRegion))
				   .Where(region => region.NbrOfArmies < 100)
				   .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
				   .OrderByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Opponent))
				   .ThenByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Neutral && GetSuperRegionForRegion(region) == GetSuperRegionForRegion(neighbor)))
				   .ThenByDescending(region => region.Neighbours.Count(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Neutral && GetSuperRegionForRegion(region) != GetSuperRegionForRegion(neighbor)))
				   .ThenBy(region => (GetSuperRegionForRegion(region).ChildRegions.Count(child => child.Player.PlayerType == PlayerType.Me)))
				   .ThenBy(region => region.NbrOfArmies)
				   .FirstOrDefault();

				var armyplacement = new ArmyPlacement() { Armies = startingArmies - 9, Region = primaryRegion };
				placements.Add(armyplacement);
				armyplacement = new ArmyPlacement() { Armies = 9, Region = secundaryRegion };
				placements.Add(armyplacement);
			}

			UpdateRegions(placements);
			return placements;
        }

		public IEnumerable<ArmyTransfer> TransferArmies()
		{
			/*
			 * Inspect Border Territories foreach super region
			 * If there are no enemy armies sighted: let's conquer the continent
			 * 
			 * If there are enemy armies sighted: let's move some troops to defend those invasion paths
			 * */
			List<ArmyTransfer> transfers = new List<ArmyTransfer>();
			
			SuperRegions.ForEach(
				(superregion) =>
				{
					//Do i have any regions in this super region?
					bool skipSuperRegion = !superregion
						.ChildRegions
						.Any(region => region.Player != null && region.Player.PlayerType == PlayerType.Me);

					if (!skipSuperRegion)
					{
						int borderTerritoriesWithEnemyArmies = superregion.BorderTerritories
                            .Where(bt => bt.Neighbours.Any(btn => btn.Player != null && btn.Player.PlayerType == PlayerType.Opponent))
							.Where(bt => (bt.Player != null && bt.Player.PlayerType == PlayerType.Me) || bt.Neighbours.Any(btn => btn.Player != null && btn.Player.PlayerType == PlayerType.Me && GetSuperRegionForRegion(btn) == superregion))
							.Count();

						int regionsWithEnemyArmies = superregion.ChildRegions
							.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Opponent)
							.Count();

						bool transferDone = false;

						/*
						 * There is nobody in our way, let's conquer the continent, or even explore a new continent.
						 * */
						if (borderTerritoriesWithEnemyArmies == 0 && regionsWithEnemyArmies == 0)
						{
							Region targetRegion = null, sourceRegion = null;

							targetRegion = superregion
								.ChildRegions
								.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Neutral)
								.OrderByDescending(region =>
									FlattenIEnumerable(region.Neighbours.Where(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Me).Select(reg => reg.NbrOfArmies))
								)
								.FirstOrDefault();

							/* No neutral armies found, that should mean we own the continent.
							 * Let's explore the world and go to a new super region
							 * */
							if (targetRegion == null)
							{
								targetRegion = superregion
									.InvasionPaths
									.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Neutral)
									.OrderByDescending(region =>
										FlattenIEnumerable(region.Neighbours.Where(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Me).Select(reg => reg.NbrOfArmies))

									)
									.FirstOrDefault();

								if (targetRegion != null)
								{
									sourceRegion = targetRegion
									.Neighbours
									.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
									.OrderByDescending(region => region.NbrOfArmies)
									.FirstOrDefault();
								}
								else
								{
									targetRegion = superregion
									.InvasionPaths
									.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Opponent)
									.OrderByDescending(region =>
										FlattenIEnumerable(
											region.Neighbours
												.Where(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Me)
												.Where(neighbor => neighbor.NbrOfArmies > 5)
												.Where(neighbor => neighbor.NbrOfArmies > region.NbrOfArmies * 2)
												.Select(reg => reg.NbrOfArmies))
									)

									.FirstOrDefault();

									if (targetRegion != null)
									{
										sourceRegion = targetRegion
										.Neighbours
										.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
										.OrderByDescending(region => region.NbrOfArmies)
										.FirstOrDefault();
									}
								}
							}


							else
							{
								sourceRegion = targetRegion
								.Neighbours
								.Where(region => GetSuperRegionForRegion(region) == superregion)
								.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
								.OrderByDescending(region => region.NbrOfArmies)
								.FirstOrDefault();
							}

							
							if (sourceRegion != null && targetRegion != null)
							{
								if (sourceRegion.NbrOfArmies > 5)
								{
                                    ArmyTransfer transfer = new ArmyTransfer() { SourceRegion = sourceRegion, TargetRegion = targetRegion, Armies = GetRequiredArmies(sourceRegion, targetRegion) };
									transfers.Add(transfer);
									transferDone = true;
								}
							}

						}

						/*
						 * There is an enemy army nearby. Let's not let them take this continent.
						 * */

						if (borderTerritoriesWithEnemyArmies > 0 && !transferDone)
						{
							Region targetRegion = null, sourceRegion = null;

							Region invadingBorderTerritory = superregion
								.InvasionPaths
                                .Where(invasionpath => invasionpath.Player != null && invasionpath.Player.PlayerType == PlayerType.Opponent)
								.OrderByDescending(region => region.NbrOfArmies)
								.FirstOrDefault();

                            if (invadingBorderTerritory != null)
                            {

                                int enemyArmies = invadingBorderTerritory.NbrOfArmies;

                                /* Let's see if we can attack. There is  60% change per attacking army. 
                                 * We will be extra safe and use a 50% chance.
                                 * This means we'll need at least double as much armies as our opponent.
                                 * If this isn't the case, we'll send more armies to this region and defend our grounds.
                                 * 
                                 * */

                                var possibleAttackingRegion = superregion
                                    .ChildRegions
                                    .Where(region => region.Neighbours.Contains(invadingBorderTerritory))
                                    .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
                                    .Where(region => (region.NbrOfArmies >= enemyArmies * 2 || region.NbrOfArmies > 100) && region.NbrOfArmies > 5)
                                    .OrderByDescending(region => region.NbrOfArmies)
                                    .FirstOrDefault();

                                //We can attack!
                                if (possibleAttackingRegion != null)
                                {
                                    targetRegion = invadingBorderTerritory;
                                    sourceRegion = possibleAttackingRegion;
                                }

                                /* We can't attack, so let's defend.
                                 * We'll send armies to the region that can be attacked with the least number of armies
                                 * We'll prefer sending from regions that can't be attacked.
                                 **/
                                else
                                {
                                    targetRegion = invadingBorderTerritory
                                        .Neighbours
                                        .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
                                        .Where(region => GetSuperRegionForRegion(region) == superregion)
                                        .OrderBy(region => region.NbrOfArmies)
                                        .FirstOrDefault();

                                    if (targetRegion != null)
                                    {

                                        sourceRegion = targetRegion
                                            .Neighbours
                                            .Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
                                            .OrderByDescending(region => region.NbrOfArmies)
                                            .FirstOrDefault();
                                    }
                                }

                                if (sourceRegion != null && targetRegion != null)
                                {
                                    if (sourceRegion.NbrOfArmies > 5)
                                    {
                                        ArmyTransfer transfer = new ArmyTransfer() { SourceRegion = sourceRegion, TargetRegion = targetRegion, Armies = GetRequiredArmies(sourceRegion, targetRegion) };
                                        transfers.Add(transfer);
										transferDone = true;
                                    }
                                }
                            }
						}


						/*
						 * There is an enemy army in this super region. Let's not let them take the whole continent.
						 * */
						if (regionsWithEnemyArmies > 0 && !transferDone)
						{
							Region targetRegion = null, sourceRegion = null;

							Region hostileRegion = superregion
								.ChildRegions
								.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Opponent)
								.FirstOrDefault();

							int enemyArmies = hostileRegion.NbrOfArmies;

							/* Let's see if we can attack. There is  60% change per attacking army. 
							 * We will be extra safe and use a 50% chance.
							 * This means we'll need at least double as much armies as our opponent.
							 * If this isn't the case, we'll send more armies to this region and defend our grounds.
							 * 
							 * */

							var possibleAttackingRegion = superregion
								.ChildRegions
								.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
								.Where(region => region.Neighbours.Contains(hostileRegion))
								.Where(region => (region.NbrOfArmies >= enemyArmies * 2 || region.NbrOfArmies > 100) && region.NbrOfArmies > 5)
								.OrderByDescending(region => region.NbrOfArmies)
								.FirstOrDefault();

							//We can attack!
							if (possibleAttackingRegion != null)
							{
								targetRegion = hostileRegion;
								sourceRegion = possibleAttackingRegion;
							}

							/* We can't attack, so let's defend.
							 * We'll send armies to the region that can be attacked with the least number of armies
							 * We'll prefer sending from regions that can't be attacked.
							 **/
							else
							{
								targetRegion = hostileRegion
									.Neighbours
									.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
									.Where(region => GetSuperRegionForRegion(region) == superregion)
									.OrderBy(region => region.NbrOfArmies)
									.FirstOrDefault();

								if (targetRegion != null)
								{

									sourceRegion = targetRegion
										.Neighbours
										.Where(region => region.Player != null && region.Player.PlayerType == PlayerType.Me)
										.OrderByDescending(region => region.NbrOfArmies)
										.FirstOrDefault();
								}
							}

							if (sourceRegion != null && targetRegion != null)
							{
								if (sourceRegion.NbrOfArmies > 5)
								{
                                    ArmyTransfer transfer = new ArmyTransfer() { SourceRegion = sourceRegion, TargetRegion = targetRegion, Armies = GetRequiredArmies(sourceRegion, targetRegion) };
									transfers.Add(transfer);
									transferDone = true;
								}
							}
						}
						/*
						 * No moves or transfers made yet.
						 * Let's see if we can move some troops away from the inland where they can't do anything
						 * besides being stuck
						 * */
						if (!transferDone)
						{
							var stuckArmies = superregion.ChildRegions.Where(region => region.NbrOfArmies > 1 && region.Neighbours.All(neighbor => neighbor.Player != null && neighbor.Player.PlayerType == PlayerType.Me));
							if (stuckArmies.Count() > 0)
							{
								var stuckArmie = stuckArmies.First();
								//Let's see if there are neighbors that have foreign neighbors (neutral/opponent)
								var firstDegree = stuckArmie.Neighbours.Where(neighbor => neighbor.Neighbours.Any(neighborneighbor => neighborneighbor.Player != null && neighborneighbor.Player.PlayerType != PlayerType.Me));
								if (firstDegree.Count() > 0)
								{
									var freeway = firstDegree.First();
									ArmyTransfer transfer = new ArmyTransfer() { SourceRegion = stuckArmie, TargetRegion = freeway, Armies = GetRequiredArmies(stuckArmie, freeway) };
									transfers.Add(transfer);
								}
								//Nope, let's try second degree
								else
								{
									var secondDegree = stuckArmie.Neighbours
										.Where(neighbor => neighbor.Neighbours
										.Any(neighborneighbor => neighborneighbor.Neighbours.Any(neighborneighborneighbor => neighborneighborneighbor.Player != null && neighborneighborneighbor.Player.PlayerType != PlayerType.Me)));
									if (secondDegree.Count() > 0)
									{
										var freeway = secondDegree.First();
										ArmyTransfer transfer = new ArmyTransfer() { SourceRegion = stuckArmie, TargetRegion = freeway, Armies = GetRequiredArmies(stuckArmie, freeway) };
										transfers.Add(transfer);
									}
								}

							}
						}
					}
				}
			);

			//Don't attack the enemy if we are in the first round, instead skip this move
			if (ConfigFactory.GetInstance().GetRoundNumber() == 1)
			{
				transfers.ForEach(
					transfer =>
					{
						if (transfer.TargetRegion.Player != null &&
							transfer.TargetRegion.Player.PlayerType == PlayerType.Opponent)
						{
							transfers.Remove(transfer);
						}
					}
				);
			}

			return transfers;
		}

        private int GetRequiredArmies(Region sourceRegion, Region targetRegion)
        {
            int source = sourceRegion.NbrOfArmies;
            int target = targetRegion.NbrOfArmies;
            return sourceRegion.NbrOfArmies - 1;
        }

		private int FlattenIEnumerable(IEnumerable<int> ienumerable)
		{
			int sum = 0;
			ienumerable.ToList().ForEach(item => sum += item);
			return sum;
		}
    }
}
