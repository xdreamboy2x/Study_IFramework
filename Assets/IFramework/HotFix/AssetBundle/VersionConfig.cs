/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.111
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace IFramework
{
    public class VersionConfig : ScriptableObject
    {
        [Serializable]
        public class BundleVersion
        {
            public string bundle;
            public long size;
            public string path;
            [HideInInspector]
            public string crc32;
        }
        [Serializable]
        public class Version : IComparable<Version>
        {
            public int version;
            [HideInInspector]
            public List<BundleVersion> bundleVersions;

            public List<BundleVersion> _delete = new List<BundleVersion>();
            public List<BundleVersion> _change = new List<BundleVersion>();
            public List<BundleVersion> _create = new List<BundleVersion>();

            public Version(int version, List<BundleVersion> versions)
            {
                this.version = version;
                this.bundleVersions = versions;
            }

            public void SetChange(Version last)
            {

                if (last == null)
                {
                    bundleVersions.ForEach((f) => { _create.Add(f); });
                }
                else
                {
                    List<BundleVersion> lastversions = new List<BundleVersion>();
                    lastversions.AddRange(last.bundleVersions);
                    for (int i = 0; i < bundleVersions.Count; i++)
                    {
                        var version = bundleVersions[i];
                        var lastVersion = lastversions.Find((_v) => { return _v.path == version.path; });
                        if (lastVersion == null)
                        {
                            _create.Add(version);
                        }
                        else
                        {
                            if (version.crc32 != lastVersion.crc32)
                            {
                                _change.Add(version);
                            }
                            lastversions.Remove(lastVersion);
                        }
                    }
                    for (int i = 0; i < lastversions.Count; i++)
                    {
                        _delete.Add(lastversions[i]);
                    }
                }

            }

            public int CompareTo(Version other)
            {
                return version.CompareTo(other.version);
            }
        }

        public List<Version> versions = new List<Version>();
#if UNITY_EDITOR
        public void SetVersion(List<BundleVersion> bundleVersions)
        {
            var laset = GetLatestVersion();
            int version = 1;
            if (laset != null)
                version = laset.version + 1;
            Version v = new Version(version, bundleVersions);
            v.SetChange(laset);
            versions.Add(v);
        }
#endif

        public Version GetLatestVersion()
        {
            if (versions.Count > 0)
                versions.Sort();
            return versions.LastOrDefault();
        }
        public Version GetVersion(int version)
        {
            return versions.Find((_v)=> { return _v.version == version; });
        }
    }
}
