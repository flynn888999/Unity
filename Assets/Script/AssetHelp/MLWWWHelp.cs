using UnityEngine;
using System.Collections;
using System;



public enum DownLoadHelpType
{
    None,
    Texture,
    oggVorbis,
    AssetBundle,
    Text,
}



public class MLWWWHelp : MonoBehaviour {

    private static MLWWWHelp _instance;
    public static MLWWWHelp Instance
    {
        get { return _instance; }
    }


	/// <summary>
	/// 持久下载路径
	/// </summary>
    public readonly string _httpURL = "http://192.168.0.100:8080/HttpServer/";

	/// <summary>
	/// AssetBundle URL
	/// </summary>
	private string _httpAssetBundleURL;
	public string HttpAssetBundleURL {
		get 
		{
			return _httpAssetBundleURL;
		}
		set
		{
			_httpAssetBundleURL = value;
		}
	}


	/// <summary>
	/// 本地存储路径
    ///// </summary>
    //private string _localAssetPath = null;
    //public string LocalAssetPath
    //{
    //    //C:/Users/Fc/AppData/LocalLow/DefaultCompany/_Test
    //    get { 
    //        if ( _localAssetPath == null)
    //        {
    //            switch (Application.platform)
    //            {
    //            case RuntimePlatform.Android:
    //                _localAssetPath = "file://" + Application.persistentDataPath;
    //                break;
    //            case RuntimePlatform.IPhonePlayer:
    //                _localAssetPath = Application.persistentDataPath;
    //                break;
    //            case RuntimePlatform.WindowsEditor:
    //                //C:/Users/fc-home/Desktop/work/_Test/Assets
    //            case RuntimePlatform.OSXEditor:
    //                _localAssetPath = Application.dataPath;
    //                break;
    //            }
    //        }
    //        return _localAssetPath; 
    //    }
    //}


	/// <summary>
	/// 下载完成后交给调用者处理
	/// </summary>
    public delegate void CallDownLoadFinish( bool isFinish);
    public CallDownLoadFinish OnFinish;


    private WWW currentWWW;
    public UnityEngine.WWW CurrentWWW
    {
        get { return currentWWW; }
    }


	/// <summary>
	/// 是否完成下载
	/// </summary>
    private bool isFinish = false;
    public bool IsFinish
    {
        get { return isFinish; }
    }

	/// <summary>
	/// 是否是下载中
	/// </summary>
    private bool isDownLoad = false;
    private DownLoadHelpType downLoadType;

	/// <summary>
	/// 下载数据
	/// </summary>
    private UnityEngine.Object data;

	/// <summary>
	/// 二进制数据
	/// </summary>
    private byte[] binary;
    public byte[] Binary
    {
        get { return binary; }
    }


    private string text;

    void Awake()
    {
        _instance = this;
		USERDebug.Log(_httpURL);
    }


    public T GetDownLoadData<T> () where T : UnityEngine.Object
    {
        return data as T;
    }

    public string GetDownLoadString()
    {
        return text;
    }


    public void DownLoadAssetBundle(string URL)
    {
        downLoadType = DownLoadHelpType.AssetBundle;
        StartCoroutine(DownLoadHelp(URL));
    }


    public void DownLoadTexture(string URL)
    {
        downLoadType = DownLoadHelpType.Texture;
        StartCoroutine(DownLoadHelp(URL));
    }

    public void DownLoadAudio(string URL)
    {
        downLoadType = DownLoadHelpType.oggVorbis;
        StartCoroutine(DownLoadHelp(URL));
    }

    public void DownLoadText(string URL)
    {
        downLoadType = DownLoadHelpType.Text;
        StartCoroutine(DownLoadHelp(URL));
    }
        

    public IEnumerator DownLoadHelp(string URL)
    {
        if (isDownLoad)
        {
            Debug.Log("正在下载中...! " );
            yield break;
        }

        isDownLoad = true;
        isFinish = false;


        yield return StartCoroutine(RequestDownLoad(URL));


        if (OnFinish != null)
        {
            //if (isFinish)
            //{
            //    byte[] bytes = currentWWW.bytes;
            //    Debug.Log("文件MD5值是 " + MLFileHelp.GetMD5HashFromBytes(bytes));
            //}

            OnFinish(isFinish);
        }

        isDownLoad = false;
    }


    private IEnumerator RequestDownLoad(string URL)
    {
        USERDebug.Log("下载 " + URL);

        using( currentWWW = new WWW(URL))
        {
            yield return currentWWW;

            if ( currentWWW.error != null)
            {
				USERDebug.LogError(currentWWW.error);
            }
            else if ( currentWWW.isDone)
            {
                isFinish = true;

                //byte[] bytes = currentWWW.bytes;
                //Debug.Log("文件MD5值是 " + MLFileHelp.GetMD5HashFromBytes(bytes));

                //  保留二进制
                binary = currentWWW.bytes;

                switch (downLoadType)
                {
                    case DownLoadHelpType.Texture:
                        {
                            data = currentWWW.texture;
                        }
                        break;
                    case DownLoadHelpType.oggVorbis:
                        {
                            data = currentWWW.audioClip;
                        }
                        break;
                    case DownLoadHelpType.AssetBundle:
                        {
                            data = currentWWW.assetBundle;
                        }
                        break;
                    case DownLoadHelpType.Text:
                        {
                            text = currentWWW.text;
                        }
                        break;
                    default:
                        {
                            // do something
                            USERDebug.LogError("MLWWWHelp => RequestDownLoad(...) 未知错误!");
                        }
                        break;
                }

                if (currentWWW.assetBundle != null)
                {
                    currentWWW.assetBundle.Unload(true);
                }
            }
            else
            {
                throw new Exception("发生一个未知错误!");
            }
        }
    }
}
