/*
 * BUG Command
 * Posts bug to a REST API
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Net;
using System.IO;

namespace XNAGameConsole.Commands
{
    class BugCommand : IConsoleCommand
    {
        public string Name
        {
            get { return "/bug"; }
        }
        public string Description
        {
            get { return "Report a bug Email | Bug"; }
        }

        private readonly Game game;
        public BugCommand(Game game)
        {
            this.game = game;
        }

        public string Execute(string[] arguments)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in arguments)
            {
                builder.Append(value);
                builder.Append(' ');
            }
            string email = arguments[0];
            int first = email.Length + 1;
            string bug = builder.ToString().Remove(0, first);
            string verid = "p0nt1V3rC0d3mAYb3";
            string fullURL = "http://www.pontification.net/api/a8hsh89nXc9GAs3edQs.php?verid=" + verid + "&email=" + email + "&bug=" + bug;

            //Do HTTP Post Here
            WebClient client = new WebClient();
            Stream data = client.OpenRead(fullURL);
            data.Close();
            return ("Bug has been reported");
        }

    }
}
