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
        private Scene scene;
        
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
            scene = new Scene(graphics, Content);
            base.Initialize();
        }
        
        protected override void Update(GameTime gameTime) {
            // Paused game
            if (!IsActive) return;            
            scene.Update();
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime) {            
            scene.Draw();
            base.Draw(gameTime);
        }
    }
}
