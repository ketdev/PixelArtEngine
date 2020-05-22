using System;

namespace Sample {
    static class Program {
        [STAThread]
        static void Main() {
            if (Environment.OSVersion.Version.Major >= 6)
                SetProcessDPIAware();
            using (var engine = new Engine.Engine())
                engine.Run();
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();
    }
}
