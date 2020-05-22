namespace Engine {
    // System ordering - lowest goes first
    public enum Layer : int {
        // -- Update -- 
        Kernel = 10000,             // Diagnose, InputListener
        Management = 20000,
        GameLogic = 30000,          // Director

        // -- Rendering -- 

        // Scene Render                [GBuffer]
        SkyboxRender = 40000,       // [OutputRT]
        ModelRender = 50000,        // [OutputRT (+depth), BorderRT, DepthRT]
        ShadowVolumeRender = 60000, // [OutputRT (stencil)]

        // Posteffects                 [OutputRT]
        BorderPass = 70000,         // [OutputRT <- BorderRT, DepthRT]
        ShadowVolumePass = 80000,   // [OutputRT]

        // Interface
        Overlay = 90000,            // GeometryRender, StringRender, TextureRender [OutputRT]
    }
}
