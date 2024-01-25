using GunsOPlenty.Weapons;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Stuff
{
    public static class WeaponHandler
    {
        static List<GOPWeapon> weapons = new List<GOPWeapon>();
        public static GOPWeapon weapon;
        public static GameObject thingy;
        private static GameObject initObj;
        public static void AddWeapons()
        {
            weapons.Add(new TestCube());
            weapons.Add(new FunnyGun());
            weapons.Add(new GoldenGun());
        }
        public static void SetupWeapons()
        {
            AddWeapons();
            foreach (GOPWeapon weapon in weapons)
            {
                weapon.Setup();
            }
        }
        public static void AddGuns()
        {
            foreach (GOPWeapon weapon in weapons)
            {
                if (weapon != null)
                {
                    if (weapon.Slot > 0 && weapon.Slot <= MonoSingleton<GunControl>.Instance.slots.Count) // if inside normal slot range
                    {
                        weapon.Create(MonoSingleton<GunControl>.Instance.transform);
                        AddToStyleDict(weapon.LoadedAsset);
                        weapon.LoadedAsset.SetActive(false);
                        MonoSingleton<GunControl>.Instance.slots[weapon.Slot - 1].Add(weapon.LoadedAsset);
                        //MonoSingleton<GunControl>.Instance.allWeapons.Add(initObj); Demonic

                    }
                    else if (weapon.Slot > MonoSingleton<GunControl>.Instance.slots.Count && weapon.Slot <= 69)
                    {
                        weapon.Create(MonoSingleton<GunControl>.Instance.transform);
                        AddToStyleDict(weapon.LoadedAsset);
                        weapon.LoadedAsset.SetActive(false);
                        int slots = (MonoSingleton<GunControl>.Instance.slots.Count);
                        for (int i = 0; i < (weapon.Slot - slots); i++)
                        {
                            MonoSingleton<GunControl>.Instance.slots.Add(new List<GameObject>());
                        }
                        MonoSingleton<GunControl>.Instance.slots[weapon.Slot - 1].Add(weapon.LoadedAsset);
                    }
                    else
                    {
                        Debug.Log("Weapon out of range");
                    }
                }
                else
                {
                    Debug.Log("Weapon isn't real");
                }
            }
        }

        /*// yes this is necessary no i don't know why
        public static void AddStyle()
        {
            foreach (GOPWeapon weapon in weapons)
            {
                AddToStyleDict(weapon.LoadedAsset);
            }
        }*/

        public static void AddToStyleDict(GameObject obj)
        {
            if (obj != null && !MonoSingleton<StyleHUD>.Instance.weaponFreshness.ContainsKey(obj))
            {
                MonoSingleton<StyleHUD>.Instance.weaponFreshness.Add(obj, 10f);
                Debug.Log(obj.name + " has been added to dictionary");
            }
            else if (MonoSingleton<StyleHUD>.Instance.weaponFreshness.ContainsKey(obj))
            {
                Debug.Log(obj.name + " is already in dictionary");
            }
            else
            {
                Debug.Log(obj.name + " isn't real");
            }
        }
    }
}
