using GunsOPlenty.Utils;
using GunsOPlenty.Weapons;
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
            //Debug.Log("Its doint it");
            WeaponHandler.AddWeaponsToInventoy();
        }

        [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.Update))]
        [HarmonyPostfix]
        public static void UpdateStyleWithGOP(StyleHUD __instance)
        {
            //TODO: Make this more efficiant
            if (!MonoSingleton<StyleHUD>.Instance.idNameDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.Instance.idNameDict.Add("ultrakill.moneyshot", "<color=#ffb700>MONEY SHOT</color>");
            }

            if (!MonoSingleton<StyleHUD>.Instance.freshnessDecayMultiplierDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.Instance.freshnessDecayMultiplierDict.Add("ultrakill.moneyshot", 0f);
            }
            //WeaponHandler.AddStyle();
        }

        [HarmonyPatch(typeof(GunControl), nameof(GunControl.UpdateWeaponList))]
        [HarmonyPrefix]
        public static void PreUpdateWithGOP(GunControl __instance)
        {
            //Debug.Log("I come in before you");
        }

        [HarmonyPatch(typeof(GunControl), nameof(GunControl.UpdateWeaponList))]
        [HarmonyPostfix]
        public static void UpdateWithGOP(GunControl __instance)
        {
            //Debug.Log("Funfunfunfunfunfunfun");
        }
    }
}
