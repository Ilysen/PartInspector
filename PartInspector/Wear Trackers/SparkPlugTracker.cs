using HutongGames.PlayMaker;
using UnityEngine;

namespace PartInspector
{
    /// <summary>
    /// Tracks how worn-down a spark plug is.
    /// </summary>
    internal class SparkPlugTracker : BaseWearTracker
    {
        /// <summary>
        /// The FSM that keeps track of this spark plug's wear.
        /// This is protected and not private because alternator belt trackers inherit logic - see <see cref="AlternatorBeltTracker"/> for more info.
        /// </summary>
        protected FsmVariables _wearFsm;

        /// <inheritdoc/>
        internal override void Initialize(string initName, params object[] extraArgs)
        {
            base.Initialize(initName);
            _wearFsm = (FsmVariables)extraArgs[0];
        }

        // unlike nearly every other part, MSC tracks integrity on spark plugs as a positive value and not a negative one;
        // i.e. it's closer to "remaining health" than it is to "total damage".
        // why it does this in differently from everything other part, I will never know 
        /// <inheritdoc/>
        internal override float GetWearPercentage() => 100 - _wearFsm.GetFsmFloat("Wear").Value;

        /// <inheritdoc/>
        internal override void BuildDisplayText()
        {
            string newText;
            float effectiveWear = GetWearPercentage();
            switch (PartInspector._displayPrecision.GetSelectedItemIndex())
            {
                case 1: // General description
                    if (effectiveWear >= 85)
                        newText = "Failing";
                    else if (effectiveWear >= 35)
                        newText = "Worn";
                    else
                        newText = "New";
                    break;
                case 2: // Broken/not broken
                    newText = effectiveWear >= 85 ? "Failing" : "Intact";
                    break;
                default: // Exact percentage
                    newText = string.Format("{0} worn down", Mathf.RoundToInt(effectiveWear) + "%");
                    break;
            }
            DisplayText = string.Format("{0} - {1}", InitialName, newText);
        }
    }
}
