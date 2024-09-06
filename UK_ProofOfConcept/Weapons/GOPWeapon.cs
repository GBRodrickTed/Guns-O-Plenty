//using GameConsole.Commands;
using GunsOPlenty.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace GunsOPlenty.Stuff
{
    public abstract class GOPWeapon
    {
        public virtual GameObject Asset { get; protected set; }
        public virtual GameObject LoadedAsset { get; protected set; }
        public virtual int Slot { get; set; }
        public virtual string Name { get; protected set; }
        public virtual string ID { get; protected set; }
        public virtual bool IsSetup { get; protected set; }
        public virtual bool ShouldHave { get; set; }
        public virtual void Setup()
        {
            Name = "Generic Gop Weapon";
            ID = "generic_gop_weapon";
        }
        public void UnSetup()
        {
            Debug.Log("UnSetting Up " + Name);
            Asset = null;
            LoadedAsset = null;
            IsSetup = false;
        }
        public void SetSlot(int newSlot)
        {
            Slot = newSlot;
        }
        public void Create(Transform transform)
        {
            LoadedAsset = UnityEngine.Object.Instantiate(Asset, transform);
        }

        public virtual void UpdateConfigSettings()
        {

        }
    }
}
