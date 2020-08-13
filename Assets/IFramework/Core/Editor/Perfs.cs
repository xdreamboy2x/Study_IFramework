/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.113
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-30
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.Serialization;
namespace IFramework
{
    public static class Perfs
    {
        static string GetKey<T>(string key)
        {
            return string.Format("{0}/{1}", typeof(T).FullName, key);
        }
        public static void DeleteAll()
        {
            EditorPrefs.DeleteAll();
        }
        public static void DeleteKey<T>(string key)
        {
            EditorPrefs.DeleteKey(GetKey<T>(key));
        }
        public static bool GetBool<T>(string key, bool defaultValue)
        {
            return EditorPrefs.GetBool(GetKey<T>(key), defaultValue);
        }
        public static bool GetBool<T>(string key)
        {
            return EditorPrefs.GetBool(GetKey<T>(key));
        }
        public static float GetFloat<T>(string key, float defaultValue)
        {
            return EditorPrefs.GetFloat(GetKey<T>(key), defaultValue);
        }
        public static float GetFloat<T>(string key)
        {
            return EditorPrefs.GetFloat(GetKey<T>(key));
        }
        public static int GetInt<T>(string key, int defaultValue)
        {
            return EditorPrefs.GetInt(GetKey<T>(key), defaultValue);
        }
        public static int GetInt<T>(string key)
        {
            return EditorPrefs.GetInt(GetKey<T>(key));
        }
        public static string GetString<T>(string key, string defaultValue)
        {
            return EditorPrefs.GetString(GetKey<T>(key), defaultValue);
        }
        public static string GetString<T>(string key)
        {
            return EditorPrefs.GetString(GetKey<T>(key));
        }
        public static bool HasKey<T>(string key)
        {
            return EditorPrefs.HasKey(GetKey<T>(key));
        }
        public static void SetBool<T>(string key, bool value)
        {
            EditorPrefs.SetBool(GetKey<T>(key), value);
        }
        public static void SetFloat<T>(string key, float value)
        {
            EditorPrefs.SetFloat(GetKey<T>(key), value);
        }
        public static void SetInt<T>(string key, int value)
        {
            EditorPrefs.SetInt(GetKey<T>(key), value);
        }
        public static void SetString<T>(string key, string value)
        {
            EditorPrefs.SetString(GetKey<T>(key), value);
        }

        public static void SetObject<T,V>(string key, V value)
        {
            SetString<T>(key, Xml.ToXmlString(value));
        }
        public static V GetObject<T, V>(string key)
        {
            key = GetKey<T>(key);
            if (HasKey<T>(key))
            {
                return Xml.ToObject<V>(EditorPrefs.GetString(key));
            }
            return default(V);
        }
    }

}
