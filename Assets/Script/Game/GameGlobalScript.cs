using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * 
 *  这个类实现脚本的Logger接口，脚本编译时的信息会从Log输出出来
 * 
 * **/

class ScriptLogger : CSLE.ICLS_Logger
{
    public void Log(string str)
    {
        UnityEngine.Debug.Log(str);
    }

    public void Log_Error(string str)
    {
        Debug.LogError(str);
    }

    public void Log_Warn(string str)
    {
        Debug.LogWarning(str);
    }
}


public class GameGlobalScript
{
    public delegate IEnumerator CallCoroutine();

    private static GameGlobalScript _instance;

    private bool isLoadProject = false;
    
    private string scriptPath = "";

    #region Attribute
    public static GameGlobalScript Instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameGlobalScript();
            return _instance;
        }
    }

    public CSLE.CLS_Environment env
    {
        get;
        private set;
    }

    public bool IsLoadProject
    {
        get;
        private set;
    }

    public string ScriptPath
    {
        get { return scriptPath; }
        set { scriptPath = value; }
    }

    #endregion

    private GameGlobalScript( )
    {
        env = new CSLE.CLS_Environment(new ScriptLogger());
        env.logger.Log("C#LightEvil Inited.Ver=" + env.version);
        RegisterType();
    }


    /// <summary>
    /// 注册脚本所用到的类型
    /// </summary>
    public void RegisterType()
    {
        //大部分类型用RegHelper_Type提供即可
        env.RegType(new CSLE.RegHelper_Type(typeof(Vector2)));
        env.RegType(new CSLE.RegHelper_Type(typeof(Vector3)));
        env.RegType(new CSLE.RegHelper_Type(typeof(Vector4)));
        env.RegType(new CSLE.RegHelper_Type(typeof(Time)));

        env.RegType(new CSLE.RegHelper_Type(typeof(Debug)));
        env.RegType(new CSLE.RegHelper_Type(typeof(GameObject)));
        env.RegType(new CSLE.RegHelper_Type(typeof(Component)));
        env.RegType(new CSLE.RegHelper_Type(typeof(Transform)));
        env.RegType(new CSLE.RegHelper_Type(typeof(Resources)));
        env.RegType(new CSLE.RegHelper_Type(typeof(UnityEngine.Object)));

        //对于AOT环境，比如IOS，get set不能用RegHelper直接提供，就用AOTExt里面提供的对应类替换
        //数组要独立注册
        env.RegType(new CSLE.RegHelper_Type(typeof(int[]), "int[]"));           
        //模板类要独立注册
        env.RegType(new CSLE.RegHelper_Type(typeof(List<int>), "List<int>"));

        #region User
        env.RegType(new CSLE.RegHelper_Type(typeof(USERDebug)));
        #endregion

        #region NGUI

        //  监听事件EventDelegate
        env.RegType(new CSLE.RegHelper_Type(typeof(UIEventListener)));
        env.RegType(new CSLE.RegHelper_DeleAction(typeof(EventDelegate), "EventDelegate"));
        env.RegType(new CSLE.RegHelper_DeleAction<GameObject>(typeof(UIEventListener.VoidDelegate), "UIEventListener.VoidDelegate"));

        //  类型
        env.RegType(new CSLE.RegHelper_Type(typeof(UISprite)));
        env.RegType(new CSLE.RegHelper_Type(typeof(UIButton)));
        env.RegType(new CSLE.RegHelper_Type(typeof(UITexture)));
        env.RegType(new CSLE.RegHelper_Type(typeof(UILabel)));
        env.RegType(new CSLE.RegHelper_Type(typeof(UIPopupList)));
        env.RegType(new CSLE.RegHelper_Type(typeof(UIInput)));

        #endregion
    }

    /// <summary>
    /// 加载所有的脚本
    /// </summary>
    public void LoadProject()
    {
        if (isLoadProject)
            return;

        try
        {
            USERDebug.Log("加载逻辑脚本: " + scriptPath);

            string[] files = System.IO.Directory.GetFiles(scriptPath, "*.cs", System.IO.SearchOption.AllDirectories);
            Dictionary<string, IList<CSLE.Token>> project = new Dictionary<string, IList<CSLE.Token>>();
            foreach (var v in files)
            {
                var tokens = env.tokenParser.Parse(System.IO.File.ReadAllText(v));
                project.Add(v, tokens);
            }
            env.Project_Compiler(project, true);

            isLoadProject = true;
        }
        catch (System.Exception e)
        {
            USERDebug.LogError("编译脚本项目失败，请检查" + e.ToString());
        }
    }

    /// <summary>
    /// 执行
    /// Execute("UIMain.Start(\""+name+"\");")
    /// 执行这段代码.
    /// 调用UIMain中的Start方法 参数为 name;
    /// </summary>
    /// <param name="code"></param>
    public void Execute(string code)
    {
        var content = env.CreateContent();

        try
        {
            var tokens = env.ParserToken(code);
            var expr = env.Expr_CompilerToken(tokens);
            expr.ComputeValue(content);
        }
        catch (System.Exception err)
        {
            var dumpv = content.DumpValue();
            var dumps = content.DumpStack(null);
            var dumpSys = err.ToString();
            Debug.LogError(dumpv + dumps + dumpSys);
        }
    }
}
