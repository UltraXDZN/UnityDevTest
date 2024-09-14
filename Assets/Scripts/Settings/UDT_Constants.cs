using System.Linq.Expressions;

namespace UDT.Settings
{
    public static class UDT_Constants
    {
        public static float DEFAULT_SPEED { get; } = 0.0f;
        public static float MAX_SPEED { get; } = 5.0f;
        public static float MAX_CROUCHING_SPEED { get; } = 1.0f;
        public static float ACCELERATION { get; } = 1.0f;
        public static float DEFAULT_TURN_SPEED { get; } = 360.0f;


        public static string HORIZONTAL_AXIS { get; } = "Horizontal";
        public static string VERTICAL_AXIS { get; } = "Vertical";

        public static float DEFAULT_X_POSITION { get; } = 0.0f;
        public static float DEFAULT_Y_POSITION { get; } = 0.0f;
        public static float DEFAULT_Z_POSITION { get; } = 0.0f;

        public static float DEFAULT_X_ROTATION { get; } = 0.0f;
        public static float DEFAULT_Y_ROTATION { get; } = 0.0f;
        public static float DEFAULT_Z_ROTATION { get; } = 0.0f;

        public static float Y_ROTATION_SKEW_ANGLE { get; } = -45.0f;

        public static string IK_WEIGHT_LEFT { get; } = "IKLeftFootWeight";
        public static string IK_WEIGHT_RIGHT { get; } = "IKRightFootWeight";
        public static string IK_WEIGHT_LEFT_HAND { get; } = "IKLeftHandWeight";
        public static string IK_WEIGHT_RIGHT_HAND { get; } = "IKRightHandWeight";

        public static float IK_DISTANCE { get; } = 1.0f;

        public static float OBSTACLE_RAY_DISTANCE_DEFAULT { get; } = 5.0f;
        public static float OBSTACLE_RAY_DISTANCE_MIN { get; } = 1.0f;

        public static float POSITION_GENERATION_INTERVAL { get; } = 1f;
        public static float AREA_ALPHA_DEFAULT { get; } = 0.5f;
        public static float GENERATED_Y_AREA_POSITION { get; } = 0.0f;
        public static float POINT_RESPAWN_TIMER { get; } = 0.0f;
        public static float LIMIT_DIVIDER_2D { get; } = 2.0f;


    }
}