using UnityEngine;
using System.Collections;


public class GameGlobalCommunity : MonoBehaviour {

    private static GameObject sGameGlobalObject = null;

    private static GameGlobalCommunity sInstance = null;

    private bool isAssetType = true;

 
    #region Some Attribute

    public static UnityEngine.GameObject GameGlobalObject
    {
        get { return sGameGlobalObject; }
        set
        {
            if (sGameGlobalObject != null)
                sGameGlobalObject = value;
        }
    }

    public static GameGlobalCommunity Instance
    {
        get { return sInstance; }
    }

    public bool IsAssetType
    {
        get { return isAssetType; }
        set { isAssetType = value; }
    }

    #endregion

    

    void Awake()
    {
        sInstance = this;
        sGameGlobalObject = gameObject;
        sGameGlobalObject.name = "GameGlobalData";
    }

    void Start()
    {
        InitGlobalManager();
    }

    void Update()
    {
        FSM.FsmGameManager.Instance.Update();
    }

    /// <summary>
    /// 全局管理初始化
    /// </summary>
    private void InitGlobalManager()
    {
        //  游戏状态机
        FSM.FsmGameManager.Create();

        //  WWW下载支持
        sGameGlobalObject.AddComponent<MLWWWHelp>();

        //  创建UI管理对象
        CXLUI.UIManager.CheckTheEnvironment();

        //  创建资源管理对象
        AssetLoad.Create();

        //  开始配置校检
        FSM.FsmGameManager.Instance.ChangeState(FSM.FsmState.FsmGameConfigState);
    }
}
