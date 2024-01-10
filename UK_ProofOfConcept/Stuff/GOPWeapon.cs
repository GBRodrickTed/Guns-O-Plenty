using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Stuff
{
    public class GOPWeapon
    {
        public virtual GameObject Create(Transform parent)
        {
            return new GameObject();
        }

        public virtual int Slot()
        {
            return 0;
        }

        public virtual int WheelOrder()
        {
            return 0;
        }
        public virtual string Name()
        {
            return "Template Gun";
        }

        public virtual string Pref()
        {
            return "tpg0";
        }
        public int Enabled()
        {
            return PrefsManager.Instance.GetInt("weapon." + Pref());
        }
    }
}
