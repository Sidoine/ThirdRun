using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ThirdRun.Android
{
    [Activity(
        Label = "@string/app_name",
        MainLauncher = true,
        Icon = "@drawable/icon",
        AlwaysRetainTaskState = true,
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.FullUser,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.ScreenSize
    )]
    public class Activity1 : Activity
    {
        private MonogameRPG.Game1? _game;

        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);

            // Create and run the game
            _game = new MonogameRPG.Game1();
            _game.Run();
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Game lifecycle handled by MonoGame
        }

        protected override void OnPause()
        {
            base.OnPause();
            // Game lifecycle handled by MonoGame
        }

        protected override void OnDestroy()
        {
            _game?.Exit();
            base.OnDestroy();
        }
    }
}