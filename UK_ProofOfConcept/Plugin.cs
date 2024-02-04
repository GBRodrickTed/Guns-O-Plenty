using UnityEngine;
using BepInEx;
using HarmonyLib;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using GunsOPlenty.Data;
using PluginConfig.API;
using UnityEngine.SceneManagement;
using PluginConfig.API.Fields;

// To anyone looking at this code, hopefully it works now
// Inspired by CrashLibs but all the code is my own
// Feedback is greatly appreciated

namespace GunsOPlenty
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    
    public class Plugin : BaseUnityPlugin
    {
        Harmony harmony = new Harmony(PluginInfo.GUID);
        bool firstTime = true;
        public void Awake()
        {
            //StartCoroutine(Setup());
        }

        public void Start()
        {
            ConfigManager.Setup();
            AssetHandler.LoadBundle();
            if (AssetHandler.bundleLoaded)
            {
                AssetHandler.CreateCustomPrefabs();
                WeaponHandler.SetupWeapons();
                harmony.PatchAll(typeof(WeaponPatch));
                harmony.PatchAll(typeof(StylePatch));
            }

            if (ConfigManager.StartupCheck.value)
            {
                SceneManager.activeSceneChanged += OnSceneChange;
            }
        }

        public void Update()
        {
            ConfigManager.Update();
        }
        void OnSceneChange(Scene before, Scene after)
        {
            if (SceneHelper.CurrentScene == "Main Menu" && firstTime && ConfigManager.StartupCheck.value)
            {
                if (AssetHandler.bundleLoaded)
                {
                    MonoSingleton<HMRPlus>.Instance.SendHudMessageEffect(
                    "Guns O' Plenty <grad=rainbow>Successfully</grad> Loaded!"
                    , 0, 4f, false, 1 / 2f);
                } else
                {
                    MonoSingleton<HMRPlus>.Instance.SendHudMessageEffect(
                    "Guns O' Plenty <grad=crimson>Failed</grad> to Load!"
                    , 0, 4f, false, 1 / 2f);
                }
                firstTime = false;
            }
        }
    }
}
