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
        public const float DAYTIME_AMBIENCE = 0.4f;
        public const float NIGHTTIME_AMBIENCE = -0.94f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region Maze Variables
        Labyrinth lab;

        List<Room> rooms;
        List<Plane> northWalls;
        List<Plane> southWalls;
        List<Plane> westWalls;
        List<Plane> eastWalls;
        List<Plane> ceilWalls;
        List<Plane> floorWalls;
        #endregion

        Cube myCube;
        BasicEffect effect;
        float angleHorz;
        float angleVert;
        Vector3 position;
        Camera camera;

        #region Lighting Variables
        Effect sceneEffect;
        float currentAmbience = DAYTIME_AMBIENCE;
        Flashlight light;
        #endregion
        #region Textures
        Texture2D box;
        Texture2D boxRed;
        Texture2D boxBlue;
        Texture2D boxGreen;
        Texture2D boxPurple;
        Texture2D boxYellow;
        #endregion

        InputManager inputMgr;

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
            #region Initialise Effect
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
            effect.DirectionalLight0.Enabled = true;
            effect.DirectionalLight0.DiffuseColor = Vector3.One;
            effect.DirectionalLight0.Direction = Vector3.Normalize(Vector3.One);
            effect.LightingEnabled = true;
            #endregion
            #region Initialise Camera & Viewing
            Matrix projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4.0f,
                                (float)this.Window.ClientBounds.Width / (float)this.Window.ClientBounds.Height, 1f, 10f);
            
            Matrix V = Matrix.CreateTranslation(0f, 0f, -10f);
            camera = new Camera(graphics.GraphicsDevice);
            #endregion
            #region Generate Labyrinth
            lab = new Labyrinth(10, 10);
            lab.GeneratePaths();

            northWalls = new List<Plane>();
            southWalls = new List<Plane>();
            westWalls = new List<Plane>();
            eastWalls = new List<Plane>();
            ceilWalls = new List<Plane>();
            floorWalls = new List<Plane>();

            rooms = lab.GenerateRooms(6, ref northWalls, ref southWalls, ref eastWalls, ref westWalls, ref ceilWalls, ref floorWalls);
            #endregion
            #region Initialise Player
            angleHorz = 0;
            angleVert = 0;
            position = lab.GetPlayerSpawn();
            #endregion
            #region Initialise Cube
            myCube = new Cube();
            myCube.size = new Vector3(3, 3, 3);
            myCube.position = position;
            myCube.BuildShape();
            #endregion
            #region Initialise Input Manager
            inputMgr = new InputManager();

            inputMgr.AddNamedAction("pan_left", new InputAction(Keys.Left, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("pan_right", new InputAction(Keys.Right, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("pan_up", new InputAction(Keys.Up, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("pan_down", new InputAction(Keys.Down, InputAction.NO_ACTION_BUTTON));

            inputMgr.AddNamedAction("move_left", new InputAction(Keys.A, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("move_right", new InputAction(Keys.D, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("move_forward", new InputAction(Keys.W, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("move_back", new InputAction(Keys.S, InputAction.NO_ACTION_BUTTON));

            inputMgr.AddNamedAction("reset", new InputAction(Keys.Home, Buttons.Start));

            inputMgr.AddNamedAction("change_ambience", new InputAction(Keys.B, Buttons.B));
            inputMgr.AddNamedAction("power_flashlight", new InputAction(Keys.Space, Buttons.RightShoulder));
            #endregion
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
            sceneEffect = Content.Load<Effect>("Lighting");

            light = new Flashlight(70f, 5f, 10f, Color.Wheat, sceneEffect);

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

            camera.Update(position, angleHorz, angleVert);
            
            base.Update(gameTime);
        }

        private void MoveCamera(Vector3 move)
        {
            Matrix rot = Matrix.CreateRotationY(MathHelper.ToRadians(angleHorz));

            move = Vector3.Transform(move, rot);

            position += move;
        }

        
        private void HandleCollisions()
        {
            Cell cell = lab.GetCellFromPosition(position);
            Vector3 minBoundCell = new Vector3(0, 0, 0);
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
            Vector2 d;
            inputMgr.Update();

            // Allows the game to exit
            if (inputMgr.ExitWasPressed())
                this.Exit();

            DoCameraPanning();
            DoPlayerMovement();
            #region Check for Lighting Changes
            if (inputMgr.ActionOccurred("change_ambience", InputActionType.Pressed))
                currentAmbience = currentAmbience == DAYTIME_AMBIENCE ? NIGHTTIME_AMBIENCE : DAYTIME_AMBIENCE;
            if (inputMgr.ActionOccurred("power_flashlight", InputActionType.Pressed))
                light.power();
            #endregion
            if (inputMgr.ActionOccurred("reset", InputActionType.Pressed))
            {
                angleHorz = 0;
                angleVert = 0;
                position = lab.GetPlayerSpawn();
            }

            // Mouse movement
            inputMgr.GetMouseDiff(out d);
            angleHorz += d.X * 0.2f;
            angleVert -= d.Y * 0.2f;

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            inputMgr.UpdateMouseState(Mouse.GetState());
            //if (angleHorz > 2 * Math.PI) angleHorz = 0;
            //if (angleVert > 2 * Math.PI) angleVert = 0;
        }

        /// <summary>
        /// Moves the player according to player input.
        /// </summary>
        private void DoPlayerMovement()
        {
            float leftStickX = inputMgr.CurGamepadState.ThumbSticks.Left.X;
            float leftStickY = inputMgr.CurGamepadState.ThumbSticks.Left.Y;

            if (inputMgr.ActionOccurred("move_forward", InputActionType.Down))
                MoveCamera(new Vector3(0, 0, 0.2f));
            if (inputMgr.ActionOccurred("move_back", InputActionType.Down))
                MoveCamera(new Vector3(0, 0, -0.2f));
            if (inputMgr.ActionOccurred("move_right", InputActionType.Down))
                MoveCamera(new Vector3(-0.2f, 0, 0));
            if (inputMgr.ActionOccurred("move_left", InputActionType.Down))
                MoveCamera(new Vector3(0.2f, 0, 0));

            if (leftStickX != 0 || leftStickY != 0)
                MoveCamera(new Vector3(leftStickX, 0, leftStickY));
        }

        /// <summary>
        /// Pans the camera according to player input.
        /// </summary>
        private void DoCameraPanning()
        {
            float rightStickX = inputMgr.CurGamepadState.ThumbSticks.Right.X;
            float rightStickY = inputMgr.CurGamepadState.ThumbSticks.Right.Y;

            if (inputMgr.ActionOccurred("pan_left", InputActionType.Down))
                angleHorz = angleHorz + 2f;
            if (inputMgr.ActionOccurred("pan_right", InputActionType.Down))
                angleHorz = angleHorz - 2f;
            if (inputMgr.ActionOccurred("pan_up", InputActionType.Down))
                angleVert = angleVert - 2f;
            if (inputMgr.ActionOccurred("pan_down", InputActionType.Down))
                angleVert = angleVert + 2f;

            angleVert += rightStickY * 2;
            angleHorz += rightStickX * 2;
        }

        private void DrawSceneBasic()
        {
            GraphicsDevice.Clear(Color.Black);


            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0, 0, 1.0f));

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
        }

        private void DrawSceneLit()
        {
            GraphicsDevice.Clear(Color.Black);
            Matrix reflected;
            Microsoft.Xna.Framework.Plane p = new Microsoft.Xna.Framework.Plane(Vector3.UnitX, 0);
            Matrix.CreateReflection(ref p, out reflected);
            sceneEffect.CurrentTechnique = sceneEffect.Techniques["Technique1"];
            sceneEffect.Parameters["ViewProjection"].SetValue(camera.View * camera.Projection);
            sceneEffect.Parameters["AmbientLightingFactor"].SetValue(currentAmbience);
            sceneEffect.Parameters["LightPos"].SetValue(camera.CameraPosition);
            sceneEffect.Parameters["CameraLookAt"].SetValue(camera.CameraLookAt);
            Vector3 offset = new Vector3(6, 0, -6);

            foreach (Plane wall in northWalls)
            {
                sceneEffect.Parameters["BoxTexture"].SetValue(box);
                sceneEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(wall.position + offset));
                wall.RenderShape(GraphicsDevice, sceneEffect);
            }

            foreach (Plane wall in southWalls)
            {
                sceneEffect.Parameters["BoxTexture"].SetValue(boxBlue);
                sceneEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(wall.position + offset));
                wall.RenderShape(GraphicsDevice, sceneEffect);
            }

            foreach (Plane wall in westWalls)
            {
                sceneEffect.Parameters["BoxTexture"].SetValue(boxRed);
                sceneEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(wall.position + offset));
                wall.RenderShape(GraphicsDevice, sceneEffect);
                
            }

            foreach (Plane wall in eastWalls)
            {
                sceneEffect.Parameters["BoxTexture"].SetValue(boxGreen);
                sceneEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(wall.position + offset));
                wall.RenderShape(GraphicsDevice, sceneEffect);
            }

            foreach (Plane wall in ceilWalls)
            {
                sceneEffect.Parameters["BoxTexture"].SetValue(boxYellow);
                sceneEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(wall.position + offset));
                wall.RenderShape(GraphicsDevice, sceneEffect);
            }

            foreach (Plane wall in floorWalls)
            {
                sceneEffect.Parameters["BoxTexture"].SetValue(boxPurple);
                sceneEffect.Parameters["World"].SetValue(Matrix.CreateTranslation(wall.position + offset));
                wall.RenderShape(GraphicsDevice, sceneEffect);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            DrawSceneLit();
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
