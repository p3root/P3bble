namespace P3bble.Types
{
    /// <summary>
    /// The music control action
    /// </summary>
    public enum MusicControlAction : byte
    {
        /// <summary>
        /// Unknown state
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Play or Pause
        /// </summary>
        PlayPause = 1,

        /// <summary>
        /// Skip next
        /// </summary>
        Next = 4,

        /// <summary>
        /// Skip previous
        /// </summary>
        Previous = 5
    }
}
