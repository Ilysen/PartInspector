namespace PartInspector
{
    /// <summary>
    /// Tracks how worn-down a spark plug is.
    /// Conveniently for us, alternator belts and spark plugs currently use the exact same value names to track wear,
    /// so right now we just inherit the logic to avoid duplicated code. unfortunately, the world is cruel and it tracks wear differently,
    /// so we do need some minor overriding to get it to play nicely.
    /// </summary>
    internal class AlternatorBeltTracker : SparkPlugTracker
    {
        /// <inheritdoc/>
        internal override float GetWearPercentage() => _wearFsm.GetFsmFloat("Wear").Value;
    }
}
