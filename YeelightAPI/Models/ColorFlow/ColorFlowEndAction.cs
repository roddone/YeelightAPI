namespace YeelightAPI.Models.ColorFlow
{
    /// <summary>
    /// Action taken after the flow is stopped
    /// </summary>
    public enum ColorFlowEndAction
    {
        /// <summary>
        /// Restore to the previous state
        /// </summary>
        Restore = 0,

        /// <summary>
        /// Keep the last state of the flow
        /// </summary>
        Keep = 1,

        /// <summary>
        /// Turn the device off
        /// </summary>
        TurnOff = 2
    }
}