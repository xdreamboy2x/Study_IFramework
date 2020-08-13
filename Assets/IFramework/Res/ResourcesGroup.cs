/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.255
 *UnityVersion:   2018.4.17f1
 *Date:           2020-05-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

namespace IFramework.Resource
{
    public class ResourcesGroup : ResourceGroup
    {
        public ResourcesGroup(string name) : base(name)
        {
        }
        public Resource<T> Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(typeof(ResourcesLoader<>).MakeGenericType(typeof(T)), path).resource as Resource<T>;
        }
        public Resource<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            return Load(typeof(AsyncResourcesLoader<>).MakeGenericType(typeof(T)), path).resource as Resource<T>;
        }
    }

}
