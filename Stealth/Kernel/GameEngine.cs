#if DEBUG
//#define NO_VSYNC
#endif

using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Stealth.Kernel {
    class GameEngine : Game {
        
        // Services
        private GraphicsDeviceManager graphics;
        private EntityWorld entityWorld;
        private RenderData renderData;


        public GameEngine(int width, int height) {
            IsMouseVisible = true;
#if NO_VSYNC
            IsFixedTimeStep = false;
#else
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(166667);  // 60 fps
#endif
            Content.RootDirectory = "Content";
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = width,
                PreferredBackBufferHeight = height,
#if NO_VSYNC
                SynchronizeWithVerticalRetrace = false,
#else
                SynchronizeWithVerticalRetrace = true,
#endif
                SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight,
#if ANDROID
                IsFullScreen = true,
#endif
            };
        }

        protected override void Initialize() {
            // Set a higher pixel size for whole scene
            var pixelSize = 3;
            renderData = new RenderData( GraphicsDevice, pixelSize );

            // initialization after graphics device is available
            entityWorld = new EntityWorld();
            EntitySystem.BlackBoard.SetEntry(Settings.ContentManager, Content);
            EntitySystem.BlackBoard.SetEntry(Settings.GraphicsManager, graphics);
            EntitySystem.BlackBoard.SetEntry(Settings.RenderData, renderData);
            entityWorld.InitializeAll(System.Reflection.Assembly.GetExecutingAssembly());

            // Load component contents
            base.Initialize();            
        }
        
        protected override void Update(GameTime gameTime) {
            // Paused game
            if (!IsActive) return;
            
            entityWorld.Update();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {
            renderData.Clear();
            
            entityWorld.Draw();

            renderData.Display();
            base.Draw(gameTime);
        }
    }
}
