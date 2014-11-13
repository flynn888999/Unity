using UnityEngine;
using System.Collections;

namespace CXLUI
{
    public class UIWidgetBase : MonoBehaviour
    {

        public delegate void WidgetFinish(UIWidgetBase wb);

        public WidgetFinish tweenFinish;
        public WidgetFinish initFinish;

        protected bool isInit = false;
        public bool IsInit
        {
            get
            {
                return isInit;
            }
        }

        virtual public void Init()
        {
            isInit = true;
            InitFinish();
        }

        virtual public void ShowTween()
        {
            TweenFinish();
        }

        virtual public void HideTween()
        {
            TweenFinish();
        }

        protected void TweenFinish()
        {
            if (tweenFinish != null)
                tweenFinish(this);
        }

        protected void InitFinish()
        {
            if (initFinish != null)
                initFinish(this);
        }


    }
}

