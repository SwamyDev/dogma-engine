using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAGameConsole;
using XNATextInput;

namespace XNAGameConsoleDemo.ConsoleCommands
{
    class MovePlayerCommand:IConsoleCommand
    {
        public string Name
        {
            get { return "fuck"; }
        }

        public string Description
        {
            get { return "Fuck You!"; }
        }

        public string Execute(string[] arguments)
        {
            return "Fuck You! ";
        }
    }
}
