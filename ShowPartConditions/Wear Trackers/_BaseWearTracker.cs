using UnityEngine;

namespace PartInspector
{
    /// <summary>
    /// Base wear tracker class with shared logic.
    /// </summary>
    internal abstract class BaseWearTracker : MonoBehaviour
    {
        /// <summary>
        /// The human-readable name for the part this component is attached to.
        /// </summary>
        public string InitialName;

        /// <summary>
        /// The text displayed on-screen when the player hovers over an object with this component.
        /// </summary>
        public string DisplayText;

        /// <summary>
        /// Initializes this wear tracker with the provided arguments.
        /// A name is required, but after that, any number of arguments can be passed. Subtypes can use this for special logic.
        /// </summary>
        internal virtual void Initialize(string initName, params object[] extraArgs)
        {
            InitialName = initName.Replace("(Clone)", "");
        }

        /// <summary>
        /// Updates the <see cref="DisplayText"/> of this wear tracker. Subtypes must each override this function.
        /// </summary>
        internal abstract void BuildDisplayText();
    }
}
