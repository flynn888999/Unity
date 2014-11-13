using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 *  全局资源关联
 *  资源对应路径
 * 
 * **/

public struct AssetResourecesInfo
{
    public AssetResourecesInfo(string assetname, string resname)
    {
        this.assetName = assetname;
        this.resName = resname;
    }
    public string assetName;
    public string resName;
}

public struct AssetPath
{

    public static Dictionary<string, AssetResourecesInfo> UIPath
    {
        get
        {
            if (GameGlobalCommunity.Instance.IsAssetType)
                return assetUiPath;
            return uiPath;
        }
    }

    public static Dictionary<string, AssetResourecesInfo> ResourecesUIPath
    {
        get
        {
            return uiPath;
        }
    }

    public static Dictionary<string, AssetResourecesInfo> AssetUIPath
    {
        get
        {
            return assetUiPath;
        }
    }

    public static Dictionary<string, string> ScriptPath
    {
        get
        {
            return scriptPath;
        }
    }




    //  AssetBundle Load 路径资源映射表
    private static Dictionary<string, AssetResourecesInfo> assetUiPath = new Dictionary<string, AssetResourecesInfo>{
        {"Login",   new AssetResourecesInfo("Login.assetbundle","Login")},
    };


    //  Resources Load 路径映射表
    private static Dictionary<string, AssetResourecesInfo> uiPath = new Dictionary<string, AssetResourecesInfo>{ 
		{"UI", new AssetResourecesInfo( "UIPrefab/UI", null)},
		{"UIRoot",new AssetResourecesInfo("UIPrefab/UIRoot",null)},


        //  UI
        {"UIProgress",new AssetResourecesInfo("UIPrefab/",null)},
        {"Login",new AssetResourecesInfo("UIPrefab/",null)}
	};


    private static Dictionary<string, string> scriptPath = new Dictionary<string, string>()
    {
        {"UI_Login","UILoginLogic"},
    };
}



