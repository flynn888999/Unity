using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

public abstract class USERDebug  {

    public static bool isShow = false;


    public static void Log(string s)
    {
        //if (!isShow) return;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        Debug.Log(s);
#endif
    }


    public static void Log(object o)
    {
        //if (!isShow) return;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        Debug.Log(o);
#endif
    }



    public static void LogError(string s)
    {
        //if (!isShow) return;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        Debug.LogError(s);
#endif
    }

    public static void LogWarning(string s)
    {
        //if (!isShow) return;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        Debug.LogWarning(s);
#endif
    }

    public static void LogException(System.Exception e)
    {
        //if (!isShow) return;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        Debug.LogException(e);
#endif
    }


    public static void LogException(System.Exception e, Object o)
    {
        //if (!isShow) return;
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        Debug.LogException(e, o);
#endif
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void PrintCurrentMethod()
    {
#if UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
        System.Diagnostics.StackFrame sf = st.GetFrame(1);

        Debug.Log(string.Format("{0}->{1}", sf.GetMethod().DeclaringType, sf.GetMethod().Name));
#endif
    }
}
