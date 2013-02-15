using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace XNAGameConsole.Commands
{
    class PromptCommand : IConsoleCommand
    {
        public string Name
        {
            get { return "prompt"; }
        }

        public string Description
        {
            get { return "Change the Prompt"; }
        }

        private readonly Game game;
        public PromptCommand(Game game)
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

            GameConsoleOptions.Options.Prompt = "[" + builder.ToString()+"]$";
            return ("Prompt Changed To :: " + builder.ToString());
        }
    }
}