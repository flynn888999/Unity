using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssetResources = AssetConfigFolder;


public struct AssetCacheInfo
{
    public byte[] binary; 
}

public class AssetLoad 
{

    private static AssetLoad _instance = null;

    //  C:\Users\fc-home\Desktop\work\SelfFramwork\Assets\LocalAsset
    private static string _localAssetPath = null;
    //private static string _localWriteFolderPath = null;

    private Dictionary<string, int> resourcesMap = new Dictionary<string,int>();

    private Dictionary<string, int> folderMap = new Dictionary<string,int>();

    private List<AssetResources> resourcesList = new List<AssetResources>();

    //  预加载缓存表
    private string asyncName;
    private Dictionary<string, AssetCacheInfo> cacheInfo = new Dictionary<string, AssetCacheInfo>();


    #region Attribute

    //public static string LocalWriteFolderPath
    //{
    //    get {

    //        if (_localWriteFolderPath == null)
    //        {
    //            switch (Application.platform)
    //            {
    //                case RuntimePlatform.Android:
    //                    _localWriteFolderPath = Application.persistentDataPath + "/LocalAsset";
    //                    break;
    //                case RuntimePlatform.IPhonePlayer:
    //                    _localWriteFolderPath = Application.persistentDataPath + "/LocalAsset";
    //                    break;
    //                case RuntimePlatform.WindowsEditor:
    //                //C:/Users/fc-home/Desktop/work/_Test/Assets
    //                case RuntimePlatform.OSXEditor:
    //                    _localWriteFolderPath = Application.dataPath + "/LocalAsset";
    //                    break;
    //            }
    //        }
    //        return _localWriteFolderPath; 
        
    //    }
    //}

    public static string LocalAssetPath
    {
        get
        {
            if (_localAssetPath == null)
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        _localAssetPath = Application.persistentDataPath + "/LocalAsset";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        _localAssetPath = Application.persistentDataPath + "/LocalAsset";
                        break;
                    case RuntimePlatform.WindowsEditor:
                    //C:/Users/fc-home/Desktop/work/_Test/Assets
                    case RuntimePlatform.OSXEditor:
                        _localAssetPath = Application.dataPath + "/LocalAsset";
                        break;
                }

//#if UNITY_EDITOR_WIN || UNITY_EDITOR_OS
//                _localAssetPath = "file:///" + _localAssetPath;
//                USERDebug.Log("本地路径: " + _localAssetPath);
//#endif
            }
            return _localAssetPath;
        }
    }

    public static AssetLoad Instance
    {
        get
        {
            return _instance;
        }
    }

    public static void Create()
    {
        _instance = new AssetLoad();
    }

    public Dictionary<string, int> ResourcesMap
    {
        get { return resourcesMap; }
        set { resourcesMap = value; }
    }

    public Dictionary<string, int> FolderMap
    {
        get { return folderMap; }
        set { folderMap = value; }
    }

    public List<AssetResources> ResourcesList
    {
        get { return resourcesList; }
        set { resourcesList = value; }
    }

    #endregion

    public static void LoadAsyncCache(string[] resNames)
    {
        GameGlobalCommunity.Instance.StartCoroutine(Instance.LoadAsyncCaches(resNames));
    }

    public static void LoadAsyncCache(string resName)
    {
        if (IsCache(resName))
        {
            return;
        }

        Instance.asyncName = resName;
        MLWWWHelp.Instance.OnFinish = Instance.OnLoadFinish;

        string URL = _instance.GetLocalDownUrl(resName);
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OS
        URL = "file:///" + URL;
#endif
        MLWWWHelp.Instance.DownLoadAssetBundle(URL);
    }

    private IEnumerator LoadAsyncCaches(string[] resNames)
    {
        foreach (string item in resNames)
        {
            LoadAsyncCache(item);
            while (!MLWWWHelp.Instance.IsFinish)
            {
                yield return null;
            }
        }
    }

    private void OnLoadFinish( bool isFinish)
    {
        if (isFinish)
        {
            AssetCacheInfo info;
            info.binary = MLWWWHelp.Instance.Binary;
            cacheInfo.Add(asyncName, info);
            asyncName = null;
        }
        else
        {
            USERDebug.LogError(string.Format("下载{0}错误!", asyncName));
        }
    }


    public static bool IsCache(string resName)
    {
        return Instance.cacheInfo.ContainsKey(resName);
    }

    public static AssetBundle Load(string resName)
    {
        AssetBundle ab = null;
        if (Instance.cacheInfo.ContainsKey(resName))
        {
            ab = AssetBundle.CreateFromMemoryImmediate(Instance.cacheInfo[resName].binary);
        }

        return ab;
    }

    public string GetHttpDownUrl(string resName)
    {
        if (resourcesMap.ContainsKey(resName))
        {
            return MLWWWHelp.Instance.HttpAssetBundleURL + resourcesList[resourcesMap[resName]].FilePath();
        }
        return "";
    }

    public string GetLocalDownUrl(string resName)
    {
        if (resourcesMap.ContainsKey(resName))
        {
            return _localAssetPath + "/" + resourcesList[resourcesMap[resName]].FilePath();
        }
        return "";
    }
}
