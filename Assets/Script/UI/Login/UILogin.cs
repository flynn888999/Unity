using UnityEngine;
using System.Collections;

public class UILogin : CXLUI.UIWidgetBase {

	

    /// <summary>
    /// 约定模块名称一般情况下不予许更改
    /// </summary>
    private readonly string appointScriptTypeName = "UI_Login";

    UILoginInstance inst;

    void Start()
    {
        inst = new UILoginInstance(appointScriptTypeName);
        inst.gameObject = gameObject;
        inst.Start();
    }

    //  窗口管理接口,暂时不用.

    #region UIWidgetBase virtual
    //override public void Init()
    //{
    //    isInit = true;
    //    InitFinish();
    //}

    //override public void ShowTween()
    //{
    //    TweenFinish();
    //}

    //override public void HideTween()
    //{
    //    TweenFinish();
    //}
    #endregion
}


/**
 * 
 *      登录脚本代理类
 * 
 * **/

class UILoginInstance
{
    CSLE.ICLS_Type type;

    //  脚本实例
    CSLE.SInstance inst;

    //  操作上下文
    CSLE.CLS_Content content;

    public UILoginInstance(string scriptTypeName)
    {
        type = GameGlobalScript.Instance.env.GetTypeByKeywordQuiet( AssetPath.ScriptPath [scriptTypeName]);

        if (type == null)
        {
            USERDebug.LogError("Type:" + scriptTypeName + "不存在于脚本项目中");
            USERDebug.PrintCurrentMethod();
            return;
        }

        content = GameGlobalScript.Instance.env.CreateContent();
        inst = type.function.New(content, null).value as CSLE.SInstance;
        content.CallType = inst.type;
        content.CallThis = inst;

        USERDebug.Log("inst = " + inst);
    }


    public GameObject gameObject
    {
        get
        {
            return inst.member["gameObject"].value as GameObject;
        }

        set
        {
            inst.member["gameObject"].value = value;
        }
    }


    public void Start()
    {
        type.function.MemberCall(content, inst, "Start", null);
    }
}
