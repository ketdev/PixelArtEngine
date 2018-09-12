namespace Stealth.Kernel {
    public static class Settings {
        public const string Title = "Stealth";

        // Blackboard service strings
        public const string ContentManager = "ContentManager";
        public const string GraphicsManager = "GraphicsManager";
        public const string RenderData = "RenderData";

        // Layer priorities
        public enum PriorityLayer : int {
            // Update (lowest goes first)
            Kernel = 1, // Diagnose, InputListener
            Management = 2, 
            GameLogic = 3, // Director

            // Draw (highest drawn last)
            Skybox = 4, // SkyboxRender [OutputRT]
            Shadows = 5, // ShadowRender [ShadowRT]
            Models = 6, // ModelRender [OutputRT, BorderRT, DepthRT <- ShadowRT]
            Border = 7, // BorderPass [OutputRT, <- BorderRT, DepthRT]
            Overlay = 9, // GeometryRender, StringRender, TextureRender [OutputRT]
        }

    }
}
