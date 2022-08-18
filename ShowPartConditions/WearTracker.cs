using UnityEngine;

namespace ShowPartConditions
{
    /// <summary>
    /// This is a data-only component used on parts to track that their wear should be included in their name.
    /// </summary>
    internal class WearTracker : MonoBehaviour
    {
        public string InitialName;
        public string WearKey;
    }
}
