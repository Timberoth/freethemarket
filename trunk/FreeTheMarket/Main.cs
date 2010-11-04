using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

// Physics Library
using JigLibX.Physics;
using JigLibX.Geometry;
using JigLibX.Collision;

namespace FreeTheMarket
{    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        private Matrix _view;
        public Matrix View
        {
            get
            {
                return _view;
            }
        }

        private Matrix _projection;
        public Matrix Projection
        {
            get
            {
                return _projection;
            }
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // HACK This crap should not be here, but should be moved into another singleton.
        Texture2D testTexture;
        Vector2 spritePosition = Vector2.Zero;
        Vector2 spriteSpeed = new Vector2(100.0f, 100.0f);

        // Physics Objects
        BoxActor fallingBox;
        BoxActor immovableBox;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Create all the required singletons.

            // Initialize physics
            InitializePhysics();

            // Create projection matrix
            _projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f),
                (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight,
                0.1f,
                1000.0f
            );
        }

        private void InitializePhysics()
        {
            PhysicsSystem world = new PhysicsSystem();
#if XBOX360
            world.CollisionSystem = new CollisionSystemBrute();
#else
            world.CollisionSystem = new CollisionSystemSAP();
#endif

            fallingBox = new BoxActor(this, new Vector3(0, 50, 0), new Vector3(1, 1, 1));
            immovableBox = new BoxActor(this, new Vector3(0, -5, 0), new Vector3(5, 5, 5));
            immovableBox.Body.Immovable = true;
            Components.Add(fallingBox);
            Components.Add(immovableBox);
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

            // Initialize all the singletons
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            testTexture = Content.Load<Texture2D>("GlossyBall");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            float timeStep = (float)gameTime.ElapsedGameTime.Ticks / TimeSpan.TicksPerSecond;
            PhysicsSystem.CurrentPhysicsSystem.Integrate(timeStep);

            UpdateSprite(gameTime);

            base.Update(gameTime);

            _view = Matrix.CreateLookAt(
                new Vector3(0, 5, 20),
                fallingBox.Body.Position,
                Vector3.Up
            );
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            // Draw update calls
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(testTexture, spritePosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }


        void UpdateSprite(GameTime gameTime)
        {
            spritePosition += spriteSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int maxX = graphics.GraphicsDevice.Viewport.Width - testTexture.Width;
            int minX = 0;
            int maxY = graphics.GraphicsDevice.Viewport.Height - testTexture.Height;
            int minY = 0;

            if (spritePosition.X > maxX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = maxX;
            }
            else if (spritePosition.X < minX)
            {
                spriteSpeed.X *= -1;
                spritePosition.X = minX;
            }
            else if (spritePosition.Y > maxY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = maxY;
            }
            else if (spritePosition.Y < minY)
            {
                spriteSpeed.Y *= -1;
                spritePosition.Y = minY;
            }
        }
    }
}
