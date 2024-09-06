using UnityEngine;
using BepInEx;
using HarmonyLib;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using GunsOPlenty.Data;
using PluginConfig.API;
using UnityEngine.SceneManagement;
using PluginConfig.API.Fields;
using GunsOPlenty.Trials;
using System.Linq.Expressions;
using Debug = UnityEngine.Debug;
using System;
using Object = UnityEngine.Object;
using System.IO;
using UnityEngine.UI;

// To anyone looking at this code, #42630668, #c2b14422
// funny colors
// Feedback is greatly appreciated

namespace GunsOPlenty
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    
    public class Plugin : BaseUnityPlugin
    {
        Harmony harmony = new Harmony(PluginInfo.GUID);
        bool firstTime = true;
        string scene = "";
        public void Awake()
        {
            
        }

        public void Start()
        {
            scene = SceneHelper.CurrentScene;
            Debug.Log("Current Scene: " + scene);
            //SaveManager.CreateContainer(Path.Combine(GOPUtils.ModSaveDir(), "test.goop"), "grgr");
            //SaveManager.SerializeValue(Path.Combine(GOPUtils.ModDataDir(), "test.goop"), "thingy1", "val_val", "audio jungle");
            //SaveManager.SerializeValue(Path.Combine(GOPUtils.ModDataDir(), "test.goop"), "thingy1", "val_val", "audio jungle");
            /*int thing;
            if (SaveManager.TryDeserializeValue(Path.Combine(GOPUtils.ModDataDir(), "test.goop"), "thingy1", "val_val", out thing))
            {
                Debug.Log("LOOK AT MY SON!!: " + thing);
            }
            else
            {
                Debug.Log("I have disowned my son lol");
            }*/

            SceneManager.activeSceneChanged += Startup;
            SceneManager.activeSceneChanged += OnMainMenu;
            //UnlockManager.Setup();
        }

        public void Update()
        {
            if (SceneHelper.CurrentScene != scene)
            {
                scene = SceneHelper.CurrentScene;
                Debug.Log("Current Scene: "+scene);
            }

        }
        void Startup(Scene before, Scene after)
        {
            if (SceneHelper.CurrentScene != "Bootstrap" && firstTime)//
            {
                ConfigManager.Setup();
                AssetHandler.LoadBundle();
                harmony.PatchAll(typeof(UnlockManager));
                if (AssetHandler.bundleLoaded)
                {
                    AssetHandler.CreateCustomPrefabs();
                    PersistentCanvas.SetupCanvas();
                    WeaponHandler.SetupWeapons();
                    harmony.PatchAll(typeof(WeaponPatch));
                    harmony.PatchAll(typeof(StylePatch));
                    CommandManager.RegisterAll();
                    TrialManager.SetupTrials();
                    UnlockManager.Setup();
                }
                firstTime = false;
            }
        }

        void OnMainMenu(Scene before, Scene after)
        {
            if (SceneHelper.CurrentScene == "Main Menu")//
            {
                TrialManager.SetupUI();
            }
        }

        public static bool IsInLevel()
        {
            return 
                SceneHelper.CurrentScene != "Main Menu" &&
                SceneHelper.CurrentScene != "Bootstrap" &&
                SceneHelper.CurrentScene != "Intro";
        }
    }
}
