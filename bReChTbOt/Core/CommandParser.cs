using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bReChTbOt.Config;
using bReChTbOt.Map;

namespace bReChTbOt.Core
{
    public class CommandParser
    {
        public CommandParser()
        {

        }

        public void Parse (String commandline)
        {
            String[] commandargs = commandline.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            String command = commandargs[0].ToLowerInvariant();
            String subcommand = String.Empty;

            switch(command)
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
                            break;
                    }
                    break;
                case "pick_starting_regions":
                    String[] regions = commandargs.Skip(2).ToArray();
                    MapFactory.GetInstance().MarkStartingRegions(regions);
                    break;
                default:
                    break;
            }
        }

        public void Ouput(String line)
        {

        }

    }
}
