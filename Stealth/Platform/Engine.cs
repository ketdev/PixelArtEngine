using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Stealth.Unit;
using System;

namespace Stealth.Platform {
    class Engine : Game {

        public const String ContentRoot = "Content";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        AudioListener listener;
        AudioEmitter emitter;

        Texture2D texture;
        Vector2 position;

        Song song;
        SoundEffect sfx;

        private SpriteFont font;

        private EntityWorld entityWorld;

        private TimeSpan elapsedTime;
        private int frameCounter;
        private int frameRate;


        public Engine(int width, int height) {
            Content.RootDirectory = ContentRoot;

            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = width,
                PreferredBackBufferHeight = height,
                SynchronizeWithVerticalRetrace = true,
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight,
#if ANDROID
                IsFullScreen = true,
#endif
            };
            
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = new System.TimeSpan(166667); // 60fps
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize() {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent() {

            //EntitySystem.BlackBoard.SetEntry("ContentManager", Content);
            //EntitySystem.BlackBoard.SetEntry("GraphicsDevice", GraphicsDevice);
            //EntitySystem.BlackBoard.SetEntry("SpriteBatch", spriteBatch);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            listener = new AudioListener();
            emitter = new AudioEmitter();
            
            // TODO: use this.Content to load your game content here

            texture = Content.Load<Texture2D>("grass");

            sfx = Content.Load<SoundEffect>("PlayerJump");

            // Fire and forget play
            sfx.Play();

            //// Play that can be manipulated after the fact
            //var instance = sfx.CreateInstance();
            //instance.IsLooped = true;
            //instance.Play();

            //song = Content.Load<Song>("PlayerJump");
            //
            //MediaPlayer.Play(song);
            ////  Uncomment the following line will also loop the song
            ////  MediaPlayer.IsRepeating = true;
            //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

            font = Content.Load<SpriteFont>("Arial");

            entityWorld = new EntityWorld();
            entityWorld.InitializeAll(System.Reflection.Assembly.GetExecutingAssembly());

            var e = entityWorld.CreateEntity(); // you can pass an unique ID as first parameter.
            e.AddComponentFromPool<Position>();
        }
        //void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e) {
        //    // 0.0f is silent, 1.0f is full volume
        //    MediaPlayer.Volume -= 0.1f;
        //    MediaPlayer.Play(song);
        //}

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            // Paused game
            if (!IsActive) return;

            // Poll for current state
            var gamepad = GamePad.GetState(PlayerIndex.One);
            var gamepadCap = GamePad.GetCapabilities(PlayerIndex.One);
            var keyb = Keyboard.GetState();
            var mouse = Mouse.GetState();
            var touchCol = TouchPanel.GetState();

            if (gamepad.Buttons.Back == ButtonState.Pressed
                || keyb.IsKeyDown(Keys.Escape)) {
                Exit();
            }

            entityWorld.Update();

            ++this.frameCounter;
            elapsedTime += gameTime.ElapsedGameTime;
            var OneSecond = TimeSpan.FromSeconds(1);
            if (elapsedTime > OneSecond) {
                elapsedTime -= OneSecond;
                frameRate = frameCounter;
                frameCounter = 0;
            }

            // TODO: Add your update logic here

            if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
                if (SoundEffect.MasterVolume == 0.0f)
                    SoundEffect.MasterVolume = 1.0f;
                else
                    SoundEffect.MasterVolume = 0.0f;
            }
            
            // gamepad
            if(gamepadCap.HasAButton && gamepad.Buttons.A == ButtonState.Pressed) {
                var inst = sfx.CreateInstance();
                listener.Position = new Vector3(-1,0,0);
                inst.Apply3D(listener, emitter);
                sfx.Play();
            }

            if (gamepadCap.HasLeftXThumbStick && gamepadCap.HasLeftYThumbStick) {
                var d = gamepad.ThumbSticks.Left * 10.0f;
                d.Y = -d.Y; // invert Y
                position += d;
            }
            if (gamepad.DPad.Right == ButtonState.Pressed)
                position.X += 10;
            if (gamepad.DPad.Left == ButtonState.Pressed)
                position.X -= 10;
            if (gamepad.DPad.Up == ButtonState.Pressed)
                position.Y -= 10;
            if (gamepad.DPad.Down == ButtonState.Pressed)
                position.Y += 10;

            //// mouse
            //position.X = mouse.X;
            //position.Y = mouse.Y;

            // touch
            foreach (var touch in touchCol) {
                position = touch.Position;
            }
            
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            string fps = string.Format("fps: {0}", this.frameRate);
#if DEBUG
            string entityCount = string.Format("Active entities: {0}", this.entityWorld.EntityManager.ActiveEntities.Count);
            string removedEntityCount = string.Format("Removed entities: {0}", this.entityWorld.EntityManager.TotalRemoved);
            string totalEntityCount = string.Format("Total entities: {0}", this.entityWorld.EntityManager.TotalCreated);
#endif

            GraphicsDevice.Clear(new Color(0xff49d8a7));

            // TODO: Add your drawing code here
            
            spriteBatch.Begin();
            entityWorld.Draw();
            spriteBatch.Draw(texture, position, Color.White);

            // draw strings
            spriteBatch.DrawString(font, fps, new Vector2(32, 32), Color.Yellow);
#if DEBUG
            spriteBatch.DrawString(font, entityCount, new Vector2(32, 62), Color.Yellow);
            spriteBatch.DrawString(font, removedEntityCount, new Vector2(32, 92), Color.Yellow);
            spriteBatch.DrawString(font, totalEntityCount, new Vector2(32, 122), Color.Yellow);
#endif

            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
