using UnityEngine;
using System.Collections;

public class UIProgress : CXLUI.UIWidgetBase {

    private Transform mTrans;
    private UILabel progressLabel = null;

    override public void Init()
    {
        mTrans = transform;
        progressLabel = mTrans.FindChild("Label").GetComponent<UILabel>();
        
        isInit = true;
        InitFinish();
    }

    public void SetString(string str)
    {
        progressLabel.text = str;
    }

    override public void ShowTween()
    {
        TweenFinish();
    }

    override public void HideTween()
    {
        TweenFinish();
    }
}
