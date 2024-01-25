using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Stuff
{
    public static class WeaponPatch
    {

        [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start))]
        [HarmonyPostfix]
        public static void AddGuns(GunControl __instance)
        {
            WeaponHandler.AddGuns();
        }

        [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.Start))]
        [HarmonyPostfix]
        public static void UpdateWithGOP(StyleHUD __instance)
        {
            //Style currently doesn't work
            //WeaponHandler.AddStyle();
        }
    }
}
