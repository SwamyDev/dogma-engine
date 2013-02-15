using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAGameConsole.Commands
{
    class MouseCommand : IConsoleCommand
    {
        public string Name
        {
            get { return "mouse"; }
        }

        public string Description
        {
            get { return "Makes the cursor visible (on / off)"; }
        }

        private readonly Game game;
        public MouseCommand(Game game)
        {
            this.game = game;
        }

        public string Execute(string[] arguments)
        {
                if (string.Compare(arguments[0], "on", true) == 0)
                {
                    game.IsMouseVisible = true;
                    return "Mouse is Visible";
                }
                else if (string.Compare(arguments[0], "off", true) == 0)
                {
                    game.IsMouseVisible = false;
                    return "Mouse is Hidden";
                }
            
                return arguments[0] + " is not a valid command on or off only";
        }
    }
}