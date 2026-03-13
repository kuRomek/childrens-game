public static class Utils
{
    public static class PhysicsLayers
    {
        public const uint Ground = 1 << 0;
        public const uint Player = 1 << 1;
        public const uint Enemy = 1 << 2;
    }

    public static class RenderLayers
    {
        public const uint Default = 1 << 0;
        public const uint Outline1 = 1 << 8;
        public const uint Outline2 = 1 << 9;
        public const uint Outline3 = 1 << 10;
        public const uint Outline4 = 1 << 11;
    }
}
