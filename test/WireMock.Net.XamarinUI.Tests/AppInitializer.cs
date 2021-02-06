using System;
using System.IO;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace WireMock.Net.XamarinUI.Tests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                var androidAppConfigurator = ConfigureApp.Android.InstalledApp("test");

                return androidAppConfigurator.StartApp();
            }

            return ConfigureApp.iOS.InstalledApp("test").StartApp();
        }
    }
}