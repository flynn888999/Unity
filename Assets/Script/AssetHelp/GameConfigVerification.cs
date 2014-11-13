using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;


/**
 * 
 *      本地资源校检类
 * 
 *      1.  配置文件加载
 *      2.  本地文件对比
 *      3.  收集需要下载的资源
 * 
 * **/
public class GameConfigVerification :  GameConfigState 
{

	/// <summary>
	/// 数据交个下一个步奏用
	/// </summary>
	private BetterList<string> needDownData;
	public BetterList<string> NeedDownData {
		get {
			return needDownData;
		}
	}


    /// <summary>
    /// 配置文件下载路径
    /// </summary>
	private string downURL;
	


	public GameConfigVerification( string URL)
	{
		downURL = URL;
	}
	


	public override void OnStart ()
	{
        NGUIDebug.Log("准备下载配置文件!");
		MLWWWHelp.Instance.OnFinish = DownLoadFinish;
		MLWWWHelp.Instance.DownLoadText(downURL);
	}




	/// <summary>
	/// 下载回调
	/// </summary>
	/// <param name="isFinish">If set to <c>true</c> is finish.</param>
	void DownLoadFinish( bool isFinish)
	{
		if( isFinish)
		{
            NGUIDebug.Log("下载完成准备解析!");
            GameGlobalCommunity.Instance.StartCoroutine(DisposeDownLoadConfig());
		}
		else
		{

		}
	}



	/// <summary>
	///  处理配置文件
	/// </summary>
	/// <returns>The down load config.</returns>
	IEnumerator DisposeDownLoadConfig()
	{
		//	让 MLWWWWHelp 在这一帧结束
		yield return null;

        //AssetBundle ab = MLWWWHelp.Instance.GetDownLoadData<AssetBundle>();
        //TextAsset ta = ab.Load("config") as TextAsset;


        NGUIDebug.Log("开始解析配置文件!");
        AnalysisXMLConfig(MLWWWHelp.Instance.GetDownLoadString());
    
		yield return null;

        NGUIDebug.Log("开始本地校检!");
        //  资源校检
        yield return GameGlobalCommunity.Instance.StartCoroutine(ContrastLocalFile());

        NGUIDebug.Log("配置解析完毕!");
        if (onFinish != null)
        {
            //object obj = needDownData;
            data = needDownData;
            onFinish();
        }
	}



    /// <summary>
    /// 解析XML配置文件
    /// </summary>
    /// <param name="text">Text.</param>
    void AnalysisXMLConfig(TextAsset text)
    {
        AnalysisXMLConfig(text.text);
    }



	void AnalysisXMLConfig( string text)
	{
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(text);
		
		
		XmlElement root = doc.DocumentElement;
		XmlNodeList listNodes = root.SelectNodes("/Config/AssetBundle");
		
		//  获取AssetBundle节点
		XmlNodeList module = listNodes[0].ChildNodes;
		
		//  down site
		MLWWWHelp.Instance.HttpAssetBundleURL = listNodes[0].Attributes["DownURL"].Value + "/";

		/**
         * 
         *  获得资源列表
         *  支持多层次子节点
         *  客户端资源目录与配置表相同
         * 
         * **/
        for (int i = 0; i < module.Count; i++)
        {
            XmlNode item = module[i];

            //  单个文件
            if (item.Name.Equals("Node"))
            {
                //  -1  根文件
                AddFolder(item, -1);
            }
            //  文件夹
            else
            {
                //  -1  根文件夹
                AddFolder(item, -1);
            }
        }

        string debug = "";
        foreach (var item in AssetLoad.Instance.ResourcesList)
        {
            debug += item.FileName + " ";
        }
        NGUIDebug.Log( "服务器资源列表 " + debug);
	}



    /// <summary>
    /// 映射到本地资源管理
    /// </summary>
    /// <param name="node"></param>
    /// <param name="parentIndex"></param>
    void AddFolder(XmlNode node, int parentIndex = -1)
    {
        if (node.Name.Equals("Node"))
        {
            //  文件
            AssetConfigFile ccf = new AssetConfigFile(node.Attributes["Name"].Value, parentIndex, node.Attributes["MD5"].Value);

            //  映射所有文件
            AssetLoad.Instance.ResourcesMap[ccf.FileName] = AssetLoad.Instance.ResourcesList.Count;

            //  完整资源列表
            AssetLoad.Instance.ResourcesList.Add(ccf);
        }
        else
        {
            //  添加一个文件夹
            AssetConfigFolder cf = new AssetConfigFolder(node.Name, parentIndex);
            AssetLoad.Instance.ResourcesList.Add(cf);

            //  保存父节点下标
            int pIndex = AssetLoad.Instance.ResourcesList.Count - 1;

            //  记录所有文件夹下标映射
            AssetLoad.Instance.FolderMap[node.Name] = pIndex;

            //  递归加载子节点
            XmlNodeList childList = node.ChildNodes;
            for (int i = 0; i < childList.Count; i++)
            {
                AddFolder(childList[i], pIndex);
            }
        }
    }



	/// <summary>
	/// 对比本地文件
	/// </summary>
	IEnumerator ContrastLocalFile()
	{
        //  本地
        string assetDir = AssetLoad.LocalAssetPath + "/";

        NGUIDebug.Log("本地数据路径 " + assetDir);

        try
        {
            //  没有则创建资源目录
            MLFileHelp.NoDirectoryCreate(AssetLoad.LocalAssetPath);
        }
        catch (System.Exception e)
        {
            NGUIDebug.Log( "NoDirectoryCreate " + e.Message);
            throw;
        }
		

        //  获取需要下载资源
		needDownData = new BetterList<string>();
		do
		{
            int length = AssetLoad.Instance.ResourcesList.Count;
            string str = "";
            foreach (AssetConfigFolder node in AssetLoad.Instance.ResourcesList)
	        {
                    if (node.IsFolder)
                    {
                        MLFileHelp.NoDirectoryCreate(AssetLoad.LocalAssetPath + "/" + node.FilePath());
                        continue;
                    }
                    AssetConfigFile file = node as AssetConfigFile;

                    str = AssetLoad.LocalAssetPath + "/" + file.FilePath();
                    if (File.Exists(str))
                    {
                        string md5 = MLFileHelp.GetMD5HashFromFile(str);
                        if (file.Md5.Equals(md5))
                        {
                            //  资源正确
                        }
                        else
                        {
                            //  需要下载
                            needDownData.Add(file.FileName);
                        }
                    }
                    else
                    {
                        //  不存在需要下载
                        needDownData.Add(file.FileName);
                    }

                    NGUIDebug.Log("3");

               
                yield return null;
	        }
		} while (false);



		if ( needDownData.size == 0)
            NGUIDebug.Log("没有下载的资源!");
		else
		{
			//	Debug
			string debug = "需要下载的资源 ";
			for (int i = 0; i < needDownData.size; i++)
			{
				debug += needDownData[i] + " ";
			}
            NGUIDebug.Log(debug);
		}
	}
}
