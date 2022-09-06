using HutongGames.PlayMaker;

namespace ShowPartConditions
{
    /// <summary>
    /// Only tracks whether or not a part is intact or damaged, nothing else. Used for blocks and oilpans.
    /// </summary>
    internal class SimpleWearTracker : BaseWearTracker
    {   
        /// <summary>
        /// Used to track if this part is broken or not.
        /// </summary>
        private FsmVariables _dbInfo;

        /// <inheritdoc/>
        internal override void Initialize(string initName, params object[] extraArgs)
        {
            base.Initialize(initName);
            _dbInfo = (FsmVariables)extraArgs[0];
        }

        /// <inheritdoc/>
        internal override void BuildDisplayText()
        {
            DisplayText = string.Format("{0} - {1}", InitialName, _dbInfo.GetFsmBool("Damaged").Value ? "Broken" : "Intact");
        }
    }
}
