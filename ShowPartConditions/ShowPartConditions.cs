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
        public override string Description => ".";

        SettingsDropDownList _displayPreference;

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
        /// A cached reference to the white text that pops up on the middle of the screen.
        /// </summary>
        private FsmString _guiInteraction;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.Update, Mod_OnUpdate);
        }

        public override void ModSettings()
        {
            _displayPreference = Settings.AddDropDownList(this, "displayPreference", "Choose how part conditions should be displayed.",
                new string[] { "Part name", "Interaction text" }, 0, RefreshDisplayGUI);
        }

        private void Mod_OnLoad()
        {
            _plyCamera = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
            _satsumaVars = PlayMakerExtensions.GetPlayMaker(GameObject.Find("SATSUMA(557kg, 248)").transform.Find("CarSimulation/MechanicalWear").gameObject, "Data").FsmVariables;
            RefreshDisplayGUI();
        }

        private void RefreshDisplayGUI() => _guiInteraction = PlayMakerGlobals.Instance.Variables.FindFsmString(_displayPreference.GetSelectedItemIndex() == 0 ? "PickedPart" : "GUIinteraction");

        private void Mod_OnUpdate()
        {
            Ray ray = _plyCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f))
            {
                Collider co = hit.collider;
                if (!_partNames.Keys.Contains(co.name))
                    return;
                GameObject go = co.gameObject;
                if (_wearTrackers.Keys.Contains(go))
                {
                    WearTracker wt = _wearTrackers[go];
                    float partWear = _satsumaVars.GetFsmFloat(wt.WearKey).Value;
                    _guiInteraction.Value = string.Format("{0} - {1}%", wt.InitialName, Mathf.RoundToInt(partWear));
                }
                else
                {
                    ModConsole.Print(string.Format("Detected wearable part object name {0}. Adding wear tracker.", go.name));
                    CreateTrackerForPart(go);
                }
            }
        }

        private void CreateTrackerForPart(GameObject go)
        {
            WearTracker wt = go.AddComponent<WearTracker>();
            wt.InitialName = go.name.Replace("(Clone)", "");
            wt.WearKey = "Wear" + _partNames[go.name];
            _wearTrackers.Add(go, wt);
            ModConsole.Print(string.Format("WearTracker added gameobject {0}", go.name));
        }

        /// <summary>
        /// A dictionary containing a bit of each part's wear key in the FSM variables.
        /// On load, game object names are checked for presence as a key;
        /// if present, a <see cref="WearTracker"/> component is created on them using the name's associated value.
        /// </summary>
        private Dictionary<string, string> _partNames = new Dictionary<string, string> {
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
