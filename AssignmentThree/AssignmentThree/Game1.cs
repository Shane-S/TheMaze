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

        Enemy chicken;

        #region Maze Variables
        Labyrinth lab;

        List<Room> rooms;
        List<Plane> northWalls;
        List<Plane> southWalls;
        List<Plane> westWalls;
        List<Plane> eastWalls;
        List<Plane> ceilWalls;
        List<Plane> floorWalls;
        private bool collisionOn;
        #endregion

        BasicEffect effect;
        #region Camera & Position
        float angleHorz;
        float angleVert;
        Camera camera;
        Vector3 position;
        Vector3 prevPosition;
        #endregion
        #region Lighting Variables
        Effect sceneEffect;
        float currentAmbience = DAYTIME_AMBIENCE;
        Flashlight light;
        bool fogOn = false;
        #endregion
        #region Textures & Models
        Texture2D box;
        Texture2D boxRed;
        Texture2D boxBlue;
        Texture2D boxGreen;
        Texture2D boxPurple;
        Texture2D boxYellow;
        Texture2D chickenTexture;
        Model chickenModel;
        #endregion
        #region Audio Variables
        Song currentSong;
        Song lightMusic;
        Song darkMusic;
        SoundEffect steps;
        SoundEffectInstance stepsInstance;
        AudioEmitter emitter;
        AudioListener listener;
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

            collisionOn = true;

            rooms = lab.GenerateRooms(6, ref northWalls, ref southWalls, ref eastWalls, ref westWalls, ref ceilWalls, ref floorWalls);
            #endregion
            #region Initialise Player
            angleHorz = 0;
            angleVert = 0;
            position = lab.GetPlayerSpawn();
            #endregion
            #region Initialise Input Manager
            inputMgr = new InputManager();

            inputMgr.AddNamedAction("pan_left", new InputAction(0, Keys.Left, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("pan_right", new InputAction(0, Keys.Right, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("pan_up", new InputAction(0, Keys.Up, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("pan_down", new InputAction(0, Keys.Down, InputAction.NO_ACTION_BUTTON));
            
            inputMgr.AddNamedAction("zoom_in", new InputAction(0, Keys.Z, Buttons.B));
            inputMgr.AddNamedAction("zoom_out", new InputAction(InputAction.SHIFT, Keys.Z, Buttons.A));
            inputMgr.AddNamedAction("zoom_reset", new InputAction(0, Keys.C, Buttons.RightShoulder));

            inputMgr.AddNamedAction("move_left", new InputAction(0, Keys.F, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("move_right", new InputAction(0, Keys.H, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("move_forward", new InputAction(0, Keys.T, InputAction.NO_ACTION_BUTTON));
            inputMgr.AddNamedAction("move_back", new InputAction(0, Keys.G, InputAction.NO_ACTION_BUTTON));

            inputMgr.AddNamedAction("change_ambience", new InputAction(0, Keys.B, Buttons.X));
            inputMgr.AddNamedAction("power_flashlight", new InputAction(0, Keys.Space, Buttons.LeftShoulder));
            inputMgr.AddNamedAction("fog_toggle", new InputAction(0, Keys.V, Buttons.DPadUp));
            inputMgr.AddNamedAction("collision_toggle", new InputAction(0, Keys.W, Buttons.Y));

            inputMgr.AddNamedAction("reset", new InputAction(0, Keys.Home, Buttons.Start));

            inputMgr.AddNamedAction("toggle_music", new InputAction(0, Keys.OemPeriod, Buttons.DPadRight));

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

            light = new Flashlight(70f, 5f, 30f, Color.Wheat, sceneEffect);

            #region Load Box Textures
            box = Content.Load<Texture2D>("wooden-crate");
            boxRed = Content.Load<Texture2D>("wooden-crate-red");
            boxBlue = Content.Load<Texture2D>("wooden-crate-blue");
            boxGreen = Content.Load<Texture2D>("wooden-crate-green");
            boxPurple = Content.Load<Texture2D>("wooden-crate-purple");
            boxYellow = Content.Load<Texture2D>("wooden-crate-yellow");
            #endregion
            #region Initialise Chicken
            chickenTexture = Content.Load<Texture2D>("chicken_diffuse");
            chickenModel = Content.Load<Model>("chicken");
            chicken = new Enemy(position, position, chickenModel, chickenTexture, 0.5f);
            #endregion
            #region Load Audio
            darkMusic = Content.Load<Song>("Blood");
            lightMusic = Content.Load<Song>("Hyrule");
            currentSong = lightMusic;
            
            steps = Content.Load<SoundEffect>("footsteps");
            stepsInstance = steps.CreateInstance();

            emitter = new AudioEmitter();
            listener = new AudioListener();

            listener.Forward = camera.CameraLookAt;
            listener.Up = Vector3.UnitY;
            listener.Velocity = Vector3.Zero;
            listener.Position = camera.CameraPosition;

            MediaPlayer.Play(lightMusic);
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.1f;
            #endregion
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

            if (collisionOn)
            {
                HandleCollisions();
            }

            chicken.Update(gameTime, lab);
            camera.Update(position, angleHorz, angleVert);
            UpdateAudio(gameTime);
            base.Update(gameTime);
        }

        public void UpdateAudio(GameTime time)
        {
            #region Footsteps
            SoundState stepsState = stepsInstance.State;
            // We've moved during this frame
            if (prevPosition != position)
            {
                if (stepsState == SoundState.Stopped)
                    stepsInstance.Play();
                else if (stepsState == SoundState.Paused)
                    stepsInstance.Resume();
            }
            else if (stepsState == SoundState.Playing)
                stepsInstance.Pause();
            #endregion
            #region Background Music
            MediaState musicState = MediaPlayer.State;
            if (inputMgr.ActionOccurred("toggle_music", InputActionType.Pressed))
            {
                if (musicState == MediaState.Playing)
                    MediaPlayer.Pause();
                else
                    MediaPlayer.Resume();

                musicState = MediaPlayer.State;
            }

            if(inputMgr.ActionOccurred("change_ambience", InputActionType.Pressed))
            {
                currentSong = currentSong == darkMusic ? lightMusic : darkMusic;
                MediaPlayer.Stop();
                MediaPlayer.Play(currentSong);

                if(musicState == MediaState.Paused)
                    MediaPlayer.Pause();
            }

            if(inputMgr.ActionOccurred("fog_toggle", InputActionType.Pressed))
            {
                if (fogOn)
                    MediaPlayer.Volume *= 0.5f;
                else
                    MediaPlayer.Volume *= 2;
            }
            #endregion
            #region Directional Audio
            Vector3 playerVel = (position - prevPosition) / (float)time.ElapsedGameTime.TotalSeconds;
            Vector3 chickenVel = (chicken.TargetPos - chicken.Position) / (float)time.ElapsedGameTime.TotalSeconds;
            Vector3 chickenForward = chickenVel;

            chickenForward.Normalize();
            listener.Forward = camera.CameraLookAt;
            listener.Velocity = playerVel;
            listener.Position = camera.CameraPosition;

            emitter.Forward = chickenForward;
            emitter.Velocity = chickenVel;
            emitter.Position = chicken.Position;
            #endregion
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
            float radius = 1.3f;


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
            }
            limit = maxBoundCell.X - radius;
            if (position.X > limit)
            {
                position.X = limit;
            }
            limit = minBoundCell.Y + radius;
            if (position.Y < limit)  // above floor
            {
                position.Y = limit;
            }
            limit = maxBoundCell.Y - radius;
            if (position.Y > limit) // below floor
            {
                position.Y = limit;
            }
            limit = minBoundCell.Z + radius;
            if (position.Z > -limit)
            {
                position.Z = -limit;
            }
            limit = maxBoundCell.Z - radius;
            if (position.Z < -limit)
            {
                position.Z = -limit;
            }

            // corner checks
            if (!cell.northWall && !cell.eastWall)
            {
                Vector3 pos = position;
                pos.X += radius;
                pos.Z += radius;
                Cell otherCell = lab.GetCellFromPosition(pos);
                Cell cornerCell = lab.GetCellFromIndex(cell.x + 1, cell.y - 1);
                if (otherCell == cornerCell)
                {
                    position.Z = -(cell.y * 12 + radius);
                    position.X = cell.x * 12 + (12 - radius);
                }
            }

            if (!cell.northWall && !cell.westWall)
            {
                Vector3 pos = position;
                pos.X -= radius;
                pos.Z += radius;
                Cell otherCell = lab.GetCellFromPosition(pos);
                Cell cornerCell = lab.GetCellFromIndex(cell.x - 1, cell.y - 1);
                if (otherCell == cornerCell)
                {
                    position.Z = -(cell.y * 12 + radius);
                    position.X = cell.x * 12 + radius;
                }
            }

            if (!cell.southWall && !cell.eastWall)
            {
                Vector3 pos = position;
                pos.X += radius;
                pos.Z -= radius;
                Cell otherCell = lab.GetCellFromPosition(pos);
                Cell cornerCell = lab.GetCellFromIndex(cell.x + 1, cell.y + 1);
                if (otherCell == cornerCell)
                {
                    position.Z = -(cell.y * 12 + (12 - radius));
                    position.X = cell.x * 12 + (12 - radius);
                }
            }

            if (!cell.southWall && !cell.westWall)
            {
                Vector3 pos = position;
                pos.X -= radius;
                pos.Z -= radius;
                Cell otherCell = lab.GetCellFromPosition(pos);
                Cell cornerCell = lab.GetCellFromIndex(cell.x - 1, cell.y + 1);
                if (otherCell == cornerCell)
                {
                    position.Z = -(cell.y * 12 + (12 - radius));
                    position.X = cell.x * 12 + radius;
                }
            }
        }

        public void GetInput()
        {
            Vector2 d;
            prevPosition = position;

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
                camera.ResetZoom();
            }

            if (inputMgr.ActionOccurred("zoom_in", InputActionType.Down))
            {
                camera.ZoomIn();
            }

            if (inputMgr.ActionOccurred("zoom_out", InputActionType.Down))
            {
                camera.ZoomOut();
            }

            if(inputMgr.ActionOccurred("zoom_reset", InputActionType.Pressed))
            {
                camera.ResetZoom();
            }

            if (inputMgr.ActionOccurred("collision_toggle", InputActionType.Pressed))
            {
                collisionOn = !collisionOn;
            }

            if(inputMgr.ActionOccurred("fog_toggle", InputActionType.Pressed))
            {
                ToggleFog();
            }

            // Mouse movement
            inputMgr.GetMouseDiff(out d);
            angleHorz += d.X * 0.2f;
            angleVert -= d.Y * 0.2f;

            Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            inputMgr.UpdateMouseState(Mouse.GetState());
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
                MoveCamera(new Vector3(-leftStickX * 0.5f, 0, leftStickY * 0.5f));
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

            angleVert -= rightStickY * 2;
            angleHorz -= rightStickX * 2;
        }

        /// <summary>
        /// Turns the fog effect on or off.
        /// </summary>
        private void ToggleFog()
        {
            fogOn = !fogOn;
            camera.FarClip = fogOn ? Camera.FOG_FAR_CLIP : Camera.NORMAL_FAR_CLIP;
        }

        private void DrawSceneBasic()
        {
            GraphicsDevice.Clear(Color.Black);

            effect.FogEnabled = true;
            effect.FogColor = Color.Tomato.ToVector3();
            effect.FogStart = 0f;
            effect.FogEnd = 55f;
            effect.World = Matrix.Identity;
            effect.View = camera.View;
            effect.Projection = camera.Projection;
            effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0, 0, 1.0f));
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
            GraphicsDevice.Clear(Color.Gray);
            
            sceneEffect.CurrentTechnique = sceneEffect.Techniques["Technique1"];
            sceneEffect.Parameters["View"].SetValue(camera.View);
            sceneEffect.Parameters["Projection"].SetValue(camera.Projection);
            sceneEffect.Parameters["AmbientLightingFactor"].SetValue(currentAmbience);
            sceneEffect.Parameters["LightPos"].SetValue(camera.CameraPosition);
            sceneEffect.Parameters["CameraLookAt"].SetValue(camera.CameraLookAt);
            sceneEffect.Parameters["FarPlane"].SetValue(camera.FarClip);
            sceneEffect.Parameters["FogColour"].SetValue(Color.Gray.ToVector4());
            sceneEffect.Parameters["FogOn"].SetValue(fogOn);


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

            Matrix chickenWorld = Matrix.CreateRotationY(chicken.Orientation) * Matrix.CreateScale(0.3f) * Matrix.CreateTranslation(chicken.Position);
            Matrix[] boneTransforms = new Matrix[chicken.EnemyModel.Bones.Count];
            
            chicken.EnemyModel.CopyAbsoluteBoneTransformsTo(boneTransforms);
            foreach (ModelMesh mm in chicken.EnemyModel.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                    mmp.Effect = sceneEffect;
                    Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(boneTransforms[mm.ParentBone.Index]));
                    mmp.Effect.Parameters["World"].SetValue(boneTransforms[mm.ParentBone.Index] * chickenWorld);
                    mmp.Effect.Parameters["BoxTexture"].SetValue(chicken.Texture);
                }
                mm.Draw();
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

            base.Draw(gameTime);
        }
    }
}
