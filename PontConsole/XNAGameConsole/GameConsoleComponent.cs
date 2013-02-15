using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAGameConsole.Commands;
using XNAGameConsole.KeyboardCapture;

namespace XNAGameConsole
{
    class GameConsoleComponent : DrawableGameComponent
    {
        public bool IsOpen
        {
            get
            {
                return renderer.IsOpen;
            }
        }
        public bool IsOpening
        {
            get
            {
                return renderer.IsOpening;
            }
        }
        private readonly GameConsole console;
        private readonly SpriteBatch spriteBatch;
        private readonly InputProcessor inputProcesser;
        private readonly Renderer renderer;

        public GameConsoleComponent(GameConsole console, Game game, SpriteBatch spriteBatch)
            : base(game)
        {
            this.console = console;
            EventInput.Initialize(game.Window);
            this.spriteBatch = spriteBatch;
            AddPresetCommands();
            inputProcesser = new InputProcessor(new CommandProcesser());
            inputProcesser.Open += (s, e) => renderer.Open();
            inputProcesser.Close += (s, e) => renderer.Close();

            renderer = new Renderer(game, spriteBatch, inputProcesser);
            var inbuiltCommands = new IConsoleCommand[] 
            {
                new ClearScreenCommand(inputProcesser),
                new ExitCommand(game),
                new MouseCommand(game),
                new TitleCommand(game),
                new PromptCommand(game),
                new BugCommand(game),
                new HelpCommand()
            };
            GameConsoleOptions.Commands.AddRange(inbuiltCommands);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!console.Enabled)
            {
                return;
            }
            spriteBatch.Begin();
            renderer.Draw(gameTime);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            if (!console.Enabled)
            {
                return;
            }
            renderer.Update(gameTime);
            base.Update(gameTime);
        }

        public void WriteLine(string text)
        {
            inputProcesser.AddToOutput(text);
        }

        void AddPresetCommands()
        {
            
        }
    }
}