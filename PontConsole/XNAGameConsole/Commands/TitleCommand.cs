using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAGameConsole.Commands
{
    class TitleCommand : IConsoleCommand
    {
        public string Name
        {
            get { return "title"; }
        }

        public string Description
        {
            get { return "Set the title of the window"; }
        }

        private readonly Game game;
        public TitleCommand(Game game)
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
            game.Window.Title = "" + builder.ToString();
            return ("Title Changed to :: " + builder.ToString());
        }
    }
}