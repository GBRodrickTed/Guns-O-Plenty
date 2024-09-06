using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.ComponentModel;

namespace GunsOPlenty.Utils
{
    //infinitly better that the sandbox customizer one
    public static class SaveManager
    {
        public static bool CreateContainer(string path, string container)
        {
            
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    //because you cant just have a close funtion
                }
            }
            if (!string.Equals(".goop", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                //Debug.Log("Can't create container because \"" + path + "\" is not a goop file.");
                return false;
            }
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            sr.Close();
            string container_name = "[" + container + "]";
            List<string> lines = new List<string>();
            lines = file.Split('\n').ToList();
            foreach (string line in lines)
            {
                if (line.IndexOf(container_name) == 0)
                {
                    //Debug.Log("Container with same name already exists.");//
                    return false;
                }
            }

            string newFile = file + "\n" + container_name;
            
            StreamWriter sw = new StreamWriter(path);
            sw.Write(newFile);
            sw.Close();
            return true;
        }
        public static bool SerializeValue<T>(string path, string container, string name, T value)
        {
            if (!File.Exists(path))
            {
                using (FileStream fs = File.Create(path))
                {
                    //because you cant just have a close funtion
                }
            }
            if (!string.Equals(".goop", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                //Debug.Log("Can't serialize value because \"" + path + "\" is not a goop file.");
                return false;
            }
            if (name.Contains("[") || name.Contains("]"))
            {
                //Debug.Log("Serialized value names cannot contain brackets");
                return false;
            }
            if (name.Contains(" "))
            {
                //Debug.Log("Serialized value names cannot spaces");
                return false;
            }
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            sr.Close();
            string container_name = "[" + container + "]";
            int container_index = -1;
            List<string> lines = new List<string>();
            lines = file.Split('\n').ToList();
            for (int i = 0; i < lines.Count; i++) 
            {
                if (lines[i].IndexOf(container_name) == 0)
                {
                    container_index = i;
                    break;
                }
            }
            if (container_index < 0)
            {
                //Debug.Log("Container \"" + container_name + "\" was not found. Creating new container");
                lines.Add(container_name);
                container_index = lines.Count - 1;
            }
            int value_index = container_index + 1;
            bool value_found = false;
            while (value_index < lines.Count)
            {
                if (lines[value_index].IndexOf('[') == 0)
                {
                    value_found = false;
                    break;
                }
                if (lines[value_index].IndexOf(name + " ") == 0 || lines[value_index].IndexOf(name + "=") == 0)
                {
                    value_found = true;
                    break;
                }
                value_index++;
            }
            if (!value_found)
            {
                if (container_index == lines.Count - 1)
                {
                    lines.Add("");
                } else
                {
                    lines.Insert(value_index, "");
                }
            }
            string value_format = name + " = " + value.ToString();
            lines[value_index] = value_format;
            string newFile = lines[0];
            for (int i = 1; i < lines.Count; i++)
            {
                newFile += "\n" + lines[i];
            }

            StreamWriter sw = new StreamWriter(path);
            sw.Write(newFile);
            sw.Close();
            return true;
        }
        //Note: I'm currently only using this for simple values like ints and bools
        //      I'd probably need to change it if i wanted to serialize/ deserialize arrays or colors
        //      Could make exeptions (i.e. switch(typeof(T)) case typeof(TypeWeWant):...)
        public static T DeserializeValue<T>(string path, string container, string name)
        {
            if (!File.Exists(path))
            {
                Debug.Log("Can't deserialize value because \"" + path + "\" does not exist.");
                return default(T);
            }
            if (!string.Equals(".goop", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log("Can't deserialize value because \"" + path + "\" is not a goop file.");
                return default(T);
            }
            if (name.Contains("[") || name.Contains("]"))
            {
                Debug.Log("Serialized value names cannot contain brackets");
                return default(T);
            }
            if (name.Contains(" "))
            {
                Debug.Log("Serialized value names cannot spaces");
                return default(T);
            }
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            sr.Close();
            string container_name = "[" + container + "]";
            int container_index = -1;
            List<string> lines = new List<string>();
            lines = file.Split('\n').ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IndexOf(container_name) == 0)
                {
                    container_index = i;
                    break;
                }
            }
            if (container_index < 0)
            {
                Debug.Log("Container \"" + container_name + "\" was not found.");
                return default(T);
            }
            int value_index = container_index + 1;
            bool value_found = false;
            while (value_index < lines.Count)
            {
                if (lines[value_index].IndexOf('[') == 0)
                {
                    value_found = false;
                    break;
                }
                if (lines[value_index].IndexOf(name + " ") == 0 || lines[value_index].IndexOf(name + "=") == 0)
                {
                    value_found = true;
                    break;
                }
                value_index++;
            }
            if (!value_found)
            {
                //Debug.Log("Value \"" + name + "\" was not found.");
                return default(T);
            }
            string value_format = lines[value_index];
            int equals_index = value_format.IndexOf("=");
            if (equals_index < 0)
            {
                Debug.Log("Could not parse value \""+name+"\".");
                return default(T);
            }
            string value_string = "";
            for (int i = equals_index+1; i < value_format.Length; i++)
            {
                if (value_format[i] != ' ')
                {
                    value_string = value_format.Substring(i);
                    break;
                }
            }
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.IsValid(value_string))
            {
                return (T)converter.ConvertFromString(value_string);
            }
            Debug.Log("Could not convert found value string to " + typeof(T).Name + ".");
            return default(T);
        }

        public static bool TryDeserializeValue<T>(string path, string container, string name, out T value)
        {
            if (!File.Exists(path))
            {
                //Debug.Log("Can't deserialize value because \"" + path + "\" does not exist.");
                value = default(T);
                return false;
            }
            if (!string.Equals(".goop", Path.GetExtension(path), StringComparison.OrdinalIgnoreCase))
            {
                //Debug.Log("Can't deserialize value because \"" + path + "\" is not a goop file.");
                value = default(T);
                return false;
            }
            if (name.Contains("[") || name.Contains("]"))
            {
                //Debug.Log("Serialized value names cannot contain brackets");
                value = default(T);
                return false;
            }
            if (name.Contains(" "))
            {
                //Debug.Log("Serialized value names cannot spaces");
                value = default(T);
                return false;
            }
            StreamReader sr = new StreamReader(path);
            string file = sr.ReadToEnd();
            sr.Close();
            string container_name = "[" + container + "]";
            int container_index = -1;
            List<string> lines = new List<string>();
            lines = file.Split('\n').ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].IndexOf(container_name) == 0)
                {
                    container_index = i;
                    break;
                }
            }
            if (container_index < 0)
            {
                //Debug.Log("Container \"" + container_name + "\" was not found.");
                value = default(T);
                return false;
            }
            int value_index = container_index + 1;
            bool value_found = false;
            while (value_index < lines.Count)
            {
                if (lines[value_index].IndexOf('[') == 0)
                {
                    value_found = false;
                    break;
                }
                if (lines[value_index].IndexOf(name + " ") == 0 || lines[value_index].IndexOf(name + "=") == 0)
                {
                    value_found = true;
                    break;
                }
                value_index++;
            }
            if (!value_found)
            {
                //Debug.Log("Value \"" + name + "\" was not found.");
                value = default(T);
                return false;
            }
            string value_format = lines[value_index];
            int equals_index = value_format.IndexOf("=");
            if (equals_index < 0)
            {
                //Debug.Log("Could not parse value \"" + name + "\".");
                value = default(T);
                return false;
            }
            string value_string = "";
            for (int i = equals_index + 1; i < value_format.Length; i++)
            {
                if (value_format[i] != ' ')
                {
                    value_string = value_format.Substring(i);
                    break;
                }
            }

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && converter.IsValid(value_string))
            {
                value = (T)converter.ConvertFromString(value_string);
                return true;
            }
            //Debug.Log("Could not convert found value string to " + typeof(T).Name + ".");
            value = default(T);
            return false;
        }
    }
}
