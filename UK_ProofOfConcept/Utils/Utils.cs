using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace GunsOPlenty.Utils
{
    public static class GOPUtils
    {
        public static Color[] RainbowGrad =
        {
            new Color(1, 0, 0),
            new Color(1, 1, 0),
            new Color(0, 1, 0),
            new Color(0, 1, 1),
            new Color(0, 0, 1),
            new Color(1, 0, 1)
        };

        public static Color[] FireGrad =
        {
            new Color(1, 0, 0),
            new Color(1, 0.8f, 0),
            new Color(1, 0, 0),
            new Color(1, 0.5f, 0)
        };

        public static Color[] CrimsonGrad =
        {
            new Color(0.85f, 0, 0),
            new Color(0.15f, 0, 0)
        };

        public static Color[] DarkBlueGrad =
        {
            HexToColor("#373B44"),
            HexToColor("#4286f4")
        };

        public static Color HexToColor(string hex)
        {
            Color color = new Color();
            if (ColorUtility.TryParseHtmlString(hex, out color));
            return color;
        }
        public static Color LerpColorArray(Color[] grad, float t) // proud of this :)
        {
            float n = t % grad.Length;
            return Color.Lerp(grad[(int)Math.Floor(n)], grad[(int)Math.Ceiling(n) % (grad.Length)], n - (int)Math.Floor(n));
        }

        public static Color LerpColorArrayNorm(Color[] grad, float t)
        {
            return LerpColorArray(grad, t * grad.Length);
            //float n = (t * grad.Length) % grad.Length;
            //return Color.Lerp(grad[(int)Math.Floor(n)], grad[(int)Math.Ceiling(n) % (grad.Length)], n - (int)Math.Floor(n));
        }
        public static string IntsToHexString(int r, int g, int b)
        {
            string rh, gh, bh;
            if (r < 16)
            {
                rh = "0" + r.ToString("X");
            } else
            {
                rh = r.ToString("X");
            }

            if (g < 16)
            {
                gh = "0" + g.ToString("X");
            }
            else
            {
                gh = g.ToString("X");
            }

            if (b < 16)
            {
                bh = "0" + b.ToString("X");
            }
            else
            {
                bh = b.ToString("X");
            }

            return rh + gh + bh;
        }

        public static string ColorToHexString(UnityEngine.Color color)
        {
            return IntsToHexString((int)(color.r * 255), (int)(color.g * 255), (int)(color.b * 255));
        }
        public static string ModDir()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string GameDir()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public static string ModPath()
        {
            return Assembly.GetExecutingAssembly().Location;
        }
        public static Quaternion RandRot(float x, float y, float z)
        {
            return Quaternion.Euler(
                    UnityEngine.Random.Range(-x, x),
                    UnityEngine.Random.Range(-y, y),
                    UnityEngine.Random.Range(-z, z)
                );
        }
        public static Quaternion RandRot(float range)
        {
            return Quaternion.Euler(
                    UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range)
                );
        }

        public static Vector3 RandPos(float x, float y, float z)
        {
            return new Vector3(
                    UnityEngine.Random.Range(-x, x),
                    UnityEngine.Random.Range(-y, y),
                    UnityEngine.Random.Range(-z, z)
                );
        }

        public static Vector3 RandPos(float range)
        {
            return new Vector3(
                    UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range),
                    UnityEngine.Random.Range(-range, range)
                );
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
                    //Debug.Log(LayerMask.LayerToName(i) + " " + i);
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

        // maybe https://stackoverflow.com/questions/62553142/how-to-copy-values-of-a-component-from-object-a-to-the-same-component-on-object
    }
}
