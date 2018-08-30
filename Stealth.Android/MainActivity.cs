using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Android.Views;
using Microsoft.Xna.Framework;
using Stealth.Kernel;

namespace Stealth {
    [Activity(Label = Settings.Title,
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@style/Theme.Splash",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.UserLandscape,
        ConfigurationChanges = 
          ConfigChanges.Orientation 
        | ConfigChanges.Keyboard 
        | ConfigChanges.KeyboardHidden 
        | ConfigChanges.ScreenSize)]
    public class MainActivity : AndroidGameActivity {
        protected override void OnCreate(Bundle bundle) {
            base.OnCreate(bundle);

            // Get backbuffer size
            DisplayMetrics dm = new DisplayMetrics();
            WindowManager.DefaultDisplay.GetMetrics(dm);

            var g = new GameEngine(dm.WidthPixels, dm.HeightPixels);
            SetContentView((View)g.Services.GetService(typeof(View)));
            g.Run();
        }
        public override void OnWindowFocusChanged(bool hasFocus) {
            base.OnWindowFocusChanged(hasFocus);
            if (hasFocus) {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)(
                    SystemUiFlags.LayoutStable |
                    SystemUiFlags.LayoutHideNavigation |
                    SystemUiFlags.LayoutFullscreen |
                    SystemUiFlags.HideNavigation |
                    SystemUiFlags.Fullscreen |
                    SystemUiFlags.ImmersiveSticky
                );
            }
        }
    }
}

