using UnityEngine;
using System.Collections;
using System.Reflection;


public enum UITweenEffectType
{
    ScaleSmallToBig,
    ScaleNormalToBig,
    //Hide0001,
}


public interface IConcreteTween
{
    UITweener ObtainTween(GameObject go);
}


public class UITweenFactory  {
    

    private static UITweenFactory _Instance;
    public static UITweenFactory Instance
    {
        get
        {
            if ( _Instance == null)
            {
                _Instance = new UITweenFactory();
            }
            return _Instance;
        }
    }

    //public EventDelegate.Callback Call;

    public UITweener CreateTween(GameObject go,UITweenEffectType eType, EventDelegate.Callback callFinished = null)
    {
        IConcreteTween concreteTween = (IConcreteTween) Assembly.Load("Assembly-CSharp").CreateInstance(eType.ToString());
        UITweener tweener = concreteTween.ObtainTween(go);
        if ( callFinished != null)  tweener.AddOnFinished(callFinished);

        return tweener;
    }


    //public void OnFinished()
    //{
    //    if (Call != null)   Call();
    //}
}




public class ScaleSmallToBig : IConcreteTween
{
    public UITweener ObtainTween(GameObject go)
    {
        UITweener tweener = null;
        go.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        tweener = TweenScale.Begin(go, 0.2f, Vector3.one);
        tweener.delay = 0.1f;
        tweener.animationCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.664f, 1.294f), new Keyframe(1, 1));
        //tweener.onFinished.Clear();
        //tweener.SetOnFinished(OnFinished);
        return tweener;
    }
}


public class ScaleNormalToBig : IConcreteTween
{
    public UITweener ObtainTween(GameObject go)
    {
        UITweener tweener = null;
        go.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        tweener = TweenScale.Begin(go, 0.3f, Vector3.one);
        tweener.delay = 0.1f;
        tweener.animationCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.22f, 5.5f), new Keyframe(1, 1));
        return tweener;
    }
}


/**public class Hide0001:IConcreteTween
{
    public UITweener ObtainTween(GameObject go)
    {
        UITweener tweener = null;

        //Vector3 pos = go.transform.localPosition;
        //tweener = TweenAlphaMove.Begin(go, 0.2f, new Vector3(pos.x, pos.y + 50f), 0f);
        //tweener.delay = 0.1f;

        return tweener;
    }
}**/