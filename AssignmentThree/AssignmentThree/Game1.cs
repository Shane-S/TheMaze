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

        Labyrinth lab;

        List<Room> rooms;
        List<Plane> northWalls;
        List<Plane> southWalls;
        List<Plane> westWalls;
        List<Plane> eastWalls;
        List<Plane> ceilWalls;
        List<Plane> floorWalls;

        Cube myCube;
        BasicEffect effect;
        float angleHorz;
        float angleVert;
        Vector3 position;
        Camera camera;
        Texture2D box;
        Texture2D boxRed;
        Texture2D boxBlue;
        Texture2D boxGreen;
        Texture2D boxPurple;
        Texture2D boxYellow;

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
            effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            effect.LightingEnabled = true;

            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f,
                                (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 1f, 10f);
            
            Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);

            camera = new Camera(new Vector3(0f, 0f, -10f), 0, 0, graphics);

            effect.Projection = camera.proj;
            effect.View = camera.view;

            myCube = new Cube();
            myCube.size = new Vector3(3, 3, 3);
            myCube.position = new Vector3(0, 0, 0);
            myCube.BuildShape();

            lab = new Labyrinth(10, 10);
            lab.GeneratePaths();

            northWalls = new List<Plane>();
            southWalls = new List<Plane>();
            westWalls = new List<Plane>();
            eastWalls = new List<Plane>();
            ceilWalls = new List<Plane>();
            floorWalls = new List<Plane>();

            rooms = lab.GenerateRooms(6, ref northWalls, ref southWalls, ref eastWalls, ref westWalls, ref ceilWalls, ref floorWalls);

            angleHorz = 0;
            angleVert = 0;
            position = lab.GetPlayerSpawn();

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
            boxRed = Content.Load<Texture2D>("wooden-crate-red");
            boxBlue = Content.Load<Texture2D>("wooden-crate-blue");
            boxGreen = Content.Load<Texture2D>("wooden-crate-green");
            boxPurple = Content.Load<Texture2D>("wooden-crate-purple");
            boxYellow = Content.Load<Texture2D>("wooden-crate-yellow");
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
            HandleCollisions();

            camera.Update(position, angleHorz, angleVert, graphics);

            angleVert = 0;
            angleHorz = 0;

            Matrix R = Matrix.Identity;
            Matrix T = Matrix.CreateTranslation(0.0f, 0f, 5f);
            Matrix S = Matrix.Identity;

            effect.World = S * R * T;
            effect.View = camera.view;
            effect.Projection = camera.proj;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0,0,1.0f));

            base.Update(gameTime);
        }

        private void HandleCollisions()
        {
            Cell cell = lab.GetCellFromPosition(position);
            Vector3 minBoundCell = new Vector3(0,0,0);
            Vector3 maxBoundCell = new Vector3(10 * 12, 12, 10 * 12);
            float radius = 2;


            if (cell.northWall)
            {
                minBoundCell.Z = cell.y * 12;
            }
            if (cell.southWall)
            {
                maxBoundCell.Z = cell.y * 12 + 12;
            }
            if (cell.eastWall)
            {
                maxBoundCell.X = cell.x * 12 + 12;
            }
            if (cell.westWall)
            {
                minBoundCell.X = cell.x * 12;
            }

            float limit = minBoundCell.X + radius;
            if (position.X < limit)
            {
                position.X = limit;
                //velocity.X = -velocity.X * GameConstants::Physics::GroundRestitution;
            }
            limit = maxBoundCell.X - radius;
            if (position.X > limit)
            {
                position.X = limit;
                //velocity.x = -velocity.x + GameConstants::Physics::GroundRestitution;
            }
            limit = minBoundCell.Y + radius;
            if (position.Y < limit)  // above floor
            {
                position.Y = limit;
                //velocity.y = -velocity.y * GameConstants::Physics::GroundRestitution;
            }
            limit = maxBoundCell.Y - radius;
            if (position.Y > limit) // below floor
            {
                position.Y = limit;
                //velocity.y = -velocity.y * GameConstants::Physics::GroundRestitution;
            }
            limit = minBoundCell.Z + radius;
            if (position.Z > -limit)
            {
                position.Z = -limit;
                //velocity.z = -velocity.z * GameConstants::Physics::GroundRestitution;
            }
            limit = maxBoundCell.Z - radius;
            if (position.Z < -limit)
            {
                position.Z = -limit;
                //velocity.z = -velocity.z * GameConstants::Physics::GroundRestitution;
            }

            // Detect walls of current cell
            //m_player->SetPos(Vector3<double>(position.x, position.y, position.z));
            //m_player->SetVelocity(Vector3<double>(velocity.x, velocity.y, velocity.z));
        }

        public void GetInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                this.Exit();

            // TODO: Add your update logic here
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Left))
                angleHorz = angleHorz + 0.03f;
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Right))
                angleHorz = angleHorz - 0.03f;
            if (GamePad.GetState(PlayerIndex.One).DPad.Up == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Up))
                angleVert = angleVert + 0.03f;
            if (GamePad.GetState(PlayerIndex.One).DPad.Down == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Down))
                angleVert = angleVert - 0.03f;
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.W))
                position += Vector3.Transform(new Vector3(0, 0, 0.2f), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.S))
                position += Vector3.Transform(new Vector3(0, 0, -0.2f), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).DPad.Left == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.A))
                position += Vector3.Transform(new Vector3(0.2f, 0, 0), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).DPad.Right == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.D))
                position += Vector3.Transform(new Vector3(-0.2f, 0, 0), camera.rotationMatrix);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Home))
            {
                angleHorz = 0;
                angleVert = 0;
                position = lab.GetPlayerSpawn();
            }
            
            if (angleHorz > 2 * Math.PI) angleHorz = 0;
            if (angleVert > 2 * Math.PI) angleVert = 0;

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

            //bool alt = false;

            effect.TextureEnabled = true;

            Vector3 offset = new Vector3(6, 0, -6);

            foreach (Plane wall in northWalls)
            {
                effect.Texture = box;
                effect.World = Matrix.CreateTranslation(wall.position + offset);
                wall.RenderShape(GraphicsDevice, effect);
            }

            foreach (Plane wall in southWalls)
            {
                effect.Texture = boxBlue;
                effect.World = Matrix.CreateTranslation(wall.position + offset);
                wall.RenderShape(GraphicsDevice, effect);
            }

            foreach (Plane wall in westWalls)
            {
                effect.Texture = boxRed;
                effect.World = Matrix.CreateTranslation(wall.position + offset);
                wall.RenderShape(GraphicsDevice, effect);
            }

            foreach (Plane wall in eastWalls)
            {
                effect.Texture = boxGreen;
                effect.World = Matrix.CreateTranslation(wall.position + offset);
                wall.RenderShape(GraphicsDevice, effect);
            }

            foreach (Plane wall in ceilWalls)
            {
                effect.Texture = boxYellow;
                effect.World = Matrix.CreateTranslation(wall.position + offset);
                wall.RenderShape(GraphicsDevice, effect);
            }

            foreach (Plane wall in floorWalls)
            {
                effect.Texture = boxPurple;
                effect.World = Matrix.CreateTranslation(wall.position + offset);
                wall.RenderShape(GraphicsDevice, effect);
            }

            //foreach (Room room in rooms)
            //{
            //    //effect.AmbientLightColor = alt ? new Vector3(1.0f, 1.0f, 0.0f) : new Vector3(0.0f, 1.0f, 1.0f);
            //    effect.World = Matrix.CreateTranslation(room.position);
            //    room.RenderShape(GraphicsDevice, effect);
            //    alt = !alt;
            //}

            base.Draw(gameTime);
        }
    }
}
