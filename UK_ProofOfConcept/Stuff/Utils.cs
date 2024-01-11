using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.ResourceManagement.Util.BinaryStorageBuffer.Writer;

namespace GunsOPlenty.Stuff
{
    public static class GOPUtils
    {
        public static string GameDirectory()
        {
            string path = Application.dataPath;
            if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                path = Utility.ParentDirectory(path, 2);
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = Utility.ParentDirectory(path, 1);
            }

            return path;
        }

        public static string ModDirectory()
        {
            return Path.Combine(GameDirectory(), "BepInEx", "plugins");
        }

        public static string ModPath()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static Quaternion RandRot(float x, float y, float z)
        {
            return Quaternion.Euler(
                    UnityEngine.Random.Range(-x, x),
                    UnityEngine.Random.Range(-y, y),
                    UnityEngine.Random.Range(-z, z)
                );
        }
        public static Quaternion RandRot(float spread)
        {
            return Quaternion.Euler(
                    UnityEngine.Random.Range(-spread, spread),
                    UnityEngine.Random.Range(-spread, spread),
                    UnityEngine.Random.Range(-spread, spread)
                );
        }

        public static GameObject ChildByName(this GameObject from, string name)
        {
            List<GameObject> children = new List<GameObject>();
            int count = 0;
            while (count < from.transform.childCount)
            {
                children.Add(from.transform.GetChild(count).gameObject);
                count++;
            }

            if (count == 0)
            {
                return null;
            }

            for (int i = 0; i < children.Count; i++)
            {
                if (children[i].name == name)
                {
                    return children[i];
                }
            }
            return null;
        }

        public static GameObject DescendantByName(this GameObject from, string name)
        {
            if (from.transform.childCount > 0)
            {
            }
            for (int i = 0; i < from.transform.childCount; i++)
            {
                GameObject child = from.transform.GetChild(i).gameObject;
                if (child.name == name)
                {
                    return child;
                }
                GameObject grandChild = DescendantByName(child, name);
                if (grandChild != null)
                {
                    return grandChild;
                }
            }
            return null;
        }

        public static List<GameObject> ChildrenList(this GameObject from)
        {
            List<GameObject> children = new List<GameObject>();
            int count = 0;
            while (count < from.transform.childCount)
            {
                children.Add(from.transform.GetChild(count).gameObject);
                count++;
            }

            return children;
        }
        private static List<GameObject> descendants = new List<GameObject>();
        private static void GatherDescendants(this GameObject from)
        {
            int count = 0;
            while (count < from.transform.childCount)
            {
                descendants.Add(from.transform.GetChild(count).gameObject);
                GatherDescendants(from.transform.GetChild(count).gameObject);
                count++;
            }
        }
        public static List<GameObject> DescendantsList(this GameObject from)
        {
            if (descendants.Count > 0)
            {
                descendants.Clear();
            }
            GatherDescendants(from);
            return descendants;
        }

        public static List<string> LayerMaskNames(LayerMask lmask)
        {
            List<string> layerNames = new List<string>();
            int layerInBytes = 1;
            for (int i = 0; i < 32; i++)
            {
                if ((lmask.value & layerInBytes) > 0)
                {
                    layerNames.Add(LayerMask.LayerToName(i));
                    Debug.Log(LayerMask.LayerToName(i) + " " + i);
                }
                layerInBytes *= 2;
            }
            return layerNames;
        }

        public static int NameToLayerBit(string name)
        {
            int layer = LayerMask.NameToLayer(name);
            if (layer > -1)
            {
                return (int)Math.Pow(2, layer);
            }
            return layer;
        }

        public static List<GameObject> FindSceneObjects(string sceneName)
        {
            List<GameObject> objs = new List<GameObject>();
            foreach (GameObject obj in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (obj.scene.name == sceneName)
                {
                    objs.Add(obj);
                }
            }

            return objs;
        }
    }
}
