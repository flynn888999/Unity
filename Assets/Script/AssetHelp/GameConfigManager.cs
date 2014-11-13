using UnityEngine;
using System.Collections;


public class MLResourcesModule
{
    public string director;

    System.Collections.Generic.Dictionary<string, MLResourcesNode> childs;
}


public struct MLResourcesNode
{
    public string name;
}


public class GameConfigManager  {

    public string currentInfo;

    public System.Action OnFinish;

	private GameConfigState mState;
    
    
    public GameConfigManager(System.Action finish)
    {
        OnFinish = finish;
    }

    public void StartConfig()
    {
        //OnStateMsg("WEB下载地址: " + MLWWWHelp.Instance.LocalAssetPath);

        mState = new GameConfigVerification(MLWWWHelp.Instance._httpURL + "config.xml");
        mState.selfState = OnStateMsg;
        mState.onFinish = OnStateFinish;
        mState.OnStart();
    }

	public void OnStateMsg( string msg)
	{
        currentInfo += msg + "\n";
		//Debug.Log( msg);
	}

	public void OnStateFinish()
	{
		if ( mState is GameConfigVerification)
		{
			//	配置解析已完成
			BetterList<string> downData = mState.GetData<BetterList<string>>();
			if ( downData == null || downData.size == 0)
			{
				//	数据完好,进入游戏
                //Debug.Log("进入游戏!");
                NGUIDebug.Log("配置资源检测完毕,等待进入游戏初始化!");
                OnOver();
			}
			else
			{
                NGUIDebug.Log("配置文件加载完毕! 准备下载资源到本地!");

				//	启动下载
				mState = new GameConfigResourcesAccess( downData);
				mState.onFinish = OnStateFinish;
				mState.selfState = OnStateMsg;
				mState.OnStart();
			}
		}
		else if ( mState is GameConfigResourcesAccess)
		{
            OnStateMsg("资源下载完毕! 等待进入游戏!");
            OnOver();
		}
	}

    void OnOver()
    {
        if (OnFinish != null)
            OnFinish();
    }

}
