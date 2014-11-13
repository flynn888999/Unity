using UnityEngine;
using System.Collections;

namespace FSM
{
    /**
     * 
     * 本地资源校检状态
     * 
     *  2014-11-12 12:06:14
     *      1.  本地资源校检
     *      2.  网络资源下载
     *      3.  *需要添加本地资源整合与全局资源关联问题
     *      
     *  des 暂时所有所有资源全部在AssetBundle文件夹
     *      后期整合资源层次关系,包括资源路径的管理
     *      
     *  2014-11-12 22:29:26
     *      1.  整理本地资源管理
     *      2.  映射所有资源包括资源目录
     *      
     * 
     * **/
    public class FsmGameConfigState : FsmGameState
    {
        UIProgress progress = null;
        GameConfigManager config = null;

        override public void Init()
        {}

        override public void Enter()
        {
            //USERDebug.Log("FsmGameConfigState .. Init Over!");
            NGUIDebug.Log("FsmGameConfigState => Enter() - 1");
            progress = CXLUI.UIManager.Instance.ShowWidget("UIProgress").GetComponent<UIProgress>();

            config = new GameConfigManager(OnConfigOver);
            config.StartConfig();
        }

        override public void Leave()
        {
            CXLUI.UIManager.Instance.DestroyWidget("UIProgress");

            progress = null;
            config = null;
        }

        override public void Update()
        {
            progress.SetString(config.currentInfo);
        }

        override public void Destroy()
        {}

        void OnConfigOver()
        {
            //  初始化游戏!
            FsmGameManager.Instance.ChangeState(FsmState.FsmGameInitState);
        }
    }
}


