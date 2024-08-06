using System.Linq.Expressions;

namespace UDT.Settings
{
    public static class UDT_Constants
    {
        public static float DEFAULT_SPEED { get; } = 5.0f;
        public static float DEFAULT_TURN_SPEED { get; } = 360.0f;


        public static string HORIZONTAL_AXIS { get; } = "Horizontal";
        public static string VERTICAL_AXIS { get; } = "Vertical";

        public static float DEFAULT_X_POSITION { get; } = 0.0f;
        public static float DEFAULT_Y_POSITION { get; } = 0.0f;
        public static float DEFAULT_Z_POSITION { get; } = 0.0f;

        public static float DEFAULT_X_ROTATION { get; } = 0.0f;
        public static float DEFAULT_Y_ROTATION { get; } = 0.0f;
        public static float DEFAULT_Z_ROTATION { get; } = 0.0f;

        public static float Y_ROTATION_SKEW_ANGLE { get; } = 45f;
    }
}