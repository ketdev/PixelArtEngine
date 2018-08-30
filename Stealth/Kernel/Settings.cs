using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Stealth.Kernel {

    public static class Settings {
        public const String Title = "Stealth";

        // Blackboard service strings
        public const String ContentManager = "ContentManager";
        public const String GraphicsManager = "GraphicsManager";

        // Layer priorities
        public enum PriorityLayer : int {

            // Update (lowest goes first)
            Kernel = 1, // Diagnose, InputListener
            Management = 2, 
            GameLogic = 3, // Director

            // Draw (highest drawn last)
            Background = 4, // TextureRender
            Map = 5,
            Overlay = 6, // StringRender
        }

    }
}
