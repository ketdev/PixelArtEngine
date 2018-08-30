using Stealth.Kernel;
using System;

namespace Stealth {
    public static class Program {
        private const int SCREEN_WIDTH = 1280*2;
        private const int SCREEN_HEIGHT = 720*2;
        
        [STAThread]
        static void Main() {
            using (var game = new GameEngine(SCREEN_WIDTH, SCREEN_HEIGHT))
                game.Run();
        }
    }
}
