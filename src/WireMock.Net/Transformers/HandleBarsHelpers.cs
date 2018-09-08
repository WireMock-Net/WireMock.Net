namespace WireMock.Transformers
{
    internal static class HandlebarsHelpers
    {
        public static void Register()
        {
            HandleBarsRegex.Register();

            HandleBarsJsonPath.Register();

            HandleBarsLinq.Register();
        }
    }
}