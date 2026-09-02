namespace OCMS.Shared.Helpers
{
    public static class TrackIdGenerator
    {
        private static readonly Random _random = new();

        public static string Generate()
            => "C-" + _random.Next(1000, 9999);
    }
}
