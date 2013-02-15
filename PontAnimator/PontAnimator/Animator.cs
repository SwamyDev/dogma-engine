using System;
using System.Collections.Generic;
using System.Linq;
using Forms = System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace PontAnimator
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Animator : Microsoft.Xna.Framework.Game
    {
        public static Animator Instance;
        public Forms.Form Winform;
        public Dictionary<string, Texture2D> LoadedTextures = new Dictionary<string, Texture2D>();
        public bool ShowCollisionShape = true;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private IntPtr _drawSurface;
        private Animation _currentAnimation;

        public Animator(IntPtr drawSurface)
        {
            Instance = this;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 600;
            _graphics.PreferredBackBufferHeight = 450;
            
            Content.RootDirectory = "Content";

            _drawSurface = drawSurface;
            _graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Winform = (Forms.Form)Forms.Form.FromHandle(Window.Handle);
            Winform.VisibleChanged += new EventHandler(Animator_VisibleChanged);
            Winform.Size = new System.Drawing.Size(10, 10);

            Mouse.WindowHandle = drawSurface;
            resizebackbuffer(MainForm.Instance.showcase.Width, MainForm.Instance.showcase.Height);

            Winform.Hide();
        }

        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _drawSurface;
        }
        private void Animator_VisibleChanged(object sender, EventArgs e)
        {
            Winform.Hide();
            Winform.Size = new System.Drawing.Size(10, 10);
            Winform.Visible = false;
        }

        public void resizebackbuffer(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTexturesFromContentFolder();
        }

        public void LoadTexturesFromContentFolder()
        {
            for (int i = 0; i < MainForm.Instance.cobAnimations.Items.Count; i++)
            {
                var stripeName = (string)MainForm.Instance.cobAnimations.Items[i];

                if (LoadedTextures.ContainsKey(stripeName))
                    continue;

                Texture2D spriteTexture = null;

                using (FileStream fileStream = new FileStream(MainForm.Instance.txbContentPath.Text + "\\" + stripeName, FileMode.Open))
                {
                    if (fileStream != null)
                    {
                        spriteTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
                        LoadedTextures.Add(stripeName, spriteTexture);
                    }
                }
            }
        }

        // Sets animation to requested texture.
        public void SetAnimation(string texturePath, int frames, float framesPerSecond)
        {
            var spriteName = (string)MainForm.Instance.cobAnimations.SelectedItem;
            Texture2D spriteTexture = null;

            // See if we already have loaded this texture into memory
            if (LoadedTextures.ContainsKey(spriteName))
            {
                spriteTexture = LoadedTextures[spriteName];
            }
            else
            {
                using (FileStream fileStream = new FileStream(texturePath, FileMode.Open))
                {
                    if (fileStream != null)
                    {
                        spriteTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
                        LoadedTextures.Add(spriteName, spriteTexture);
                    }
                }
            }

            if (spriteTexture != null)
            {
                int frameWidth = spriteTexture.Width / frames;
                float duration = (1 / framesPerSecond) * frames;
                _currentAnimation = new Animation(spriteTexture, frameWidth, 1);
                _currentAnimation.PlayAnimation(0, frames, duration, true);
            }
        }

        public void UpdateAnimationSettings(int frames, int framesPerSecond)
        {
            if (_currentAnimation != null)
                _currentAnimation.UpdateSettings(frames, framesPerSecond);
        }

        public void StopAnimation()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.Reset();
                _currentAnimation.IsPaused = true;
            }
        }

        public void PauseAnimation()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.IsPaused = true;
            }
        }

        public void PlayAnimation()
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.IsPaused = false;
            }
        }

        /**
         * Saves a png to disk containing the whole animation sprite sheet, where the sprites
         * are orderd according to their column values.
         */
        public void GetAnimationSheet()
        {
            SerializableDictionary<string, int> stripeColumns = FolderSettings.Instance.StripeColumn;

            // Sort columns and get size of the needed render target.
            List<KeyValuePair<string, int>> columnPairs = new List<KeyValuePair<string, int>>();
            int targetWidth = 0;
            int targetHeight = 0;
            foreach (var pair in stripeColumns)
            {
                columnPairs.Add(pair);

                Texture2D texture = LoadedTextures[pair.Key];
                targetHeight += texture.Height;
                if (targetWidth < texture.Width)
                {
                    targetWidth = texture.Width;
                }
            }
            columnPairs.Sort((A, B) =>
                {
                    if (B.Value > A.Value)
                        return -1;
                    else
                        return 1;
                }
            );

            // Set up a render target to write the images on.
            RenderTarget2D renderTarget = new RenderTarget2D(GraphicsDevice, targetWidth, targetHeight);

            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.Transparent);
            _spriteBatch.Begin();
            
            // Draw images to render target.
            int startHeight = 0;
            foreach (var pair in columnPairs)
            {
                Texture2D stripe = LoadedTextures[pair.Key];
                _spriteBatch.Draw(stripe, new Vector2(0, startHeight), Color.White);
                startHeight += stripe.Height;
            }

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            // Save render target to disk.
            string path = System.Windows.Forms.Application.StartupPath + "\\AnimationSheets\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (Stream stream = File.OpenWrite(path + DateTime.Now.ToString("dd-mm-yy_HH-mm-ss") + ".png"))
            {
                if (stream != null)
                {
                    renderTarget.SaveAsPng(stream, renderTarget.Width, renderTarget.Height);
                    stream.Close();
                }
            }

            renderTarget.Dispose();
            renderTarget = null;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

            foreach (var pair in LoadedTextures)
            {
                pair.Value.Dispose();
            }

            LoadedTextures.Clear();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                MainForm.Instance.Close();
            }

            // TODO: Add your update logic here
            ShowCollisionShape = MainForm.Instance.chbShowCollisionShape.Checked;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(MainForm.Instance.GetBackgroundColor());
            var center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);

            _spriteBatch.Begin();
            if (_currentAnimation != null)
                _currentAnimation.Draw(gameTime, _spriteBatch, center, SpriteEffects.None);

            if (ShowCollisionShape)
            {
                // Draw collision shape around animation using Primitives class.
                var topLeft = center - new Vector2(29, 87);
                var topRight = center - new Vector2(-29, 87);
                var bottomRight = center + new Vector2(29, 87);
                var bottomLeft = center + new Vector2(-29, 87);

                // Draw transparent rectangle.
                Primitives.Instance.DrawRectangle(_spriteBatch, topLeft, bottomRight, new Color(100, 100, 100, 100));
                // Draw lines.
                Primitives.Instance.DrawLine(_spriteBatch, topLeft, topRight, Color.Yellow, 1);
                Primitives.Instance.DrawLine(_spriteBatch, topRight, bottomRight, Color.Yellow, 1);
                Primitives.Instance.DrawLine(_spriteBatch, bottomRight, bottomLeft, Color.Yellow, 1);
                Primitives.Instance.DrawLine(_spriteBatch, bottomLeft, topLeft, Color.Yellow, 1);
                // Draw edge points.
                Primitives.Instance.DrawPoint(_spriteBatch, topLeft, Color.Blue, 4);
                Primitives.Instance.DrawPoint(_spriteBatch, topRight, Color.Blue, 4);
                Primitives.Instance.DrawPoint(_spriteBatch, bottomLeft, Color.Blue, 4);
                Primitives.Instance.DrawPoint(_spriteBatch, bottomRight, Color.Blue, 4);
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
