using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Stuff
{
    public class WeaponHandler
    {
        internal static List<GOPWeapon> WeaponList = new List<GOPWeapon>();
        internal static List<GOPWeapon> WeaponAddendumBucket = new List<GOPWeapon>();
        internal static Dictionary<string, int> WeaponOwned = new Dictionary<string, int>();
        internal static int SlotVar = 0;

        public static void Register(GOPWeapon weapon)
        {
            if (weapon.WheelOrder() <= 5) // if it's within the normal gun slots
            {
                WeaponList.Add(weapon);
            } else
            {
                WeaponAddendumBucket.Add(weapon);
            }

            WeaponOwned.Add("weapon." + weapon.Pref(), 0);
        }

        public static void Patch(Harmony harm)
        {
            harm.PatchAll(typeof(WeaponRegisterPatchs));
        }

        public class WeaponRegisterPatchs
        {
            [HarmonyPatch(typeof(GunSetter), nameof(GunSetter.ResetWeapons))]
            [HarmonyPostfix]
            public static void GiveGuns(GunSetter __instance)
            {
                List<List<GameObject>> SlotToList = __instance.GetComponent<GunControl>().slots;

                foreach (GOPWeapon weapon in WeaponList)
                {
                    if (GameProgressSaver.CheckGear(weapon.Pref()) == 1)
                    {
                        GameObject created = weapon.Create(__instance.transform);
                        created.SetActive(false);
                        SlotToList[weapon.Slot()].Add(created);
                    }
                    else
                    {
                        WeaponOwned["weapon." + weapon.Pref()] = 0;
                    }
                }

                
                foreach (List<GameObject> slot in SlotToList)
                {
                    while (slot.Remove(null))
                        ;
                }
            }

            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.CheckGear))]
            [HarmonyPrefix]
            public static bool CheckGearForCustoms(ref int __result, string gear)
            {
                foreach (GOPWeapon weapon in WeaponList)
                {
                    if (weapon.Pref() == gear)
                    {
                        __result = WeaponOwned["weapon." + weapon.Pref()];
                        return false;
                    }
                }
                return true;
            }

            [HarmonyPatch(typeof(GameProgressSaver), nameof(GameProgressSaver.AddGear))]
            [HarmonyPrefix]
            public static bool AddGearForCustoms(string gear)
            {
                foreach (GOPWeapon weapon in WeaponList)
                {
                    if (weapon.Pref() == gear)
                    {
                        WeaponOwned["weapon." + weapon.Pref()] = 1;
                        return false;
                    }
                }
                return true;
            }

            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start))]
            [HarmonyPostfix]
            public static void AddNewSlots(GunControl __instance)
            {
                Debug.Log("Got here");


                foreach (GOPWeapon weapon in WeaponAddendumBucket)
                {
                    GameObject created = weapon.Create(__instance.transform);
                    created.SetActive(false);
                    created.name = weapon.Name();
                    //__instance.slots[1].Add(created);
                    // Add empty slots until the slot for the weapon is created
                    while (__instance.slots.Count < (weapon.WheelOrder() + 1))
                        __instance.slots.Add(new List<GameObject>());
                    __instance.slots[weapon.WheelOrder()].Add(created);
                }

                foreach (var slot in __instance.slots)
                {
                    while (slot.Remove(null))
                        ;
                }

                /*foreach (List<GameObject> slot in __instance.slots)
                {
                    foreach (GameObject thing in slot)
                    {
                        Debug.Log(thing.name);
                    }
                }*/

                //properly sets last used weapon in a hacky way if it's out of the "Standard" array index
                if (SlotVar > 6)
                {
                    __instance.SwitchWeapon(SlotVar, __instance.slots[SlotVar - 1]);
                }

                __instance.UpdateWeaponList(true);

            }

            [HarmonyPatch(typeof(GunControl), nameof(GunControl.Start))]
            [HarmonyPrefix]
            public static void SaveLastWeapon(GunControl __instance)
            {

                SlotVar = PlayerPrefs.GetInt("CurSlo", 1);

            }

            [HarmonyPatch(typeof(StyleCalculator), nameof(StyleCalculator.HitCalculator))]
            [HarmonyPostfix]
            public static void SpecialProjectileStyles(StyleCalculator __instance ,string hitter, string enemyType, string hitLimb, bool dead, EnemyIdentifier eid = null, GameObject sourceWeapon = null)
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
                        __instance.AddPoints(5, "", eid, sourceWeapon);
                    }
                }
            }
        }
    }
}
