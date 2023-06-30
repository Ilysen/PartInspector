using HutongGames.PlayMaker;
using UnityEngine;

namespace PartInspector.Wear_Trackers
{
	/// <summary>
	/// Tracks the amount of fluid in fluid containers (brake fluid, motor oil, and two-stroke fuel).
	/// Jerry cans are excluded because they already have a way to check their fullness.
	/// </summary>
	internal class FluidTracker : BaseWearTracker
	{
		/// <summary>
		/// The FSM that keeps track of this spark plug's wear.
		/// This is protected and not private because alternator belt trackers inherit logic - see <see cref="AlternatorBeltTracker"/> for more info.
		/// </summary>
		protected FsmVariables _fluidFsm;

		/// <summary>
		/// The max fluid that this container can hold. Assigned in PartInspector.cs during initialization and used to calculate fractions (i.e. "half full") in <see cref="BuildDisplayText"/>.
		/// </summary>
		private float _maxFluid = 1f;

		/// <inheritdoc/>
		internal override void Initialize(string initName, params object[] extraArgs)
		{
			base.Initialize(initName);
			_fluidFsm = (FsmVariables)extraArgs[0];
			_maxFluid = (float)extraArgs[1];
		}

		/// <inheritdoc/>
		internal override float GetWearPercentage() => (GetFluidLevel() / _maxFluid) * 100;

		private float GetFluidLevel() => _fluidFsm.GetFsmFloat("Fluid").Value;

		/// <inheritdoc/>
		internal override void BuildDisplayText()
		{
			string newText;
			float fluidLevel = GetWearPercentage();
			if (fluidLevel <= 0)
				return;
			switch (PartInspector.DisplayPrecision.GetSelectedItemIndex())
			{
				case 1: // General description; we also lump in "intact or broken" here
				case 2:
					if (fluidLevel >= 100)
						newText = "full";
					else if (fluidLevel >= 90)
						newText = "nearly full";
					else if (fluidLevel >= 75)
						newText = "three-quarters full";
					else if (fluidLevel >= 51)
						newText = "over half full";
					else if (fluidLevel < 51 && fluidLevel > 49)
						newText = "exactly half full, nice!";
					else if (fluidLevel >= 25)
						newText = "under half full";
					else if (fluidLevel >= 10)
						newText = "one quarter full";
					else
						newText = "nearly empty";
					break;
				default: // Exact percentage
					float ml = Mathf.RoundToInt(GetFluidLevel() * 1000);
					if (ml >= 1000) // 1 liter or above - truncate value to read something like "1.2 L"
						newText = $"{System.Math.Round(GetFluidLevel(), 2)} L";
					else // Below 1 liter - display as exact mL value, like "372 mL"
						newText = $"{ml} mL";
					break;
			}
			DisplayText = $"{InitialName} - {newText}";
		}
	}
}
