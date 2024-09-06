using GunsOPlenty.Utils;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using Object = UnityEngine.Object;
using GunsOPlenty.Data;
using GunsOPlenty.Trials.UI;
using UnityEngine.Analytics;

namespace GunsOPlenty.Trials
{
    public class AndroidUser : Trial
    {
        static AndroidUser instance;
        public override string Name { get; protected set; }
        public override string ID { get; protected set; }
        public override string GUID { get; protected set; }
        public override Sprite Icon { get; protected set; }
        public override List<string> Tribulations { get; protected set; }
        public override List<string> TipsAndTricks { get; protected set; }
        public override List<string> Reward { get; protected set; }
        public override bool IsEnabled { get; protected set; }
        Harmony harmony;
        string level;
        public bool shouldHaveTrial = false;
        public bool levelStarted = false;
        public bool levelCompleted = false;
        public override void Init()
        {
            instance = this;
            Name = "Android User";
            ID = "android_user";
            GUID = (PluginInfo.TrialGUID + "." + ID);
            Icon = AssetHandler.LoadAsset<Sprite>("android_user_icon");
            Tribulations = new List<string> {
                "Beat level 2-3",
                "Beat the level on <link=https://www.youtube.com/watch?v=o6hzud-TEmI>android settings</link>",
                "Finish the level with an S rank or higher"
            };
            TipsAndTricks = new List<string>();
            Reward = new List<string> {
                "Funny Gun"
            };
            FunFacts = new List<string> {
                "Funny Gun was the first gun added to the mod (if you couldn't tell already)",
                "Click on android settings"
            };
            IsEnabled = false;
            level = "Level 2-3";
            harmony = new Harmony(GUID);
        }
        public override void StartTrial()
        {
            shouldHaveTrial = true;
            //Shader.SetGlobalFloat("_ResY", 36f);
            //Shader.SetGlobalFloat("_TextureWarping", value);
            //MonoSingleton<PrefsManager>.Instance.SetInt("vertexWarping", stuff);
            SceneHelper.LoadScene(level);
            Enable();
        }
        public override void Enable()
        {
            if (IsEnabled == false)
            {
                IsEnabled = true;
                harmony.PatchAll(this.GetType());
            }
        }

        public override void Disable()
        {
            if (IsEnabled == true)
            {
                IsEnabled = false;
                harmony.UnpatchSelf();
                SetupTrial();
            }
        }
        public void Androidify()
        {
            Shader.SetGlobalFloat("_TextureWarping", 1f);
            Shader.SetGlobalFloat("_VertexWarping", 8f);
            Shader.SetGlobalInt("_ColorPrecision", 3);
            Shader.SetGlobalFloat("_DitherStrength", 0);
            PostProcessV2_Handler whytheFUCKisitv2 = MonoSingleton<PostProcessV2_Handler>.Instance;
            if (whytheFUCKisitv2)
            {
                whytheFUCKisitv2.downscaleResolution = 36f;
            }
            /*DownscaleChangeSprite[] array = Object.FindObjectsOfType<DownscaleChangeSprite>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].CheckScale();
            }*/
        }
        public void SetupTrial()
        {

        }
        [HarmonyPatch(typeof(LevelStatsEnabler), nameof(LevelStatsEnabler.Start))]
        [HarmonyPrefix]
        public static void Crash(LevelStatsEnabler __instance)
        {
            /*PostProcessV2_Handler instance = MonoSingleton<PostProcessV2_Handler>.Instance;
            if (instance)
            {
                instance.downscaleResolution = 36f;
            }*/

            //Shader.SetGlobalFloat("_VertexWarping", 0.1f);
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.Update))]
        [HarmonyPrefix]
        public static void Crash1(StatsManager __instance)
        {
            if (!instance.levelCompleted) { instance.Androidify(); }
        }

        //corolates to the exact moment you enter the exit in this specific level
        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.StopTimer))]
        [HarmonyPrefix]
        public static void LevelComplete(StatsManager __instance)
        {
            instance.levelCompleted = true;
        }

        [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.SendInfo))]
        [HarmonyPostfix]
        public static void RankCheck(StatsManager __instance)
        {
            Debug.Log(instance.Name + " Final Rank " + __instance.rankScore);
            if (__instance.rankScore >= 4 && !__instance.asscon.cheatsEnabled && !__instance.majorUsed)
            {
                UnlockManager.TrialCompleted(instance.ID);
                Debug.Log(instance.Name + " Completed");
                string weaponID = "funny_gun";
                if (UnlockManager.weaponsUnlocked.ContainsKey(weaponID))
                {
                    if (UnlockManager.weaponsUnlocked[weaponID].value == false)
                    {
                        PopupManager.Instance.CreatePopup("Funny Gun :D Unlocked");
                        UnlockManager.WeaponUnlocked(weaponID);
                    }
                }
            }
            else
            {
                Debug.Log(instance.Name + " Failed " + __instance.rankScore);
            }
        }

        [HarmonyPatch(typeof(OptionsManager), nameof(OptionsManager.QuitMission))]
        [HarmonyPrefix]
        public static void QuitCheck(OptionsManager __instance)
        {
            instance.Disable();
        }
        [HarmonyPatch(typeof(SceneHelper), nameof(SceneHelper.LoadScene))]
        [HarmonyPrefix]
        public static bool LevelChangeCheck(SceneHelper __instance)
        {
            if (instance.shouldHaveTrial)
            {
                instance.shouldHaveTrial = false;
            }
            instance.Disable();
            instance.levelStarted = false;
            /*if (instance.levelCompleted)
            {
                MonoSingleton<SceneHelper>.Instance.StartCoroutine(MonoSingleton<SceneHelper>.Instance.LoadSceneAsync("Main Menu", true));
                return false;
            }*/
            return true;
        }
    }
}
