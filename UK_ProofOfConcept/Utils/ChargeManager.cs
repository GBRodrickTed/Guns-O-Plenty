using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Utils
{
    public static class ChargeManager
    { //TODO: Could be better organized
        public struct FunGun
        {
            public float fireTime;
            public float fireDelay;
            public float fireMult;
            public float fireMultTime;
            public float fireMultDelay;
        }
        public static FunGun funGun;
        public struct GoldenGun
        {
            public int pumps;
        }
        public static GoldenGun goldenGun;

        public static void Setup()
        {
            funGun.fireDelay = 1f;
            funGun.fireTime = 0f;
            funGun.fireMult = 1f;
            funGun.fireMultTime = 0f;
            funGun.fireMultDelay = 0.1f;

            Debug.Log("IN CHARGE MANAGER");

            Debug.Log(funGun.fireDelay);
            Debug.Log(funGun.fireTime);
            Debug.Log(funGun.fireMult);
            Debug.Log(funGun.fireMultTime);
            Debug.Log(funGun.fireMultDelay);

            goldenGun.pumps = 0;
        }
    }
}
