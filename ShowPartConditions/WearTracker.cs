using UnityEngine;

namespace ShowPartConditions
{
    /// <summary>
    /// This is a data-only component used on parts to track that their wear should be included in their name.
    /// The main mod class (<see cref="ShowPartConditions"/>) uses the data stored here.
    /// </summary>
    internal class WearTracker : MonoBehaviour
    {
        /// <summary>
        /// The human-readable name for the part this component is attached to.
        /// </summary>
        public string InitialName;

        /// <summary>
        /// A key used to fetch this part's wear from the game's FSMs. Drawn from <see cref="ShowPartConditions._partNames"/>.
        /// </summary>
        public string WearKey;
    }
}
