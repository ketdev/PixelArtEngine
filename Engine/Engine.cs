#if DEBUG
//#define NO_VSYNC
#endif

using Artemis;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine {
    class Engine : Game {
        // Settings
        private const int SCREEN_WIDTH = 1280 * 2;
        private const int SCREEN_HEIGHT = 720 * 2;
        private const int PIXEL_SIZE = 3;
        private const string ASSETS_ROOT = "Assets";

        // Services
        private GraphicsDeviceManager graphics;
        private Scene scene;

        public Engine() {
            IsMouseVisible = true;
#if NO_VSYNC
            IsFixedTimeStep = false;
#else
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(166667);  // 60 fps
#endif
            Content.RootDirectory = ASSETS_ROOT;
            graphics = new GraphicsDeviceManager(this) {
                PreferredBackBufferWidth = SCREEN_WIDTH,
                PreferredBackBufferHeight = SCREEN_HEIGHT,
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
            scene = new Scene(graphics, Content, PIXEL_SIZE);
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
