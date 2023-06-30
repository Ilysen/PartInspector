﻿using UnityEngine;

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
		/// Returns the wear percentage for this part.
		/// MSC doesn't track this in a standardized way, so each different type of tracker needs its own logic to get the appropriate value.
		/// This should be overridden on all subtypes, but is virtual and not abstract because some types don't need to worry about it.
		/// </summary>
		internal virtual float GetWearPercentage() => 0;

		/// <summary>
		/// Updates the <see cref="DisplayText"/> of this wear tracker. Subtypes must each override this function.
		/// </summary>
		internal abstract void BuildDisplayText();
	}
}
