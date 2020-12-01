﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.51
 *UnityVersion:   2018.4.24f1
 *Date:           2020-09-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using IFramework.GUITool;
using System;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections.Generic;
using IFramework.GUITool.HorizontalMenuToorbar;
using System.Linq;
#pragma warning disable
namespace IFramework
{
    partial class RootWindow : EditorWindow
    {
        public static class PkgKitTool
        {
            public static class Constant
            {
                public enum ErrorCode
                {
                    Sucess = 200,
                    Email = 201,
                    Password = 202,
                    UserName = 203,
                    AuthenticationCode = 204,
                    Token = 205,
                    Author = 206,
                    Version = 207,
                    Package = 208,
                    Exception = 209,
                    Field = 210,
                }
                public abstract class ResponseModel
                {
                    public ErrorCode code;
                    public string err;

                    internal static T Dispose<T>(string text) where T : ResponseModel
                    {
                        try
                        {
                            //Debug.Log(text);
                            T t = JsonUtility.FromJson<T>(text);
                            return t;
                        }
                        catch (Exception)
                        {
                            _window.ShowNotification(new GUIContent("数据解析错误"));
                            return null;
                        }
                    }

                    internal bool CheckCode()
                    {
                        if (code != ErrorCode.Sucess)
                        {
                            _window.ShowNotification(new GUIContent(code.ToString() + err));
                        }
                        return code == ErrorCode.Sucess;
                    }
                }
                public abstract class TokenModel : ResponseModel
                {
                    public string token;
                }

                public class GetSignupRandomCodeModel : ResponseModel { }
                public class GetChangePasswordRandomCodeModel : ResponseModel { }
                public class ForgetPasswordModel : ResponseModel { }

                public class SignupModel : TokenModel { }
                public class LoginModel : TokenModel
                {
                    public string name;
                }

                public class ChangePasswordModel : TokenModel { }
                public class PutPackageModel : ResponseModel { }
                [Serializable]
                public class PackageListModel : ResponseModel
                {
                    [Serializable]
                    public class PkgShort
                    {
                        public string name;
                        public string author;
                        public string versions;
                        public DateTime time;
                    }
                    public PkgShort[] pkgs = new PkgShort[0];
                }
                [Serializable]
                public class PackageInfosModel : ResponseModel
                {
                    public string name;
                    public string author;

                    public List<VersionInfo> versions = new List<VersionInfo>();
                    [Serializable]
                    public class VersionInfo
                    {
                        public bool preview;
                        public string version;
                        public string describtion;
                        public string dependences;
                        public string assetPath;
                    }

                }

                public class DeletePackageModel : ResponseModel { }

                public class GetPackageUrlModel : ResponseModel
                {
                    public string url;
                }

                //public const string host = "https://localhost:5001/api/v1";
                public const string host = "http://47.116.133.15:5001/api/v1";
                public const string getSignupCode = host + "/User/GetSinupRandomCode";
                public const string login = host + "/User/Login";
                public const string signup = host + "/User/Sinup";
                public const string getChangePasswordCode = host + "/User/GetChangePasswordRandomCode";
                public const string changePassword = host + "/User/ChangePassword";
                public const string forgetPassword = host + "/User/ForgetPassword";
                public const string loginWithToken = host + "/User/LoginWithToken";
                public const string putpackage = host + "/Pkg/PutPkg";
                public const string getpackageList = host + "/Pkg/GetPkglist";
                public const string getpackageInfos = host + "/Pkg/GetPkgInfos";
                public const string getpkgurl = host + "/Pkg/DownLoadPkg";
                public const string deletepackage = host + "/Pkg/DeletePkg";
            }

            static class WebRequest
            {
                private abstract class Request
                {
                    public readonly string url;
                    private readonly Action<UnityWebRequest> _callback;
                    protected UnityWebRequest request;
                    public float progress { get { return request.downloadProgress; } }
                    public bool isDone { get { return request.isDone; } }
                    public string error { get { return request.error; } }
                    protected Request(string url, Action<UnityWebRequest> callback)
                    {
                        this.url = url;
                        this._callback = callback;
                    }

                    public void Start()
                    {
                        request.SendWebRequest();
                    }

                    public void Compelete()
                    {
                        EditorUtility.ClearProgressBar();
                        if (!string.IsNullOrEmpty(request.error))
                        {
                            _window.ShowNotification(new GUIContent(request.error));
                            return;
                        }
                        if (_callback != null)
                        {
                            _callback.Invoke(request);
                        }
                        request.Abort();
                        request.Dispose();
                    }
                    public static long GetTimeStamp(bool bflag = true)
                    {
                        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                        long ret;
                        if (bflag)
                            ret = Convert.ToInt64(ts.TotalSeconds);
                        else
                            ret = Convert.ToInt64(ts.TotalMilliseconds);
                        return ret;
                    }
                }
                private class Request_Get : Request
                {
                    public Request_Get(string url, Action<UnityWebRequest> callback, Dictionary<string, object> forms) : base(url, callback)
                    {
                        string newUrl = url;
                        if (forms != null && forms.Count > 0)
                        {
                            newUrl += "?";
                            foreach (var item in forms)
                            {
                                newUrl += string.Format("{0}={1}", item.Key, item.Value) + "&";
                            }
                            newUrl += GetTimeStamp();
                        }
                        else
                        {
                            newUrl += "?" + GetTimeStamp();
                        }
                        request = UnityWebRequest.Get(newUrl);
                    }
                }
                private class Request_Post : Request
                {
                    public Request_Post(string url, Action<UnityWebRequest> callback, WWWForm forms) : base(url, callback)
                    {
                        if (forms == null)
                        {
                            forms = new WWWForm();
                        }
                        url += "?" + GetTimeStamp();
                        request = UnityWebRequest.Post(url, forms);
                    }
                }
                private const int maxRequest = 20;
                private static Queue<Request> _waitRequests;
                private static List<Request> _requests;
                private static void Update()
                {
                    if (_waitRequests.Count <= 0 && _requests.Count <= 0) return;
                    while (_requests.Count < maxRequest && _waitRequests.Count > 0)
                    {
                        var _req = _waitRequests.Dequeue();
                        _req.Start();
                        _requests.Add(_req);
                    }

                    for (int i = _requests.Count - 1; i >= 0; i--)
                    {
                        var _req = _requests[i];
                        if (_req.isDone)
                        {
                            _req.Compelete();
                            _requests.Remove(_req);
                            //  break;
                        }
                        else
                        {
                            EditorUtility.DisplayProgressBar("Post Request", _req.url, _req.progress);
                        }
                    }
                }
                private static void Run(Request request)
                {
                    if (_waitRequests == null)
                    {
                        _waitRequests = new Queue<Request>();
                        _requests = new List<Request>();
                        EditorEnv.update += Update;
                    }
                    _waitRequests.Enqueue(request);
                }



                public static void GetRequest(string url, Dictionary<string, object> forms, Action<UnityWebRequest> callback)
                {
                    Run(new Request_Get(url, callback, forms));
                }
                public static void PostRequest(string url, WWWForm forms, Action<UnityWebRequest> callback)
                {
                    Run(new Request_Post(url, callback, forms));
                }
                private static void GetRequest<T>(string url, Dictionary<string, object> forms, Action<T> callback) where T : Constant.ResponseModel
                {
                    GetRequest(url, forms, (req) =>
                    {
                        T t = Constant.ResponseModel.Dispose<T>(req.downloadHandler.text);
                        if (t == null) return;
                        bool bo = t.CheckCode();
                        if (bo && callback != null) callback.Invoke(t);
                    });
                }
                private static void PostRequest<T>(string url, WWWForm forms, Action<T> callback) where T : Constant.ResponseModel
                {
                    PostRequest(url, forms, (req) =>
                    {
                        T t = Constant.ResponseModel.Dispose<T>(req.downloadHandler.text);
                        if (t == null) return;
                        bool bo = t.CheckCode();
                        if (bo && callback != null) callback.Invoke(t);
                    });
                }

                public static void GetSignupCode(string email, Action<Constant.GetSignupRandomCodeModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    PostRequest<Constant.GetSignupRandomCodeModel>(Constant.getSignupCode, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void Signup(string email, string password, string name, string code, Action<Constant.SignupModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    www.AddField("password", password);
                    www.AddField("name", name);
                    www.AddField("code", code);
                    PostRequest<Constant.SignupModel>(Constant.signup, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void Login(string email, string password, Action<Constant.LoginModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    www.AddField("password", password);
                    PostRequest<Constant.LoginModel>(Constant.login, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void Login(string token, Action<Constant.LoginModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("token", token);
                    PostRequest<Constant.LoginModel>(Constant.loginWithToken, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void GetChangePasswordCode(string email, Action<Constant.GetChangePasswordRandomCodeModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    PostRequest<Constant.GetChangePasswordRandomCodeModel>(Constant.getChangePasswordCode, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void ChangePassword(string email, string password, string code, Action<Constant.ChangePasswordModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    www.AddField("password", password);
                    www.AddField("code", code);
                    PostRequest<Constant.ChangePasswordModel>(Constant.changePassword, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void ForgetPassword(string email, Action<Constant.ForgetPasswordModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("email", email);
                    PostRequest<Constant.ForgetPasswordModel>(Constant.forgetPassword, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }


                public static void PutPackage(string name, string author, string version, string describtion, bool preview, string dependences, string assetpath, byte[] buffer, Action<Constant.PutPackageModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("name", name);
                    www.AddField("author", author);
                    www.AddField("version", version);
                    www.AddField("describtion", describtion);
                    www.AddField("assetpath", assetpath);
                    www.AddField("preview", preview.ToString());
                    www.AddField("dependences", dependences);
                    www.AddBinaryData("buffer", buffer);
                    PostRequest<Constant.PutPackageModel>(Constant.putpackage, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }


                public static void GetPackageList(Action<Constant.PackageListModel> callback)
                {
                    PostRequest<Constant.PackageListModel>(Constant.getpackageList, null, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }

                public static void GetPkgInfos(string name, Action<Constant.PackageInfosModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("name", name);
                    PostRequest<Constant.PackageInfosModel>(Constant.getpackageInfos, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }
                public static void DownLoadPkg(string name, string version, Action<UnityWebRequest> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("name", name);
                    www.AddField("version", version);
                    PostRequest(Constant.getpkgurl, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }
                public static void DeletePkg(string author, string name, string version, Action<Constant.DeletePackageModel> callback)
                {
                    WWWForm www = new WWWForm();
                    www.AddField("author", author);
                    www.AddField("name", name);
                    www.AddField("version", version);
                    PostRequest<Constant.DeletePackageModel>(Constant.deletepackage, www, (m) =>
                    {
                        if (callback != null) callback(m);
                    });
                }
            }

            public class SignupInfo : ChangePasswordInfo
            {
                public string name;
            }
            public class ForgetPassworldInfo
            {
                public string email;
            }
            public class LoginInfo : ForgetPassworldInfo
            {
                public string password;
            }
            public class ChangePasswordInfo : LoginInfo
            {
                public string code;
            }

            public class UploadInfo
            {
                public string name;
                public string author { get { return userjson.name; } }
                public int[] version = new int[4];
                public string assetPath;
                public string describtion = "No Describtion";
                public bool preview;
                public string[] dependences = new string[0];
                public byte[] buffer;
            }

            public static event Action onFreshpkgs;

            public static PkgkitInfo.UserJson userjson { get { return _window._windowInfo.userJson; } set { _window._windowInfo.userJson = value; } }
            public static List<PkgKitTool.Constant.PackageInfosModel> pkgs { get { return _window._windowInfo.pkgInfos; } }
            public static bool login { get { return _window._windowInfo.login; } }
            public static string rootPath
            {
                get
                {
                    string path = Path.Combine(Application.persistentDataPath + "/../", EditorEnv.frameworkName + "Memory");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                        File.SetAttributes(path, FileAttributes.Hidden);
                    }
                    return path;
                }
            }

            private static string userjsonPath { get { return rootPath + "/user.json"; } }
            private static string pkgjsonPath { get { return rootPath + "/pkgs.json"; } }
            private static string pkgversionjsonPath
            {
                get
                {
                    string path = rootPath + "/pkgversion";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    return path;
                }
            }
            private static string pkgPath
            {
                get
                {
                    string path = rootPath + "/pkgs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    return path;
                }
            }
            private static string localPkgPath
            {
                get
                {
                    string path = rootPath + "/localpkgs";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    return path;
                }
            }

            public static void OpenMemory()
            {
                EditorTools.OpenFloder(rootPath);
            }

            public static void ClearMemory()
            {
                Logout();
                Directory.Delete(rootPath, true);
            }
            public static void Logout()
            {
                ClearUserJson();
            }
            private static void ClearUserJson()
            {
                userjson = new PkgkitInfo.UserJson();
                if (File.Exists(userjsonPath))
                {
                    File.Delete(userjsonPath);
                }
            }
            private static void WriteUserJson(string name, string token)
            {
                userjson = new PkgkitInfo.UserJson()
                {
                    name = name,
                    token = token
                };
                File.WriteAllText(userjsonPath, JsonUtility.ToJson(userjson, true));
            }

            public static void Init()
            {
                if (login) return;
                if (File.Exists(userjsonPath))
                {
                    userjson = JsonUtility.FromJson<PkgkitInfo.UserJson>(File.ReadAllText(userjsonPath));
                    LoginWithToken();
                }
                else
                {
                    FreshWebPackages();
                }
            }


            public static void GetSignupCode(SignupInfo info)
            {
                WebRequest.GetSignupCode(info.email, (m) => {
                    _window.ShowNotification(new GUIContent("邮件发送成功"));
                });
            }
            public static void Signup(SignupInfo info)
            {
                WebRequest.Signup(info.email, info.password, info.name, info.code, (m) =>
                {
                    WriteUserJson(info.name, m.token);
                    LoginWithToken();
                    _window.ShowNotification(new GUIContent("注册成功"));
                });
            }
            private static void LoginWithToken()
            {
                WebRequest.Login(userjson.token, (m) =>
                {
                    FreshWebPackages();
                });
            }
            public static void FreshWebPackages()
            {
                _window._windowInfo.pkgInfos.Clear();
                Action<Constant.PackageInfosModel, int> freshWindowInfo = (info, max) => {

                    _window._windowInfo.pkgInfos.Add(info);
                    if (max <= _window._windowInfo.pkgInfos.Count)
                    {
                        if (onFreshpkgs != null)
                        {
                            onFreshpkgs();
                        }
                    }
                };
                WebRequest.GetPackageList((m) =>
                {
                    Constant.PackageListModel local = new Constant.PackageListModel();
                    if (File.Exists(pkgjsonPath))
                    {
                        local = JsonUtility.FromJson<Constant.PackageListModel>(File.ReadAllText(pkgjsonPath));
                    }
                    File.WriteAllText(pkgjsonPath, JsonUtility.ToJson(m));
                    var ps_l = local.pkgs.ToList();
                    if (m.pkgs.Length == 0)
                    {
                        if (onFreshpkgs != null)
                        {
                            onFreshpkgs();
                        }
                    }
                    for (int i = 0; i < m.pkgs.Length; i++)
                    {
                        var p = m.pkgs[i];
                        int count = ps_l.RemoveAll((_p) => { return _p.name == p.name && _p.time == p.time && _p.versions == p.versions; });
                        string path = Path.Combine(pkgversionjsonPath, p.name + ".json");
                        if (count > 0)
                        {
                            if (File.Exists(path))
                            {
                                freshWindowInfo(JsonUtility.FromJson<Constant.PackageInfosModel>(File.ReadAllText(path)), m.pkgs.Length);
                            }
                            else
                            {
                                WebRequest.GetPkgInfos(p.name, (model) =>
                                {
                                    freshWindowInfo(model, m.pkgs.Length);
                                    File.WriteAllText(path, JsonUtility.ToJson(model, true));
                                });
                            }
                        }
                        else
                        {
                            WebRequest.GetPkgInfos(p.name, (model) =>
                            {
                                freshWindowInfo(model, m.pkgs.Length);
                                File.WriteAllText(path, JsonUtility.ToJson(model, true));
                            });
                        }
                    }
                    ps_l.ForEach((_p) =>
                    {
                        string path = Path.Combine(pkgversionjsonPath, _p.name + ".json");
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    });
                });
            }

            public static void Login(LoginInfo info)
            {
                WebRequest.Login(info.email, info.password, (m) =>
                {
                    WriteUserJson(m.name, m.token);
                    FreshWebPackages();
                });
            }
            public static void ForgetPassword(ForgetPassworldInfo info)
            {
                WebRequest.ForgetPassword(info.email, (m) => {
                    _window.ShowNotification(new GUIContent("邮件发送成功"));
                });
            }


            public static void GetChangePasswordCode(ChangePasswordInfo info)
            {
                WebRequest.GetChangePasswordCode(info.email, (m) => {
                    _window.ShowNotification(new GUIContent("邮件发送成功"));
                });
            }

            public static void ChangePassword(ChangePasswordInfo info)
            {
                WebRequest.ChangePassword(info.email, info.password, info.code, (m) => {
                    _window.ShowNotification(new GUIContent("密码修改成功"));
                });
            }


            public static void UploadPkg(UploadInfo info)
            {
                string file = string.Format("{0}_{1}.unitypackage", info.name, string.Join(".", info.version));
                string path = Path.Combine(localPkgPath, file);
                AssetDatabase.ExportPackage(info.assetPath, path, ExportPackageOptions.Recurse);
                info.buffer = File.ReadAllBytes(path);
                WebRequest.PutPackage(info.name,
                    info.author,
                    string.Join(".", info.version),
                    info.describtion, info.preview,
                    string.Join("@", info.dependences),
                    info.assetPath, info.buffer,
                    (m) => {
                        _window.ShowNotification(new GUIContent("上传成功"));
                        FreshWebPackages();
                    });
            }

            public static void DownLoadPkg(string name, string version)
            {
                WebRequest.DownLoadPkg(name, version, (req) =>
                {
                    string path = Path.Combine(pkgPath, string.Format("{0}_{1}.unitypackage", name, version));
                    File.WriteAllBytes(path, req.downloadHandler.data);
                    AssetDatabase.ImportPackage(path, true);
                    AssetDatabase.Refresh();
                });
            }
            public static void DeletePkg(string name, string version)
            {
                if (!login) return;
                WebRequest.DeletePkg(userjson.name, name, version, (m) =>
                {
                    _window.ShowNotification(new GUIContent("删除成功"));
                    FreshWebPackages();
                });
            }
            internal static void RemoveDir(string assetPath)
            {
                Directory.Delete(assetPath, true);
                AssetDatabase.Refresh();
            }


        }

        class UserOptionWindow
        {
            enum UserOperation
            {
                Signup, Login, ForgetPassword, ChangePassword, Upload, Memory, SelfPkgs, System
            }

            class SystemGUI
            {
                public void OnGUI()
                {
                    GUILayout.Label("System Information");

                    GUILayout.Label("操作系统：" + SystemInfo.operatingSystem);
                    GUILayout.Label("系统内存：" + SystemInfo.systemMemorySize + "MB");
                    GUILayout.Label("处理器：" + SystemInfo.processorType);
                    GUILayout.Label("处理器数量：" + SystemInfo.processorCount);
                    GUILayout.Label("显卡：" + SystemInfo.graphicsDeviceName);
                    GUILayout.Label("显卡类型：" + SystemInfo.graphicsDeviceType);
                    GUILayout.Label("显存：" + SystemInfo.graphicsMemorySize + "MB");
                    GUILayout.Label("显卡标识：" + SystemInfo.graphicsDeviceID);
                    GUILayout.Label("显卡供应商：" + SystemInfo.graphicsDeviceVendor);
                    GUILayout.Label("显卡供应商标识码：" + SystemInfo.graphicsDeviceVendorID);
                    GUILayout.Label("设备模式：" + SystemInfo.deviceModel);
                    GUILayout.Label("设备名称：" + SystemInfo.deviceName);
                    GUILayout.Label("设备类型：" + SystemInfo.deviceType);
                    GUILayout.Label("设备标识：" + SystemInfo.deviceUniqueIdentifier);
                    GUILayout.Space(10);
                    GUILayout.Label("Screen Information");

                    GUILayout.BeginVertical("Box");
                    GUILayout.Label("DPI：" + Screen.dpi);
                    GUILayout.Label("分辨率：" + Screen.currentResolution.ToString());
                    GUILayout.EndVertical();

                }
            }
            class SignupGUI
            {
                PkgKitTool.SignupInfo _signup = new PkgKitTool.SignupInfo();

                public void OnGUI()
                {
                    _signup.email = EditorGUILayout.TextField("Email", _signup.email);
                    {
                        EditorGUILayout.BeginHorizontal();
                        _signup.code = EditorGUILayout.TextField("Code", _signup.code);
                        if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                        {
                            PkgKitTool.GetSignupCode(_signup);
                        }
                        EditorGUILayout.EndHorizontal();
                        _signup.name = EditorGUILayout.TextField("Name", _signup.name);
                        _signup.password = EditorGUILayout.TextField("Password", _signup.password);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                        {
                            PkgKitTool.Signup(_signup);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            class LoginGUI
            {
                private PkgKitTool.LoginInfo _login = new PkgKitTool.LoginInfo();

                public void OnGUI()
                {
                    if (PkgKitTool.login)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button(Contents.logout, GUILayout.Width(Contents.gap * 6)))
                        {
                            PkgKitTool.Logout();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        _login.email = EditorGUILayout.TextField("Email", _login.email);
                        _login.password = EditorGUILayout.TextField("Password", _login.password);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.Space();
                        if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                        {
                            PkgKitTool.Login(_login);
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                }
            }
            class ChangePasswordGUI
            {
                private PkgKitTool.ChangePasswordInfo _change = new PkgKitTool.ChangePasswordInfo();
                public void OnGUI()
                {
                    _change.email = EditorGUILayout.TextField("Email", _change.email);
                    EditorGUILayout.BeginHorizontal();
                    _change.code = EditorGUILayout.TextField("Code", _change.code);
                    if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                    {
                        PkgKitTool.GetChangePasswordCode(_change);
                    }
                    EditorGUILayout.EndHorizontal();
                    _change.password = EditorGUILayout.TextField("Password", _change.password);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                    {
                        PkgKitTool.ChangePassword(_change);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            class ForgetPassworldGUI
            {
                private PkgKitTool.ForgetPassworldInfo _foget = new PkgKitTool.ForgetPassworldInfo();
                public void OnGUI()
                {
                    _foget.email = EditorGUILayout.TextField("Email", _foget.email);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                    {
                        PkgKitTool.ForgetPassword(_foget);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            class UploadGUI
            {
                private PkgKitTool.UploadInfo _upload = new PkgKitTool.UploadInfo();

                public void OnGUI()
                {
                    EditorGUILayout.LabelField("Author", PkgKitTool.userjson.name);
                    _upload.name = EditorGUILayout.TextField("Name", _upload.name);
                    _upload.preview = EditorGUILayout.Toggle("Preview", _upload.preview);
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Version");
                        GUILayout.Space(100);
                        for (int i = 0; i < _upload.version.Length; i++)
                        {
                            _upload.version[i] = EditorGUILayout.IntField(_upload.version[i]);
                            if (i < _upload.version.Length - 1)
                            {
                                GUILayout.Label(".", GUILayout.Width(Contents.gap));
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.TextField("AssetPath", _upload.assetPath);
                        if (GUILayout.Button(Contents.select, GUILayout.Width(Contents.gap * 5)))
                        {
                            var str = EditorUtility.OpenFolderPanel("AssetPath", Application.dataPath, "");
                            if (str.Contains("Assets") && IO.IsDirectory(str))
                            {
                                _upload.assetPath = str.ToAssetsPath();
                                _upload.name = Path.GetFileNameWithoutExtension(_upload.assetPath);
                            }
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.Label("Dependences", Styles.boldLabel);
                    for (int i = 0; i < _upload.dependences.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        _upload.dependences[i] = EditorGUILayout.TextField(_upload.dependences[i]);
                        if (GUILayout.Button("", Styles.minus, GUILayout.Width(18)))
                        {
                            ArrayUtility.RemoveAt(ref _upload.dependences, i);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("", Styles.plus))
                    {
                        ArrayUtility.Add(ref _upload.dependences, "");
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Label("Describtion", Styles.boldLabel);
                    _upload.describtion = EditorGUILayout.TextArea(_upload.describtion, GUILayout.MinHeight(Contents.gap * 10));

                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button(Contents.go, GUILayout.Width(Contents.gap * 5)))
                        {
                            PkgKitTool.UploadPkg(_upload);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
            class MemoryGUI
            {
                public void OnGUI()
                {
                    if (GUILayout.Button("Open Memory Floder"))
                    {
                        PkgKitTool.OpenMemory();
                    }
                    if (GUILayout.Button("Clear Memory Floder"))
                    {
                        PkgKitTool.ClearMemory();
                    }
                }
            }

            class SelfPkgsGUI
            {
                ListViewCalculator calc = new ListViewCalculator();
                private Vector2 scroll;
                const string name = "name";
                const string version = "version";
                const string minus = "minus";
                class Temp
                {
                    public string name;
                    public string version;
                }
                private static ListViewCalculator.ColumnSetting[] setting = new ListViewCalculator.ColumnSetting[]
                {
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=minus,
                        width=Contents.gap*3
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=name,
                        width=Contents.gap*30
                    },
                    new ListViewCalculator.ColumnSetting()
                    {
                        name=version,
                        width=Contents.gap*10
                    },
                };
                List<Temp> tmps = new List<Temp>();
                public void OnGUI(Rect position)
                {
                    if (!PkgKitTool.login) return;
                    var pkgs = PkgKitTool.pkgs.FindAll((p) => { return p.author == PkgKitTool.userjson.name; });
                    tmps.Clear();

                    pkgs.ForEach((p) => {
                        for (int i = 0; i < p.versions.Count; i++)
                        {
                            tmps.Add(new Temp()
                            {
                                name = p.name,
                                version = p.versions[i].version
                            });
                        }
                    });
                    GUI.BeginClip(position);
                    calc.Calc(new Rect(0, 0, position.width, position.height), Vector2.zero, scroll, 20, tmps.Count, setting);
                    scroll = GUI.BeginScrollView(calc.view, scroll, calc.content);
                    for (int i = calc.firstVisibleRow; i < calc.lastVisibleRow + 1; i++)
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            GUIStyle style = i % 2 == 0 ? Styles.entryBackEven : Styles.entryBackodd;
                            style.Draw(calc.rows[i].position, false, false, false, false);
                        }

                        if (GUI.Button(calc.rows[i][minus].position, "", Styles.minus))
                        {
                            if (EditorUtility.DisplayDialog("Make Sure", string.Format("Confirm to delete the pkg \nName:   {0}\nVersion:   {1}", tmps[i].name, tmps[i].version), "Yes", "Cancel"))
                            {
                                PkgKitTool.DeletePkg(tmps[i].name, tmps[i].version);
                            }
                        }
                        GUI.Label(calc.rows[i][name].position, tmps[i].name);
                        GUI.Label(calc.rows[i][version].position, tmps[i].version);

                    }
                    GUI.EndScrollView();
                    GUI.EndClip();
                }
            }

            public UserOptionWindow()
            {
                _split.fistPan += menu.OnGUI;
                _split.secondPan += ContentGUI;
                menu.ReadTree(Enum.GetNames(typeof(UserOperation)).ToList());
                menu.onCurrentChange += (obj) => {
                    _userOperation = (UserOperation)Enum.Parse(typeof(UserOperation), obj);
                };
            }

            private void ContentGUI(Rect position)
            {
                if (_userOperation == UserOperation.SelfPkgs)
                {
                    _selfpkgs.OnGUI(position);
                }
                position = position.Zoom(AnchorType.MiddleCenter, -10);
                GUILayout.BeginArea(position);
                switch (_userOperation)
                {
                    case UserOperation.Signup:
                        _sign.OnGUI();
                        break;
                    case UserOperation.Login:
                        _login.OnGUI();
                        break;
                    case UserOperation.ChangePassword:
                        GUI.enabled = !PkgKitTool.login;
                        _change.OnGUI();
                        GUI.enabled = true;
                        break;
                    case UserOperation.Upload:
                        GUI.enabled = PkgKitTool.login;
                        _upload.OnGUI();
                        GUI.enabled = true;
                        break;
                    case UserOperation.ForgetPassword:
                        GUI.enabled = !PkgKitTool.login;
                        _forget.OnGUI();
                        GUI.enabled = true;
                        break;
                    case UserOperation.Memory:
                        _memory.OnGUI();
                        break;
                    case UserOperation.System:
                        _sys.OnGUI();
                        break;
                    default:
                        break;
                }
                GUILayout.EndArea();
            }



            private UserOperation _userOperation;
            private MenuTree menu = new MenuTree();
            private SplitView _split = new SplitView();
            private SignupGUI _sign = new SignupGUI();
            private LoginGUI _login = new LoginGUI();
            private ChangePasswordGUI _change = new ChangePasswordGUI();
            private ForgetPassworldGUI _forget = new ForgetPassworldGUI();
            private UploadGUI _upload = new UploadGUI();
            private MemoryGUI _memory = new MemoryGUI();
            private SelfPkgsGUI _selfpkgs = new SelfPkgsGUI();
            private SystemGUI _sys = new SystemGUI();
            public void OnGUI(Rect position)
            {
                _split.OnGUI(position);
            }
        }

        class PkgsWindow
        {
            public PkgsWindow()
            {
                _split.fistPan += menu.OnGUI;
                _split.secondPan += ContentGUI;
                menu.onCurrentChange += (obj) => {
                    name = obj;
                    index = 0;
                    p = _window._windowInfo.pkgInfos.Find((_p) => { return _p.name == name; });
                    versions = p.versions.ToList().ConvertAll((v) => { return "v" + v.version; }).ToArray();
                };

                PkgKitTool.onFreshpkgs += () => {

                    menu.Clear();
                    var list = _window._windowInfo.pkgInfos.ConvertAll((p) => { return p.name; });
                    menu.ReadTree(list);
                    p = null;
                };


                {
                    menu.Clear();
                    var list = _window._windowInfo.pkgInfos.ConvertAll((p) => { return p.name; });
                    menu.ReadTree(list);
                    p = null;
                }
            }
            private PkgKitTool.Constant.PackageInfosModel p;
            string name;
            string[] versions;
            int index;
            private void ContentGUI(Rect position)
            {
                if (p == null) return;
                var version = p.versions[index];
                GUILayout.BeginArea(position.Zoom(AnchorType.MiddleCenter, -10));


                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p.name, Styles.header);
                    if (version.preview)
                    {
                        GUILayout.Label("preview");
                    }
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(Contents.newest, GUILayout.Width(Contents.gap * 6)))
                    {
                        PkgKitTool.DownLoadPkg(name, "");
                    }
                    if (GUILayout.Button(Contents.install, GUILayout.Width(Contents.gap * 6)))
                    {
                        PkgKitTool.DownLoadPkg(name, p.versions[index].version);
                    }
                    index = EditorGUILayout.Popup(index, versions, GUILayout.Width(Contents.gap * 6));
                    using (new EditorGUI.DisabledScope(!Directory.Exists(version.assetPath)))
                    {
                        if (GUILayout.Button(Contents.remove, GUILayout.Width(Contents.gap * 6)))
                        {
                            PkgKitTool.RemoveDir(version.assetPath);
                        }
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("Author: " + p.author, Styles.boldLabel);
                GUILayout.Space(Contents.gap / 2);
                GUILayout.Label("Dependences", Styles.boldLabel);
                var strs = version.dependences.Split('@');
                for (int i = 0; i < strs.Length; i++)
                {
                    GUILayout.Label(strs[i]);
                }
                GUILayout.Space(Contents.gap / 2);
                GUILayout.Label("Describtion", Styles.boldLabel);
                GUILayout.Label(version.describtion);
                GUILayout.EndArea();

                Event e = Event.current;
                if (position.Contains(e.mousePosition) && e.button==1)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy/Name"), false, () => { GUIUtility.systemCopyBuffer = p.name; });
                    menu.AddItem(new GUIContent("Copy/Author"), false, () => { GUIUtility.systemCopyBuffer = p.author; });
                    menu.AddItem(new GUIContent("Copy/AssetPath"), false, () => { GUIUtility.systemCopyBuffer = p.versions[index].assetPath; });
                    menu.AddItem(new GUIContent("Copy/Dependences"), false, () => { GUIUtility.systemCopyBuffer = p.versions[index].dependences; });
                    menu.AddItem(new GUIContent("Copy/Describtion"), false, () => { GUIUtility.systemCopyBuffer = p.versions[index].describtion; });

                    menu.ShowAsContext();
                    if (e.type!= EventType.Layout && e.type!= EventType.Repaint)
                    {
                        e.Use();
                    }
                }
            }

            private MenuTree menu = new MenuTree();
            private SplitView _split = new SplitView();
            public void OnGUI(Rect position)
            {
                _split.OnGUI(position);
            }
        }

        class WindowCollection : IRectGUIDrawer, ILayoutGUIDrawer
        {
            public const string name = "Name";
            public const string dock = "Dock";
            public const string get = "Get";
            public const string close = "Close";
            public const float lineHeight = 20f;
            public static Texture tx = EditorGUIUtility.IconContent("BuildSettings.Editor.Small").image;

            private string _search = "";
            private SearchFieldDrawer _sear;

            private TableViewCalculator _table = new TableViewCalculator();
            private Vector2 _scroll;
            private ListViewCalculator.ColumnSetting[] setting = new ListViewCalculator.ColumnSetting[]
            {
                new ListViewCalculator.ColumnSetting()
                {
                    name=name,
                    width=200
                },
            };
            public WindowCollection()
            {
                _sear = new SearchFieldDrawer("", null, 0);
                _sear.onValueChange += (str) => { _search = str; };
            }
            public void OnGUI(Rect position)
            {
                GUI.BeginClip(position);
                var rs = new Rect(position.x, 0, position.width - 20, position.height).HorizontalSplit(position.height - 2 * lineHeight);

                var fitterWindows = EditorWindowTool.windows.FindAll((w) => { return w.searchName.ToLower().Contains(_search); }).ToArray();
                _table.Calc(rs[0], new Vector2(0, lineHeight), _scroll, lineHeight, fitterWindows.Length, setting);

                Event e = Event.current;

                this.LabelField(_table.titleRow.position, "", Styles.titlestyle)
                    .Pan(() =>
                    {
                        _sear.OnGUI(_table.titleRow[name].position);
                    });
                _scroll = GUI.BeginScrollView(_table.view, _scroll, _table.content);
                for (int i = _table.firstVisibleRow; i < _table.lastVisibleRow + 1; i++)
                {
                    int index = i;
                    EditorWindowTool.EditorWindowItem window = fitterWindows[i];
                    if (e.type == EventType.Repaint)
                    {
                        GUIStyle style = index % 2 == 0 ? Styles.entryBackEven : Styles.entryBackodd;
                        style.Draw(_table.rows[index].position, false, false, _table.rows[i].selected, false);
                    }
                    if (e.button == 0 && e.clickCount == 1 && _table.rows[index].position.Contains(e.mousePosition))
                    {
                        _table.SelectRow(index);
                        _window.Repaint();
                    }
                    if (window.type.Namespace.Contains("UnityEditor"))
                        this.Label(_table.rows[index][name].position, new GUIContent(window.searchName, tx));
                    else
                        this.Label(_table.rows[index][name].position, window.searchName);
                }
                GUI.EndScrollView();
                string windowName = "";
                for (int j = _table.rows.Count - 1; j >= 0; j--)
                {
                    if (_table.rows[j].selected)
                    {
                        windowName = fitterWindows[j].searchName;
                        break;
                    }
                }
                using (new EditorGUI.DisabledGroupScope(_table.selectedRows.Count < 0))
                {
                    this.BeginArea(rs[1])
                            .FlexibleSpace()
                            .BeginHorizontal()
                                .FlexibleSpace()
                                    .Button(() =>
                                    {
                                        var w = EditorWindowTool.FindOrCreate(windowName);
                                        if (w != null)
                                        {
                                            w.Focus();
                                        }
                                    }, get)
                                    .Button(() =>
                                    {
                                        var w = EditorWindowTool.FindOrCreate(windowName);
                                        if (w != null)
                                        {
                                            _window.DockWindow(w, EditorWindowTool.DockPosition.Right);
                                            w.Focus();
                                        }
                                    }, dock)
                                    .Button(() =>
                                    {

                                        EditorWindowTool.FindAll(windowName).ToList().ForEach((w) =>
                                        {
                                            w.Close();
                                        });
                                    }, close)
                             .EndHorizontal()
                        .FlexibleSpace()
                    .EndArea();
                }
                GUI.EndClip();


            }
        }
        enum WindowType
        {
            WindowCollection,
            UserOption,
            Pkgs,
        }
        class Contents
        {
            public const float lineHeight = 20;
            public const float gap = 10;

            public static GUIContent go = new GUIContent("Go");
            public static GUIContent logout = new GUIContent("Logout");
            public static GUIContent select = new GUIContent("Select");
            public static GUIContent remove = new GUIContent("Remove");
            public static GUIContent install = new GUIContent("Install");
            public static GUIContent newest = new GUIContent("Newset");
            public static GUIContent refresh = EditorGUIUtility.IconContent("TreeEditor.Refresh");
        }
        class Styles
        {
            public static GUIStyle titlestyle = GUIStyles.Get("IN BigTitle");

            public static GUIStyle minus = "OL Minus";
            public static GUIStyle plus = "OL Plus";
            public static GUIStyle boldLabel = "BoldLabel";
            public static GUIStyle header = new GUIStyle("BoldLabel")
            {
                fontSize = 20
            };
            public static GUIStyle entryBackodd = GUIStyles.Get("CN EntryBackodd");
            public static GUIStyle entryBackEven = GUIStyles.Get("CN EntryBackEven");
        }

        [Serializable]
        public class PkgkitInfo
        {
            [Serializable]
            public class UserJson
            {
                public string name;
                public string token;
            }
            public List<PkgKitTool.Constant.PackageInfosModel> pkgInfos = new List<PkgKitTool.Constant.PackageInfosModel>();
            public UserJson userJson;
            public bool login
            {
                get
                {
                    if (userJson == null) return false;
                    if (userJson.name == null) return false;
                    return true;
                }
            }
        }
        private PkgkitInfo _windowInfo;
        private static RootWindow _window;
        private UserOptionWindow _userOption;
        private WindowCollection _collection;
        private PkgsWindow _pkgs;
        private ToolBarTree _toolBarTree;
        private WindowType __windowType;
        private WindowType _windowType
        {
            get { return __windowType; }
            set
            {
                if (__windowType != value)
                {
                    __windowType = value;
                }
            }
        }

    }
    partial class RootWindow
    {
        [MenuItem("IFramework/RootWindow",priority =-1000)]
        static void ShowWindow()
        {
            GetWindow<RootWindow>();
        }
        private void OnEnable()
        {
            _window = this;
            if (_windowInfo == null)
            {
                _windowInfo = new PkgkitInfo();
            }
            _collection = new WindowCollection();
            _pkgs = new PkgsWindow();
            _userOption = new UserOptionWindow();
            _toolBarTree = new ToolBarTree();
            PkgKitTool.Init();
            _toolBarTree.Popup((value) => { _windowType = (WindowType)value; }, typeof(WindowType).GetEnumNames(), (int)_windowType)
                .Button(Contents.refresh, (r) => {
                    PkgKitTool.FreshWebPackages();
                }, 20)
                .FlexibleSpace()
                .Label(new GUIContent(PkgKitTool.userjson.name), 100, () => { return PkgKitTool.login; });
            __windowType= EditorTools.Prefs.GetObject<RootWindow, WindowType>("__windowType");
        }
        private void OnDisable()
        {
            EditorTools.Prefs.SetObject<RootWindow,WindowType>("__windowType",  __windowType);
        }
        private void OnGUI()
        {
            var rs = this.LocalPosition().HorizontalSplit(20);

            _toolBarTree.OnGUI(rs[0]);

            var r2 = rs[1].Zoom(AnchorType.UpperCenter, -10);
            switch (_windowType)
            {
                case WindowType.UserOption:
                    _userOption.OnGUI(r2);
                    break;
                case WindowType.Pkgs:
                    _pkgs.OnGUI(r2);
                    break;
                case WindowType.WindowCollection:
                    _collection.OnGUI(r2);
                    break;
                default:
                    break;
            }
        }
    }

}
