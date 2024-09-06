using GunsOPlenty.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunsOPlenty.Utils
{
    public static class PersistentCanvas
    {
        public static GameObject instance;
        public static void SetupCanvas()
        {
            instance = GameObject.Instantiate<GameObject>(AssetHandler.LoadAsset<GameObject>("Popup Canvas"));
            GameObject.DontDestroyOnLoad(instance);
        }
    }
}
