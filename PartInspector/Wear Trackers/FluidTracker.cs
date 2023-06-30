using HutongGames.PlayMaker;
using UnityEngine;

namespace PartInspector.Wear_Trackers
{
	internal class FluidTracker : BaseWearTracker
	{
		/// <summary>
		/// The FSM that keeps track of this spark plug's wear.
		/// This is protected and not private because alternator belt trackers inherit logic - see <see cref="AlternatorBeltTracker"/> for more info.
		/// </summary>
		protected FsmVariables _fluidFsm;

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
						newText = "a bit over half full";
					else if (fluidLevel < 51 && fluidLevel > 49)
						newText = "exactly half full, nice!";
					else if (fluidLevel >= 25)
						newText = "a bit under half full";
					else if (fluidLevel >= 10)
						newText = "one quarter full";
					else
						newText = "nearly empty";
					break;
				default: // Exact percentage
					newText = Mathf.RoundToInt(GetFluidLevel()).ToString();
					break;
			}
			DisplayText = $"{InitialName} - {newText} ml";
		}
	}
}
