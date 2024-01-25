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
        public virtual int Slot { get; protected set; }
        public virtual string Name { get; protected set; }
        public abstract void Setup();
        public void Create(Transform transform)
        {
            LoadedAsset = UnityEngine.Object.Instantiate(Asset, transform);
        }
    }
}
