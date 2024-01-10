using System;
using UnityEngine;
using GunsOPlenty.Weapons;

namespace GunsOPlenty.Weapons
{
    public class GoldenGunAnim : MonoBehaviour
    {
        private void Start()
        {
            this.soul = base.GetComponentInParent<GoldenGunSoul>();
        }

        public void ReadyGun()
        {
            this.soul.ReadyGun();
        }
        public void PumpGun()
        {
            this.soul.PumpGun();
        }
        public void UnpumpGun()
        {
            this.soul.UnpumpGun();
        }

        private GoldenGunSoul soul;
    }
}