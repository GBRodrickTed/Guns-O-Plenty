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
            Debug.Log("no");
            return false;
        }

        [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitLevelScore))]
        [HarmonyPrefix]
        public static bool nope(LeaderboardController __instance)
        {
            Debug.Log("nope");
            return false;
        }

        [HarmonyPatch(typeof(LeaderboardController), nameof(LeaderboardController.SubmitFishSize))]
        [HarmonyPrefix]
        public static bool notevenfish(LeaderboardController __instance)
        {
            Debug.Log("not even fish");
            return false;
        }

        [HarmonyPatch(typeof(StyleHUD), nameof(StyleHUD.Start))]
        [HarmonyPostfix]
        public static void AddStyleToDict(StyleHUD __instance)
        {
            /*__instance.idNameDict.Add("ultrakill.moneyshot", "<color=#ffb700>MONEY SHOT</color>");
            __instance.freshnessDecayMultiplierDict.Add("ultrakill.moneyshot", 0f);*/
            if (!MonoSingleton<StyleHUD>.instance.idNameDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.instance.idNameDict.Add("ultrakill.moneyshot", "<color=#ffb700>MONEY SHOT</color>");
            }

            if (!MonoSingleton<StyleHUD>.instance.freshnessDecayMultiplierDict.ContainsKey("ultrakill.moneyshot"))
            {
                MonoSingleton<StyleHUD>.instance.freshnessDecayMultiplierDict.Add("ultrakill.moneyshot", 0f);
            }
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
                        __instance.AddPoints(100, "ultrakill.bigkill", eid, null);
                    }
                    else
                    {
                        __instance.AddPoints(50, "ultrakill.kill", eid, null);
                    }
                    __instance.gc.AddKill();
                }
                else
                {
                    //Debug.Log(sourceWeapon.name);
                   __instance.AddPoints(5, "", eid, null);
                }
            }
        }
    }
}
