using HutongGames.PlayMaker;
using UnityEngine;

namespace ShowPartConditions
{
    /// <summary>
    /// Tracks how dirty an oil filter is.
    /// </summary>
    internal class OilFilterTracker : BaseWearTracker
    {
        /// <summary>
        /// The FSM that keeps track of this filter's dirtiness.
        /// </summary>
        private FsmVariables _dirtFsm;

        /// <inheritdoc/>
        internal override void Initialize(string initName, params object[] extraArgs)
        {
            base.Initialize(initName);
            _dirtFsm = (FsmVariables)extraArgs[0];
        }

        /// <inheritdoc/>
        internal override void BuildDisplayText()
        {
            string newText;
            float effectiveFilth = _dirtFsm.GetFsmFloat("Dirt").Value;
            switch (ShowPartConditions._displayPrecision.GetSelectedItemIndex())
            {
                case 1: // "Intact" and "broken" doesn't really apply to oil filters, so we always generalize if not using the percentage option
                case 2:
                    if (effectiveFilth >= 80)
                        newText = "Filthy";
                    else if (effectiveFilth >= 60)
                        newText = "Dirty";
                    else if (effectiveFilth >= 40)
                        newText = "Grimy";
                    else if (effectiveFilth >= 20)
                        newText = "Clean";
                    else
                        newText = "Brand new";
                    break;
                default: // Exact percentage
                    newText = string.Format("{0} dirty", Mathf.RoundToInt(effectiveFilth) + "%");
                    break;
            }
            DisplayText = string.Format("{0} - {1}", InitialName, newText);
        }
    }
}
