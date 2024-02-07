using GunsOPlenty.Utils;
using HarmonyLib;
using UnityEngine;

namespace GunsOPlenty.Stuff
{
    public class StylePatch
    {

        [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitCyberGrindScore))]
        [HarmonyPrefix]
        public static bool no(LeaderboardController __instance)
        {
            if (WeaponHandler.isCheating)
            {
                Debug.Log("no");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitLevelScore))]
        [HarmonyPrefix]
        public static bool nope(LeaderboardController __instance)
        {
            if (WeaponHandler.isCheating)
            {
                Debug.Log("nope");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitFishSize))]
        [HarmonyPrefix]
        public static bool notevenfish(LeaderboardController __instance)
        {
            if (WeaponHandler.isCheating)
            {
                Debug.Log("not even fish");
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.Start))]
        [HarmonyPostfix]
        public static void AddStyleToDict(StyleHUD __instance)
        {
            /*__instance.idNameDict.Add("ultrakill.moneyshot", "<color=#ffb700>MONEY SHOT</color>");
            __instance.freshnessDecayMultiplierDict.Add("ultrakill.moneyshot", 0f);*/
            /*if (!MonoSingleton<StyleHUD>.Instance.idNameDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.Instance.idNameDict.Add("ultrakill.moneyshot", "<color=#ffb700>MONEY SHOT</color>");
            }

            if (!MonoSingleton<StyleHUD>.Instance.freshnessDecayMultiplierDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.Instance.freshnessDecayMultiplierDict.Add("ultrakill.moneyshot", 0f);
            }*/
        }

        [HarmonyPatch(typeof(StyleCalculator), nameof(StyleCalculator.HitCalculator))]
        [HarmonyPostfix]
        public static void SpecialProjectileStyles(StyleCalculator __instance, string hitter, string enemyType, string hitLimb, bool dead, EnemyIdentifier eid = null, GameObject sourceWeapon = null)
        {
            if (hitter == "coin shot")
            {
                __instance.enemiesShot = true;
                if (dead)
                {
                    if (enemyType == "spider") // wtf is a "spider"
                    {
                        __instance.AddPoints(100, "ultrakill.bigkill", eid, sourceWeapon);
                    }
                    else
                    {
                        __instance.AddPoints(50, "ultrakill.kill", eid, sourceWeapon);
                    }
                    __instance.gc.AddKill();
                }
                else
                {
                    //Debug.Log(sourceWeapon.name);
                   __instance.AddPoints(5, "", eid, sourceWeapon);
                }
            }
        }
    }
}
