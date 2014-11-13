using UnityEngine;
using System.Collections;


namespace FSM
{

    /**
     * 
     * 2014/11/12
     *      1.  逻辑脚本的初始化
     *      2.  显示Login场景
     *      
     * **/
    public class FsmGameInitState : FsmGameState
    {
        override public void Init()
        {
            
        }

        override public void Enter()
        {
            //  脚本初始化
            GameGlobalScript.Instance.ScriptPath = AssetLoad.LocalAssetPath + "/Script";
            GameGlobalScript.Instance.LoadProject();

            GameGlobalCommunity.Instance.StartCoroutine(Create());
        }

        private IEnumerator Create()
        {
            AssetLoad.LoadAsyncCache("Login.assetbundle");

            while (!AssetLoad.IsCache("Login.assetbundle"))
            {
                yield return null;
            }

            //  显示Login场景
            CXLUI.UIManager.Instance.ShowWidget("Login");
        }


        override public void Leave()
        {
            
        }

        override public void Update()
        {
            
        }

        override public void Destroy()
        {
            
        }
    }
}


