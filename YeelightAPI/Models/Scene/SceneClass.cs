namespace YeelightAPI.Models.Scene
{
    /// <summary>
    /// Available classes for a Scene
    /// </summary>
    public enum SceneClass
    {
        /// <summary>
        /// RGB Color
        /// </summary>
        color,

        /// <summary>
        /// HSV Color
        /// </summary>
        hsv,

        /// <summary>
        /// Color temperature
        /// </summary>
        ct,

        /// <summary>
        /// Color Flow
        /// </summary>
        cf,

        /// <summary>
        /// automatic delay off
        /// </summary>
        auto_delay_off
    }
}