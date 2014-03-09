using System;
using System.Text.RegularExpressions;

namespace bReChTbOt.Core
{
    public class Bot
    {
        private CommandParser parser;

        public Bot()
        {
            parser = new CommandParser();
        }

        public void Run()
        {
            while (true)
            {
                /* Normalize the input:
                 * 1) Trim leading and trailing whitespaces
                 * 2) Replace all whitespaces with a regular space
                 * */
                String line = Console.ReadLine().Trim();
                Regex.Replace(line, "\\s+", " "); 

                //Let the parser deal with it
                parser.Parse(line);
            }
        }
    }
}
