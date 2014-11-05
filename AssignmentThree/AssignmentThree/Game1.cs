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

namespace AssignmentThree
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<Room> rooms;
        Cube myCube;
        BasicEffect effect;
        float angle;
        Vector3 position;
        Camera camera;
        Texture2D box;

        KeyboardState oldKeyboardState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.AmbientLightColor = new Vector3(0.0f, 1.0f, 1.0f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            effect.LightingEnabled = true;

            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f,
                                (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 1f, 10f);
            
            Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);

            camera = new Camera(new Vector3(0f, 0f, -10f), 0, graphics);

            effect.Projection = camera.proj;
            effect.View = camera.view;

            myCube = new Cube();
            myCube.size = new Vector3(3, 3, 3);
            myCube.position = new Vector3(0, 0, 0);
            myCube.BuildShape();

            Labyrinth lab = new Labyrinth(4, 4);
            lab.GeneratePaths();
            rooms = lab.GenerateRooms(6);

            angle = 0;
            position = new Vector3(0, 0, 0);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            box = Content.Load<Texture2D>("wooden-crate");
            // TODO: use this.Content to load your game content here
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
            GetInput();

            camera.Update(position, angle, graphics);

            Matrix R = Matrix.Identity;
            Matrix T = Matrix.CreateTranslation(0.0f, 0f, 5f);
            Matrix S = Matrix.Identity;

            effect.World = S * R * T;
            effect.View = camera.view;
            effect.Projection = camera.proj;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0,0,1.0f));

            base.Update(gameTime);
        }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Left))
                angle = angle + 0.015f;
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Right))
                angle = angle - 0.015f;
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.W))
                position += Vector3.Transform(new Vector3(0, 0, 0.1f), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.S))
                position += Vector3.Transform(new Vector3(0, 0, -0.1f), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.A))
                position += Vector3.Transform(new Vector3(0.1f, 0, 0), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.D))
                position += Vector3.Transform(new Vector3(-0.1f, 0, 0), camera.rotationMatrix);

            if (angle > 2 * Math.PI) angle = 0;

            oldKeyboardState = keyboardState;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            myCube.RenderShape(GraphicsDevice, effect);

            bool alt = false;

            effect.TextureEnabled = true;
            effect.Texture = box;

            foreach (Room room in rooms)
            {
                //effect.AmbientLightColor = alt ? new Vector3(1.0f, 1.0f, 0.0f) : new Vector3(0.0f, 1.0f, 1.0f);
                effect.World = Matrix.CreateTranslation(room.position);
                room.RenderShape(GraphicsDevice, effect);
                alt = !alt;
            }

            base.Draw(gameTime);
        }
    }
}
