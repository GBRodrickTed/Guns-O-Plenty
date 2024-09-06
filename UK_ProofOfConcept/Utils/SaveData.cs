using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static ChessManager;
using UnityEngine;
using System.Xml.Linq;
using System.IO;
using GameConsole;

namespace GunsOPlenty.Utils
{
    public class SaveDataBase
    {
        public object defaultValue;
        public object objectValue;
        public string path;
        public string container;
        public string name;
        public SaveDataBase(string path, object defaultValue)
        {
            this.defaultValue = defaultValue;
            this.objectValue = defaultValue;
            this.path = path;
        }
    }
    public class SaveData<T> : SaveDataBase
    {
        public static SaveData<T> Bind(string path, string container, string name, T defaultValue)
        {
            SaveData<T> saveData = new SaveData<T>(path, container, name, defaultValue);
            T value;
            if (!SaveManager.TryDeserializeValue(path, container, name, out value))
            {
                if (!SaveManager.SerializeValue(path, container, name, defaultValue))
                {
                    Debug.Log("Error when serializing " + container + ":" + name + "  in file " + path);
                }
                saveData.value = (T)defaultValue;
            } else
            {
                saveData.value = value;
            }
            return saveData;
        }
        public bool Rebind(string path, string container, string name)
        {
            this.path = path;
            this.container = container;
            this.name = name;
            T value;
            if (!SaveManager.TryDeserializeValue(path, container, name, out value))
            {
                if (!SaveManager.SerializeValue(path, container, name, defaultValue))
                {
                    Debug.Log("Error when serializing " + container + ":" + name + "  in file " + path);
                    return false;
                }
                this.value = (T)defaultValue;
            }
            else
            {
                this.value = value;
            }
            return true;
        }
        private T value_m;
        public T value
        {
            get
            {
                return value_m;
            }
            set
            {
                this.value_m = value;
                this.objectValue = value;
            }
        }
        public SaveData(string path, string container, string name, T defaultValue) : base(path, defaultValue)
        {
            this.defaultValue = defaultValue;
            this.value = defaultValue;
            this.container = container;
            this.name = name;
            this.path = path;
        }

        public void SetValue(T value)
        {
            this.value_m = value;
            this.objectValue = value;
            SaveManager.SerializeValue(path, container, name, value);
        }
    }
}
