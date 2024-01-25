using UnityEngine;
using BepInEx;
using HarmonyLib;
using GunsOPlenty.Stuff;
using GunsOPlenty.Utils;
using GunsOPlenty.Data;

// To anyone looking at this code, hopefully it works now
// Inspired by CrashLibs but all the code is my own
// Feedback is greatly appreciated

namespace GunsOPlenty
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    
    public class Plugin : BaseUnityPlugin
    {
        Harmony harmony = new Harmony(PluginInfo.GUID);

        public void Awake()
        {
            //StartCoroutine(Setup());
        }

        public void Start()
        {
            AssetHandler.LoadBundle();
            if (AssetHandler.bundleLoaded)
            {
                AssetHandler.CreateCustomPrefabs();
                WeaponHandler.SetupWeapons();
                harmony.PatchAll(typeof(WeaponPatch));
                harmony.PatchAll(typeof(StylePatch));
            } else
            {
                Debug.Log("GOP couldn't load");
            }
        }

        public void Update()
        {
            //Instantiate<GameObject>(asset, MonoSingleton<CameraController>.Instance.transform.position, Quaternion.identity);
        }
    }
}
