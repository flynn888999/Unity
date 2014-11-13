using UnityEngine;
using System.Collections;

public class GameStart : MonoBehaviour  {


    void Start()
    {
        if (GameGlobalCommunity.GameGlobalObject == null)
        {
            GameGlobalCommunity.GameGlobalObject = Instantiate(Resources.Load<GameObject>("GameGlobalData"), Vector3.zero, Quaternion.identity) as GameObject;
            DontDestroyOnLoad(GameGlobalCommunity.GameGlobalObject);
            USERDebug.Log("加载游戏主入口!");

            Destroy(gameObject);
        }
    }
}
