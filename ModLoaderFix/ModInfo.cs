using UnityEngine;

namespace ModLoaderFix
{
    [System.Serializable]
    public class ModInfo
    {
        public string Name;
        public string Description ;

        public bool GetValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Description);
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }
 
}