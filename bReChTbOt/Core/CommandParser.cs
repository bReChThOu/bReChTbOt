﻿// <copyright file="CommandParser.cs">
//        Copyright (c) 2013 All Rights Reserved
// </copyright>
// <author>Brecht Houben</author>
// <date>10/03/2014</date>
using bReChTbOt.Config;
using bReChTbOt.Map;
using System;
using System.Linq;

namespace bReChTbOt.Core
{
    public class CommandParser
    {
        public CommandParser()
        {

        }

        public void Parse(String commandline)
        {
            if (!String.IsNullOrEmpty(commandline))
            {
                String[] commandargs = commandline.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (commandargs.Length > 0)
                {
                    String command = commandargs[0].ToLowerInvariant();
                    String subcommand = String.Empty;

                    switch (command)
                    {
                        case "settings":
                            subcommand = commandargs[1].ToLowerInvariant();
                            switch (subcommand)
                            {
                                default:
                                case "your_bot":
                                    String mybotname = commandargs[2];
                                    ConfigFactory.GetInstance().SetMyBotName(mybotname);
                                    break;
                                case "opponent_bot":
                                    String opponentbotname = commandargs[2];
                                    ConfigFactory.GetInstance().SetOpponentBotName(opponentbotname);
                                    break;
                                case "starting_armies":
                                    int armies = Int32.Parse(commandargs[2]);
                                    ConfigFactory.GetInstance().SetStartingArmies(armies);
                                    break;
                            }
                            break;
                        case "go":
                            subcommand = commandargs[1].Trim().ToLowerInvariant();
                            switch(subcommand)
                            {
                                case "place_armies":
                                    CommandBuilder.OutputArmyPlacements(MapFactory.GetInstance().PlaceArmies());
                                    break;
                                case "attack/transfer":
                                    CommandBuilder.OutputArmyTransfers(MapFactory.GetInstance().TransferArmies());
                                    break;
                            }
                            break;
                        case "setup_map":
                            subcommand = commandargs[1].Trim().ToLowerInvariant();
                            switch (subcommand)
                            {
                                default:
                                case "super_regions":
                                    for (int i = 2; i < commandargs.Length; i++)
                                    {
                                        int id = Int32.Parse(commandargs[i]);
                                        int reward = Int32.Parse(commandargs[++i]);
                                        MapFactory.GetInstance().AddSuperRegion(id, reward);
                                    }
                                    break;
                                case "regions":
                                    for (int i = 2; i < commandargs.Length; i++)
                                    {
                                        int id = Int32.Parse(commandargs[i]);
                                        int superRegionId = Int32.Parse(commandargs[++i]);
                                        MapFactory.GetInstance().AddRegion(id, superRegionId);
                                    }
                                    break;
                                case "neighbors":
                                    for (int i = 2; i < commandargs.Length; i++)
                                    {
                                        int id = Int32.Parse(commandargs[i]);
                                        String[] neighborstrings = commandargs[++i].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        MapFactory.GetInstance().SetRegionNeighbors(id, neighborstrings);
                                    }
                                    MapFactory.GetInstance().CalculateSuperRegionsBorders();
                                    break;
                            }
                            break;
                        case "pick_starting_regions":
                            String[] regions = commandargs.Skip(2).ToArray();
                            MapFactory.GetInstance().MarkStartingRegions(regions);
                            CommandBuilder.OutputStartingRegions(MapFactory.GetInstance().PickFavoriteStartingRegions());
                            break;
                        case "update_map":
                            ConfigFactory.GetInstance().SetRoundNumber(ConfigFactory.GetInstance().GetRoundNumber() + 1);
                             MapFactory.GetInstance().ClearRegions();
                            for (int i = 1; i < commandargs.Length; i++)
                            {
                                int regionid = Int32.Parse(commandargs[i]);
                                string player = commandargs[++i];
                                int nbrOfArmies = Int32.Parse(commandargs[++i]);
                                MapFactory.GetInstance().UpdateRegion(regionid, player, nbrOfArmies);
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
        }
    }
}
