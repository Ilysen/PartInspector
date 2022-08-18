using HutongGames.PlayMaker;
using MSCLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShowPartConditions
{
    public class ShowPartConditions : Mod
    {
        // MSCLoader stuff
        public override string ID => "ShowPartConditions";
        public override string Name => "Show Part Conditions";
        public override string Author => "Ava";
        public override string Version => "1.0";
        public override string Description => "Displays the integrity level of parts that you look at.";
        SettingsDropDownList _displayLocation;
        SettingsDropDownList _displayPrecision;
        SettingsCheckBox _verboseLogging;

        /// <summary>
        /// The FSM variables used to track the Satsuma's part wear. We reference this a lot, so we save it early.
        /// </summary>
        private FsmVariables _satsumaVars;

        /// <summary>
        /// Every <see cref="WearTracker"/> in the game world, associated to its game object.
        /// </summary>
        private Dictionary<GameObject, WearTracker> _wearTrackers = new Dictionary<GameObject, WearTracker>();

        /// <summary>
        /// The first person camera used by the player. Names are only updated if we're looking at the respective part.
        /// </summary>
        private Camera _plyCamera;

        /// <summary>
        /// The text GUI used to display the part's condition. Can either be within the part's name or in a separate area.
        /// </summary>
        private FsmString _displayGui;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.Update, Mod_OnUpdate);
        }

        public override void ModSettings()
        {
            _verboseLogging = Settings.AddCheckBox(this, "verboseLogging", "Verbose logging", false);
            _displayLocation = Settings.AddDropDownList(this, "displayLocation", "Display location",
                new string[] { "Part name", "Interaction text" }, 0, RefreshDisplayGUI);
            _displayPrecision = Settings.AddDropDownList(this, "displayPrecision", "Display precision",
                new string[] { "Show exact integrity", "Show general description", "Show broken/not broken" }, 0);
        }

        private void Mod_OnLoad()
        {
            _plyCamera = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
            _satsumaVars = PlayMakerExtensions.GetPlayMaker(GameObject.Find("SATSUMA(557kg, 248)").transform.Find("CarSimulation/MechanicalWear").gameObject, "Data").FsmVariables;
            RefreshDisplayGUI();
            ModConsole.Print(string.Format("{0} version {1} has been initialized!", Name, Version));
        }

        /// <summary>
        /// Updates the value of <see cref="_displayGui"/> based on user settings.
        /// </summary>
        private void RefreshDisplayGUI() => _displayGui = PlayMakerGlobals.Instance.Variables.FindFsmString(_displayLocation.GetSelectedItemIndex() == 0 ? "PickedPart" : "GUIinteraction");

        private void Mod_OnUpdate()
        {
            // Raycasting every frame is extremely cheap compared to other logic, so we check to make sure we're aiming at something first
            Ray ray = _plyCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f))
            {
                // If we're aiming at a part designated in _partNames, continue
                Collider co = hit.collider;
                if (!_partNames.Keys.Contains(co.name))
                    return;
                GameObject go = co.gameObject;
                // Does the object already have a wear tracker? Display the part's integrity data
                if (_wearTrackers.Keys.Contains(go))
                {
                    WearTracker wt = _wearTrackers[go];
                    float partWear = _satsumaVars.GetFsmFloat(wt.WearKey).Value;
                    // "Why not just change the object's name if you're using the part name?"
                    // Good question! I actually tried this, but unfortunately things act weird
                    // The part name will be cut off and display incorrectly;
                    // chances are I missed something obvious or don't know how,
                    // but this works for the time being with only minimal issues
                    _displayGui.Value = string.Format("{0} - {1}", wt.InitialName, GetDamageString(partWear));
                }
                // Otherwise, add a wear tracker component. We'll use the data next frame
                else
                {
                    if (_verboseLogging.GetValue())
                        ModConsole.Print(string.Format("Detected valid object: {0}. Adding wear tracker.", go.name));
                    CreateTrackerForPart(go);
                }
            }
        }

        /// <summary>
        /// Returns a string representative of the provided integrity percentage.
        /// </summary>
        /// <param name="partWear">How intact the provided part is, as a percentage.</param>
        private string GetDamageString(float partWear)
        {
            if (partWear <= 0) // Always display broken parts as just "broken"
                return "Broken";
            switch (_displayPrecision.GetSelectedItemIndex())
            {
                case 1: // General description
                    string descriptor;
                    if (partWear >= 90)
                        descriptor = "mint";
                    else if (partWear >= 80)
                        descriptor = "great";
                    else if (partWear >= 65)
                        descriptor = "good";
                    else if (partWear >= 50)
                        descriptor = "decent";
                    else if (partWear >= 35)
                        descriptor = "shoddy";
                    else if (partWear >= 20)
                        descriptor = "poor";
                    else if (partWear >= 10)
                        descriptor = "bad";
                    else
                        descriptor = "terrible";
                    return string.Format("In {0} condition", descriptor);
                case 2: // Broken/not broken
                    return "Intact";
                default: // Exact percentage
                    return Mathf.RoundToInt(partWear) + "%";
            }
        }

        /// <summary>
        /// Creates a <see cref="WearTracker"/> component for the provided <see cref="GameObject"/>.
        /// Info will be taken from <see cref="_partNames"/> to create the component; invalid objects will thus cause this function to throw an error.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> that will begin being tracked.</param>
        private void CreateTrackerForPart(GameObject go)
        {
            WearTracker wt = go.AddComponent<WearTracker>();
            wt.InitialName = go.name.Replace("(Clone)", "");
            wt.WearKey = "Wear" + _partNames[go.name];
            _wearTrackers.Add(go, wt);
            if (_verboseLogging.GetValue())
                ModConsole.Print(string.Format("A WearTracker component was added to a new gameobject: {0}", go.name));
        }

        /// <summary>
        /// Game objects with names in the keys of this dict will gain a <see cref="WearTracker"/> component when inspected, if they don't have one already, with some of that component's info being taken from the associated value of that key.
        /// </summary>
        /// modding is a pathway to abilities some consider to be unnatural
        private readonly Dictionary<string, string> _partNames = new Dictionary<string, string>
        {
            { "alternator(Clone)", "Alternator" },
            { "clutch disc(Clone)", "Clutch" },
            { "crankshaft(Clone)", "Crankshaft" },
            { "fuel pump(Clone)", "Fuelpump" },
            { "gearbox(Clone)", "Gearbox" },
            { "head gasket(Clone)", "Headgasket" },
            { "piston1(Clone)", "Piston1" },
            { "piston2(Clone)", "Piston2" },
            { "piston3(Clone)", "Piston3" },
            { "piston4(Clone)", "Piston4" },
            { "rocker shaft(Clone)", "Rockershaft" },
            { "starter(Clone)", "Starter" },
            { "water pump(Clone)", "Waterpump" }
        };
    }
}
