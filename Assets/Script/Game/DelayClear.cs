using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public struct DelayInfo
{
    //  清理对象
    public GameObject obj;
}


public class DelayClear {


    //  每次清理的时间间隔
    public float clearTime = 2f;

    //  每次清理GC的间隔 如果没有清理列表将不再调用
    //public float clearGcTime = 30f;

    //  每次清理的数量
    public uint clearNum = 1;

    //  清理列表
    private Queue<DelayInfo> mClear = new Queue<DelayInfo>();

    //  时间计算
    private float countTime = 0;

    //  上次清理GC的时间
    //private float beforClearGcTime;


    private DelayInfo peekInfo;


    public DelayClear()
    {
        //beforClearGcTime = Time.realtimeSinceStartup;
    }

   
    public void Update()
    {
        if (mClear.Count == 0)  return;


        //  检查与清理列表
        peekInfo = mClear.Peek();
        Inspect(ref peekInfo); 
    }


    public void Add( GameObject go)
    {
        DelayInfo newInfo;
        newInfo.obj = go;
        mClear.Enqueue(newInfo);
        //UISceneManager.SetGameObjectVisible(go.transform, false);
    }


	public void DestroyScripts( GameObject go)
	{
		MonoBehaviour[] Scripts = go.GetComponents<MonoBehaviour>();
		for (int i = 0; i < Scripts.Length; i++) {
			GameObject.Destroy(Scripts[i]);
		}
	}


    private void Inspect( ref DelayInfo info)
    {
        countTime += Time.deltaTime;
        if (countTime > clearTime)
        {
            countTime = 0f;
            Clear(clearNum);
        }
    }



    private void Clear( uint clearNum, bool isGc = false)
    {
        while( clearNum > 0 && mClear.Count > 0)
        {
            DelayInfo go = mClear.Dequeue();
            GameObject.Destroy(go.obj);
            --clearNum;
        }

        if (isGc || mClear.Count == 0) Gc();
    }


    private void Gc()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}
