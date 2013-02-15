using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pontification.ScreenManagement
{
    /**
     * Implement the IScreenContent interface, if you want to draw on a screen managed by the screen manager class of the Pontification Game Engine.
     */
    public interface IScreenContent
    {
        // Property which contain information about the screen the content is in.
        GameScreen CurrentScreen { get; set; }

        // LoadContent method is invoked when the screen is loaded to the screen manager.
        void LoadContent();

        // UnloadContent method is invoked when the screen is removed from the screen manager.
        void UnloadContent();

        // Update is invoked by the screen every frame, if the screen is currently active.
        void Update(GameTime gameTime);

        // The HandleInput event is called after the update event, if the current screen is active. It passes player input as a parameter.
        void HandleInput(GameTime gameTime, InputState input);

        // Draw is invoked by the screen after the HandleInput event, if the screen is currently visible.
        void Draw(SpriteBatch sb, GameTime gameTime);
    }
}
