using HutongGames.PlayMaker;
using MSCLoader;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PartInspector
{
    public class PartInspector : Mod
    {
        private enum TrackerType
        {
            Standard = 1,
            Simple = 2,
            OilFilter = 3
        }

        // MSCLoader stuff
        public override string ID => "PartInspector";
        public override string Name => "Part Inspector";
        public override string Author => "Ava";
        public override string Version => "1.0";
        public override string Description => "Inspect your parts for integrity, condition, and dirtiness.";

        internal static SettingsDropDownList _displayLocation;
        internal static SettingsDropDownList _displayPrecision;
        internal static SettingsCheckBox _verboseLogging;
        internal static SettingsSlider _textUpdateFrequency;

        /// <summary>
        /// The FSM variables used to track the Satsuma's part wear. We reference this a lot, so we save it early.
        /// </summary>
        internal static FsmVariables _satsumaVars;

        /// <summary>
        /// A list of all FSMs used in the motor database. These are where the game keeps track of if parts are installed, broken, etc - but not wear-and-tear, which exists on independently FSMs on each part.
        /// </summary>
        private List<PlayMakerFSM> _motorDb;

        /// <summary>
        /// Every wear tracker in the game world, associated to its game object.
        /// </summary>
        private Dictionary<GameObject, BaseWearTracker> _wearTrackers;

        /// <summary>
        /// The first person camera used by the player. Names are only updated if we're looking at the respective part.
        /// </summary>
        private Camera _plyCamera;

        /// <summary>
        /// The text GUI used to display the part's condition. Can either be within the part's name or in a separate area.
        /// </summary>
        private FsmString _displayGui;

        /// <summary>
        /// To save performance, and because parts are unlikely to rapidly change condition in a given time period, display names only update every few seconds. This value tracks how often parts update, in seconds.
        /// </summary>
        private float _timeBetweenUpdates = 10f;

        /// <summary>
        /// How many seconds have elapsed since we last updated displays. See <see cref="_timeBetweenUpdates"/> for more info.
        /// </summary>
        private float _updateTimer;

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
            SetupFunction(Setup.Update, Mod_OnUpdate);
        }

        public override void ModSettings()
        {
            _displayLocation = Settings.AddDropDownList(this, "displayLocation", "Display location",
                new string[] { "Part name", "Interaction text" }, 0, RefreshDisplayGUI);
            _displayPrecision = Settings.AddDropDownList(this, "displayPrecision", "Display precision",
                new string[] { "Show exact integrity", "Show general description", "Show broken/not broken" }, 0);
            _textUpdateFrequency = Settings.AddSlider(this, "updateFrequency", "Text update frequency",
                0f, 10f, 10f, RebuildDisplays);
            _verboseLogging = Settings.AddCheckBox(this, "verboseLogging", "Verbose logging", false);
        }

        private void Mod_OnLoad()
        {
            _plyCamera = GameObject.Find("PLAYER/Pivot/AnimPivot/Camera/FPSCamera/FPSCamera").GetComponent<Camera>();
            _satsumaVars = PlayMakerExtensions.GetPlayMaker(GameObject.Find("SATSUMA(557kg, 248)").transform.Find("CarSimulation/MechanicalWear").gameObject, "Data").FsmVariables;
            _motorDb = new List<PlayMakerFSM>();
            _wearTrackers = new Dictionary<GameObject, BaseWearTracker>();
            foreach (PlayMakerFSM fsm in GameObject.Find("Database/DatabaseMotor").GetComponentsInChildren<PlayMakerFSM>())
            {
                if (_verboseLogging.GetValue())
                    ModConsole.Print(string.Format("Adding fsm to database: {0}", fsm.gameObject.name));
                _motorDb.Add(fsm);
            }
            RefreshDisplayGUI();
            RebuildDisplays();
            ModConsole.Print(string.Format("{0} version {1} has been initialized!", Name, Version));
        }

        /// <summary>
        /// Updates the value of <see cref="_displayGui"/> based on user settings.
        /// </summary>
        private void RefreshDisplayGUI() => _displayGui = PlayMakerGlobals.Instance.Variables.FindFsmString(_displayLocation.GetSelectedItemIndex() == 0 ? "PickedPart" : "GUIinteraction");

        /// <summary>
        /// Simple wrapper to adjust relevant values when update frequency settings are changed.
        /// </summary>
        private void RebuildDisplays()
        {
            _updateTimer = 0f;
            _timeBetweenUpdates = _textUpdateFrequency.GetValue();
        }

        private void Mod_OnUpdate()
        {
            UpdateDisplays();
            UpdateInspection();
        }

        /// <summary>
        /// Manipulate the value of <see cref="_updateTimer"/> and update display texts as needed.
        /// </summary>
        private void UpdateDisplays()
        {
            _updateTimer += Time.deltaTime;
            if (_updateTimer >= _timeBetweenUpdates)
            {
                _updateTimer = 0f;
                foreach (BaseWearTracker wt in _wearTrackers.Values)
                    wt.BuildDisplayText();
            }
        }

        /// <summary>
        /// Raycasts to find if the player is looking at a part. If so, displays the text from that part's wear tracker.
        /// </summary>
        private void UpdateInspection()
        {
            // Raycasting every frame is extremely cheap compared to other logic, so we check to make sure we're aiming at something first
            Ray ray = _plyCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f))
            {
                // If we're aiming at a part designated in _partNames, continue
                GameObject go = hit.collider.gameObject;
                if (!_partNames.Keys.Contains(go.name))
                {
                    // We check for a parent object because some parts (like the water pump) have children with colliders
                    if (!_partNames.Keys.Contains(go.transform.parent.gameObject.name))
                        return;
                    else
                        go = go.transform.parent.gameObject;
                }
                // Does the object already have a wear tracker? Display the part's integrity data
                if (_wearTrackers.Keys.Contains(go))
                {
                    BaseWearTracker wt = _wearTrackers[go];
                    _displayGui.Value = wt.DisplayText;
                }
                // Otherwise, add a wear tracker component. We'll use the data next frame
                // We avoid doing this on load so that this way it's compatible with objects that can show up during gameplay
                else
                {
                    if (_verboseLogging.GetValue())
                        ModConsole.Print(string.Format("Detected valid object: {0}. Adding wear tracker.", go.name));
                    TrackerType tt = TrackerType.Standard;
                    if (_partNames[go.name] is TrackerType pt)
                        tt = pt;
                    CreateTrackerForPart(go, tt);
                }
            }
        }

        /// <summary>
        /// Creates a wear tracker component for the provided <see cref="GameObject"/>.
        /// Info will be taken from <see cref="_partNames"/> to create the component; invalid objects will thus cause this function to throw an error.
        /// </summary>
        /// <param name="go">The <see cref="GameObject"/> that will begin being tracked.</param>
        private void CreateTrackerForPart(GameObject go, TrackerType tt = TrackerType.Standard)
        {
            FsmVariables dbInfo = null;
            foreach (PlayMakerFSM fsm in _motorDb)
            {
                FsmVariables vars = fsm.FsmVariables;
                if (vars.GetFsmString("UniqueTag").Value == go.name)
                {
                    dbInfo = vars;
                    break;
                }
            }
            BaseWearTracker bwt = null;
            switch (tt)
            {
                case TrackerType.Standard:
                    StandardWearTracker swt = go.AddComponent<StandardWearTracker>();
                    swt.Initialize(go.name, "Wear" + _partNames[go.name], _satsumaVars, dbInfo);
                    bwt = swt;
                    break;
                case TrackerType.Simple:
                    SimpleWearTracker smt = go.AddComponent<SimpleWearTracker>();
                    smt.Initialize(go.name, dbInfo);
                    bwt = smt;
                    break;
                case TrackerType.OilFilter:
                    OilFilterTracker of = go.AddComponent<OilFilterTracker>();
                    of.Initialize(go.name, PlayMakerExtensions.GetPlayMaker(go, "Use").FsmVariables);
                    bwt = of;
                    break;
            }
            if (bwt != null)
            {
                bwt.BuildDisplayText();
                _wearTrackers.Add(go, bwt);
            }
            if (_verboseLogging.GetValue())
                ModConsole.Print(string.Format("A wear tracker component was added to a new gameobject: {0}.", go.name));
        }

        /// <summary>
        /// Game objects with names in the keys of this dict will gain a wear tracker component when inspected, if they don't have one already, with some of that component's info being taken from the associated value of that key.
        /// <br/><br/>
        /// Names with an associated string will be given a <see cref="StandardWearTracker"/> using that string as the wear key; names with an associated <see cref="TrackerType"/> will instead use that type when creating the tracker. This is very spaghetti, but then again, so is this game.
        /// </summary>
        /// modding is a pathway to abilities some consider to be unnatural
        private readonly Dictionary<string, object> _partNames = new Dictionary<string, object>
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
            { "water pump(Clone)", "Waterpump" },
            { "block(Clone)", TrackerType.Simple },
            { "oilpan(Clone)", TrackerType.Simple },
            { "oil filter(Clone)", TrackerType.OilFilter }
        };
    }
}
