/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.50
 *UnityVersion:   2018.4.24f1
 *Date:           2020-08-20
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Resource
{
    public class AssetDataBaseGroup : ResourceGroup
    {
        public AssetDataBaseGroup(string name) : base(name)
        {
        }
        public Resource<T> Load<T>(string path) where T : UnityEngine.Object
        {
            return Load(typeof(AssetDataBaseLoader<>).MakeGenericType(typeof(T)), path).resource as Resource<T>;
        }
    }
}
