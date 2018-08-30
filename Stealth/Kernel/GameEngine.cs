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
            // initialization after graphics device is available
            entityWorld = new EntityWorld();
            EntitySystem.BlackBoard.SetEntry(Settings.ContentManager, Content);
            EntitySystem.BlackBoard.SetEntry(Settings.GraphicsManager, graphics);
            entityWorld.InitializeAll(System.Reflection.Assembly.GetExecutingAssembly());
            
            // Load component contents
            base.Initialize();            
        }
        
        protected override void Update(GameTime gameTime) {
            // Paused game
            if (!IsActive) return;
            
            // Update all systems
            entityWorld.Update();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, default(Color), 1, 0);

            // Draw all systems
            entityWorld.Draw();            
            base.Draw(gameTime);
        }
    }
}
