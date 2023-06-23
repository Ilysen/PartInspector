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
        /// The FSM that keeps track of this filter's dirtiness.
        /// </summary>
        private FsmVariables _wearFsm;

        /// <inheritdoc/>
        internal override void Initialize(string initName, params object[] extraArgs)
        {
            base.Initialize(initName);
            _wearFsm = (FsmVariables)extraArgs[0];
        }

        /// <inheritdoc/>
        internal override void BuildDisplayText()
        {
            string newText;
            float effectiveWear = _wearFsm.GetFsmFloat("Wear").Value;
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
