using UnityEngine;
using BepInEx;
using HarmonyLib;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System;
using GunsOPlenty.Stuff;
using GunsOPlenty.Weapons;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace GunsOPlenty
{
    // To anyone looking at this code, please know that I started this when I was a complete unity noob so some of this code could be done a lot more efficiently.
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        Harmony harmony = new Harmony(PluginInfo.GUID);
        public static string modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static FunnyGun FunGun;


        public void Start()
        {
            WeaponHandler.Patch(harmony);

            FunnyGun.LoadAssets();
            GoldenGun.LoadAssets();

            WeaponHandler.Register(new FunnyGun());
            WeaponHandler.Register(new GoldenGun());

        }
    }
}
