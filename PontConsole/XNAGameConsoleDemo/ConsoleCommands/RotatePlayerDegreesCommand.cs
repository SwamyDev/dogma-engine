using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAGameConsole;
using XNATextInput;

namespace XNAGameConsoleDemo.ConsoleCommands
{
    class RotatePlayerDegreesCommand:IConsoleCommand
    {
        public string Name
        {
            get { return "shit"; }
        }

        public string Description
        {
            get { return "Returns Response"; }
        }

        public string Execute(string[] arguments)
        {
            return String.Format("You Fucked That Up!");
        }
    }
}
