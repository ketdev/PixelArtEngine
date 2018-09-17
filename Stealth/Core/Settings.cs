namespace Stealth.Kernel {
    public static class Settings {
        public const string Title = "Stealth";
        
        // System ordering - lowest goes first
        public enum PriorityLayer : int {
            // -- Update -- 

            Kernel = 1, // Diagnose, InputListener
            Management = 2, 
            GameLogic = 3, // Director


            // -- Rendering -- 

            // Scene Render            [GBuffer]
            SkyboxRender = 4,       // [OutputRT]
            ModelRender = 5,        // [OutputRT (+depth), BorderRT, DepthRT]
            ShadowVolumeRender = 6, // [OutputRT (stencil)]

            // Posteffects             [OutputRT]
            BorderPass = 7,         // [OutputRT <- BorderRT, DepthRT]
            ShadowVolumePass = 8,   // [OutputRT]

            // Interface
            Overlay = 9, // GeometryRender, StringRender, TextureRender [OutputRT]
        }

    }
}
