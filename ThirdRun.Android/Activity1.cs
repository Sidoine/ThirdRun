using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Android;

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
    public class Activity1 : AndroidGameActivity
    {
        private MonogameRPG.Game1? _game;

        protected override void OnCreate(Bundle? bundle)
        {
            base.OnCreate(bundle);

            _game = new MonogameRPG.Game1();
            SetContentView((View)_game.Services.GetService(typeof(View))!);
            _game.Run();
        }
    }
}