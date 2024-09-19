namespace UDT.Settings
{
    public static class UDT_Constants
    {
#region Player values
#region Player speed values
        public static float DEFAULT_SPEED { get; } = 0.0f;
        public static float MAX_SPEED { get; } = 5.0f;
        public static float MAX_CROUCHING_SPEED { get; } = 1.0f;
        public static float ACCELERATION { get; } = 1.0f;
        public static float DEFAULT_TURN_SPEED { get; } = 360.0f;
#endregion  

#region Player movement values
        public static string HORIZONTAL_AXIS { get; } = "Horizontal";
        public static string VERTICAL_AXIS { get; } = "Vertical";
#endregion
        
#region Player IK values
        public static string IK_WEIGHT_LEFT { get; } = "IKLeftFootWeight";
        public static string IK_WEIGHT_RIGHT { get; } = "IKRightFootWeight";
        public static string IK_WEIGHT_LEFT_HAND { get; } = "IKLeftHandWeight";
        public static string IK_WEIGHT_RIGHT_HAND { get; } = "IKRightHandWeight";

        public static float IK_DISTANCE { get; } = 1.0f;
#endregion

#region Player obstacle calculation values
        public static float OBSTACLE_RAY_DISTANCE_DEFAULT { get; } = 5.0f;
        public static float OBSTACLE_RAY_DISTANCE_MIN { get; } = 1.0f;
        public static float PLAYER_AI_DETECTION_RANGE { get; } = 0f;
#endregion

#region Player health and damage values
        public static int PLAYER_MAX_HEALTH { get; } = 300;
        public static int PLAYER_DEATH_HEALTH { get; } = 0;
        public static int DEFAULT_DAMAGE { get; } = 10;
#endregion
#endregion

#region Camera skew values
        public static float Y_ROTATION_SKEW_ANGLE { get; } = -45.0f;
#endregion

#region AI Values
#region AI Area values
        public static float POSITION_GENERATION_INTERVAL { get; } = 1f;
        public static float AREA_ALPHA_DEFAULT { get; } = 0.5f;
        public static float GENERATED_Y_AREA_POSITION { get; } = 0.0f;
        public static float POINT_RESPAWN_TIMER { get; } = 0.0f;
        public static float LIMIT_DIVIDER_2D { get; } = 2.0f;
#endregion

#region Fish AI values
        public static float FISH_SPEED { get; } = 3.0f;
        public static float SCARED_SPEED_MULTIPLIER { get; } = 10.0f;
        public static float CHANGE_DIRECTION_INTERVAL { get; } = 2.0f;
        public static float DETECTION_RADIUS { get; } = 5.0f;
        public static int FEAR_RANGE_POWER { get; } = 2;
#endregion

#region Global AI values
        public static float AI_DEFAULT_MAX_SPEED { get; } = 3.5f;
        public static float AI_DEFAULT_MIN_SPEED { get; } = 0.0f;
        public static float AI_DEFAULT_ACCELERATION { get; } = 8.0f;
        public static float AI_DEFAULT_DECELERATION { get; } = 8.0f;
        public static float AI_DEFAULT_SLOWING_DISTANCE { get; } = 5.0f;
        public static float AI_DEFAULT_STOPPING_DISTANCE { get; } = 0.5f;
        public static float AI_DETECTION_RANGE { get; } = 10.0f;
#endregion

#region Villager AI values
    public static float VILLAGER_SPEED { get; } = 2.0f;
    public static float VILLAGER_OUT_OF_SCREEN_LOCATION_MULTIPLIER { get; } = 50.0f;

#region Villager AI run away state settings
    public static float VILLAGER_RUN_AWAY_SPEED { get; } = 10.0f;
    public static float VILLAGER_RUN_AWAY_ACCELERATION { get; } = 1000.0f;

#endregion

#endregion

#region Guard AI values
#region Guard AI settings
        public static float GUARD_DETECTION_RADIUS { get; } = 10.0f;
        public static float GUARD_ATTACK_RADIUS { get; } = 2.0f;
        public static float GUARD_ATTACK_COOLDOWN { get; } = 2.0f;
        public static int GUARD_HEALTH { get; } = 100;
        public static int GUARD_RANGE_POWER { get; } = 2;
#endregion
#region Guard AI Damage and health values
        public static int GUARD_DEATH_HEALTH { get; } = 0;
        public static int GUARD_DAMAGE { get; } = 10;
#endregion

#endregion
#endregion

#region Time values
        public static float TIME_SCALE_DEFAULT { get; } = 1.0f;
        public static float TIME_SCALE_PAUSE { get; } = 0.0f;
#endregion
    }
}