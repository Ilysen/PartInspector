namespace PartInspector
{
    /// <summary>
    /// Tracks how worn-down a spark plug is.
    /// Conveniently for us, alternator belts and spark plugs currently use the exact same value names to track wear,
    /// so right now we just inherit the logic to avoid duplicated code.
    /// </summary>
    internal class AlternatorBeltTracker : SparkPlugTracker { }
}
