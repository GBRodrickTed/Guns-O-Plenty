using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Weapons
{
    public class WowthrowerAnim : MonoBehaviour
    {
        private void Start()
        {
            this.soul = base.GetComponentInParent<WowthrowerSoul>();
        }

        public void BigShootIsTalking()
        {
            this.soul.BigShootIsTalking();
        }
        public void BigShootEnd()
        {
            this.soul.BigShootEnd();
        }

        private WowthrowerSoul soul;
    }
}
