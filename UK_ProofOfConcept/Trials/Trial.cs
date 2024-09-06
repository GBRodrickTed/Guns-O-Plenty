using BepInEx;
using GameConsole.Commands;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Trials
{
    //TODO: Stop using override in all the trials
    //MAYBE: Have a seperate "TrialInfo" class for easy customization and other stuff.
    public class Trial
    {
        public virtual string Name { get; protected set; }
        public virtual string ID { get; protected set; }
        public virtual string GUID { get; protected set; }
        public virtual List<string> Tribulations { get; protected set; }
        public virtual List<string> TipsAndTricks { get; protected set; }
        public virtual List<string> FunFacts { get; protected set; }
        public virtual List<string> Reward { get; protected set; }
        public virtual Sprite Icon { get; protected set; }
        public virtual bool IsEnabled { get; protected set; }
        public virtual void Init()
        {

        }
        public virtual void StartTrial()
        {
            Enable();
        }
        public virtual void Enable()
        {
            //harmony.PatchAll(this.GetType());
        }
        public virtual void Disable()
        {
            //harmony.UnpatchSelf();
        }
    }
}
