using GunsOPlenty.Utils;
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
        public static GameObject thingy;
        private static GameObject initObj;
        public static bool isCheating = false;
        public static void AddWeapons()
        {
            
        }
        public static void SetupWeapons()
        {
            ChargeManager.Setup();
            Debug.Log("Setting up weapon");
            /*if (ConfigManager.FunGunEnable.value) 
            { 
                GOPWeapon weapon = new FunnyGun();
                weapons.Add(weapon); 
            }
            if (ConfigManager.GoldenGunEnable.value)
            {
                GOPWeapon weapon = new GoldenGun();
                weapons.Add(weapon);
            }
            if (ConfigManager.TestCubeEnable.value)
            {
                GOPWeapon weapon = new TestCube();
                weapons.Add(weapon);
            }*/
            weapons.Add(new FunnyGun());
            weapons.Add(new GoldenGun());
            weapons.Add(new TestCube());
            foreach (GOPWeapon weapon in weapons)
            {
                if (!weapon.IsSetup)
                {
                    weapon.Setup();
                }
            }
            /*foreach (GOPWeapon weapon in weapons)
            {
                foreach(Component comp in weapon.Asset.GetComponents(typeof(Component)))
                {
                    Debug.Log(weapon.Name + ": " + comp.GetType().Name);
                }
            }*/
        }
        public static void RemoveWeapons()
        {
            //Debug.Log("Removing weapons");
            foreach (GOPWeapon gopWeapon in weapons)
            {
                foreach(var slot in MonoSingleton<GunControl>.Instance.slots)
                {
                    foreach(var weapon in slot)
                    {
                        if (weapon == gopWeapon.LoadedAsset)
                        {
                            //Debug.Log("Removing: " + weapon.name);
                            slot.Remove(weapon);
                        }
                    }
                    //gopWeapon.UnSetup();
                }
            }
            foreach (var slot in MonoSingleton<GunControl>.Instance.slots)
            {
                if (slot.Count == 0)
                {
                    MonoSingleton<GunControl>.Instance.slots.Remove(slot);
                    //Debug.Log("Removing an empty weapon slot");
                }
            }
            weapons.Clear();
        }
        public static void AddWeaponsToInventoy()
        {
            isCheating = false;
            foreach (GOPWeapon weapon in weapons)
            {
                switch(weapon.Name) // TODO: there must be a better way
                {
                    case "Funny Gun :D":
                        weapon.ShouldHave = ConfigManager.FunGunEnable.value;
                        weapon.Slot = ConfigManager.FunGunSlot.value;
                        isCheating |= ConfigManager.FunGunEnable.value;
                        break;
                    case "Golden Shotgun":
                        weapon.ShouldHave = ConfigManager.GoldenGunEnable.value;
                        weapon.Slot = ConfigManager.GoldenGunSlot.value;
                        isCheating |= ConfigManager.GoldenGunEnable.value;
                        break;
                    case "Test Cube":
                        weapon.ShouldHave = ConfigManager.TestCubeEnable.value;
                        weapon.Slot = ConfigManager.TestCubeSlot.value;
                        isCheating |= ConfigManager.TestCubeEnable.value;
                        break;
                    default:
                        Debug.Log("ummm dafuq?");
                        break;
                }
                if (weapon.ShouldHave)
                {
                    weapon.Create(MonoSingleton<GunControl>.Instance.transform);
                    MonoSingleton<GunControl>.Instance.shud.weaponFreshness.Add(weapon.LoadedAsset, 10f);
                    weapon.LoadedAsset.SetActive(false);
                    int slots = (MonoSingleton<GunControl>.Instance.slots.Count);
                    if (weapon.Slot > slots)
                    {
                        for (int i = 0; i < (weapon.Slot - slots); i++)
                        {
                            MonoSingleton<GunControl>.Instance.slots.Add(new List<GameObject>());
                        }
                    }
                    MonoSingleton<GunControl>.Instance.slots[weapon.Slot - 1].Add(weapon.LoadedAsset);
                }
                MonoSingleton<GunControl>.Instance.UpdateWeaponList(true);
                /*if (weapon != null)
                {
                    if (weapon.Slot > 0 && weapon.Slot <= MonoSingleton<GunControl>.Instance.slots.Count) // if inside normal slot range
                    {
                        Debug.Log("In notmal slots");
                        weapon.Create(MonoSingleton<GunControl>.Instance.transform);
                        AddToStyleDict(weapon.LoadedAsset);
                        weapon.LoadedAsset.SetActive(false);
                        MonoSingleton<GunControl>.Instance.slots[weapon.Slot - 1].Add(weapon.LoadedAsset);
                        //MonoSingleton<GunControl>.Instance.allWeapons.Add(initObj); Demonic

                    }
                    else if (weapon.Slot > MonoSingleton<GunControl>.Instance.slots.Count && weapon.Slot <= 69)
                    {
                        Debug.Log("NOT In notmal slots: " + weapon.Slot + ", " + MonoSingleton<GunControl>.Instance.slots.Count);
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
                }*/
            }
        }

        // yes this is necessary no i don't know why
        public static void AddStyle()
        {
            foreach (GOPWeapon weapon in weapons)
            {
                AddToStyleDict(weapon.LoadedAsset);
            }
        }

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
