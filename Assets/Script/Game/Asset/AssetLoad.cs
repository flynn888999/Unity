using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using AssetResources = AssetConfigFolder;




/**
 * 
 *      资源加载管理
 *      
 *      2014-11-13 15:44:06
 *      1.  由于WWW下载为异步下载,所以建立缓存表,
 *          所有模块初始化时应当把自己模块所有需要的资源加载到缓存表里.
 *      2.  保证所有资源入口都是AssetLoad接口    
 * 
 * **/

public class AssetLoad 
{

    private static AssetLoad _instance = null;

    //  C:\Users\fc-home\Desktop\work\SelfFramwork\Assets\LocalAsset
    private static string _localAssetPath = null;

    private Dictionary<string, int> resourcesMap = new Dictionary<string,int>();

    private Dictionary<string, int> folderMap = new Dictionary<string,int>();

    private List<AssetResources> resourcesList = new List<AssetResources>();

    //  预加载缓存表
    private string asyncName;
    private Dictionary<string, AssetCacheInfo> cacheInfo = new Dictionary<string, AssetCacheInfo>();

    //  加载完成
    public EventDelegate.Callback OnLoadAsyncFinish;
    private bool isLoadArray = false;

    #region Attribute

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
                    case RuntimePlatform.OSXEditor:
                        _localAssetPath = Application.dataPath + "/LocalAsset";
                        break;
                }
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


    /// <summary>
    /// 加载一批资源并卸载之前的资源
    /// </summary>
    public static void LoadAsyncCacheUnloadBefor(string[] resNames)
    {
        BetterList<string> unLoad = new BetterList<string>();

        int length = resNames.Length;
        for (int i = 0; i < length; i++)
        {
            if (!IsCache(resNames[i]))
                unLoad.Add(resNames[i]);
        }

        foreach (string item in unLoad)
        {
            Instance.UnLoadCache(item);
        }

        LoadAsyncCache(resNames);
    }


    /// <summary>
    /// 加载一组资源到缓存列表
    /// </summary>
    /// <param name="resNames"></param>
    public static void LoadAsyncCache(string[] resNames)
    {
        Instance.isLoadArray = true;
        GameGlobalCommunity.Instance.StartCoroutine(Instance.LoadLocalArray(resNames));
    }



    /// <summary>
    /// 加载一个AssetBundle到缓存
    /// </summary>
    /// <param name="resName"></param>
    public static void LoadAsyncCache(string resName)
    {
        Instance.isLoadArray = false;
        Instance.LoadLocal(resName);
    }



    /// <summary>
    /// 加载本地资源队列
    /// </summary>
    /// <param name="resNames"></param>
    /// <returns></returns>
    private IEnumerator LoadLocalArray(string[] resNames)
    {
        foreach (string item in resNames)
        {
            LoadLocal(item);
            while (!MLWWWHelp.Instance.IsFinish)
            {
                yield return null;
            }
        }

        //  all load over call
        if (OnLoadAsyncFinish != null)
            OnLoadAsyncFinish();
    }



    /// <summary>
    /// 单个本地资源加载
    /// </summary>
    /// <param name="resName"></param>
    private void LoadLocal(string resName)
    {
        if (IsCache(resName))
        {
            return;
        }

        Instance.asyncName = resName;
        MLWWWHelp.Instance.OnFinish = Instance.OnLoadFinish;

        string URL = _instance.GetLocalDownUrl(resName);
        //  PC上需要加上flie:///
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OS
        URL = "file:///" + URL;
#endif

        MLWWWHelp.Instance.DownLoadAssetBundle(URL);
    }



    /// <summary>
    /// www 帮助类回调
    /// </summary>
    /// <param name="isFinish"></param>
    private void OnLoadFinish( bool isFinish)
    {
        if (isFinish)
        {
            AssetCacheInfo info = new AssetCacheInfo();
            info.binary = MLWWWHelp.Instance.Binary;
            cacheInfo.Add(asyncName, info);
            asyncName = null;

            //  单个回调立马派发
            if (!isLoadArray && OnLoadAsyncFinish != null)
                OnLoadAsyncFinish();
        }
        else
        {
            USERDebug.LogError(string.Format("下载{0}错误!", asyncName));
        }
    }


    /// <summary>
    /// 卸载一个缓存
    /// </summary>
    /// <param name="resName"></param>
    private void UnLoadCache(string resName)
    {
        if (cacheInfo.ContainsKey(resName))
        {
            cacheInfo[resName].UnLoad();
            cacheInfo.Remove(resName);
        }
    }



    /// <summary>
    /// 资源是否存在缓存表
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public static bool IsCache(string resName)
    {
        return Instance.cacheInfo.ContainsKey(resName);
    }




    /// <summary>
    /// 同步加载一个AssetBundle
    /// 必须是存在缓存表里面
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public static AssetBundle Load(string resName)
    {
        if (Instance.cacheInfo.ContainsKey(resName))
        {
            if (Instance.cacheInfo[resName].Bundle == null)
            {
                Instance.cacheInfo[resName].Bundle = AssetBundle.CreateFromMemoryImmediate(Instance.cacheInfo[resName].binary);
                return Instance.cacheInfo[resName].Bundle;
            }
            else
                return Instance.cacheInfo[resName].Bundle;
        }
        return null;
    }


    /// <summary>
    /// 加载单个原子数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="info"></param>
    /// <returns></returns>
    public static T Load<T>(AssetResourecesInfo info) where T : Object
    {
        AssetBundle bundle = Load(info.assetName);
        if (bundle != null)
        {
            return (bundle.Load(info.resName) as T);
        }

        USERDebug.LogError(string.Format("Asset: {0}, ResourcesName: {1} 没有加载成功! {2}", info.assetName, info.resName, USERDebug.PrintCurrentMethod()));
        return null;
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
