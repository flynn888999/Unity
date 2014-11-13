using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CXLUI
{

	public class UIManager : MonoBehaviour, IUIShow{

		public static UIManager _instance;
		public static UIManager Instance
		{
			get
			{
				return _instance;
			}
		}
		
		public static Vector2 designRatio = Vector2.zero;
		public static Vector2 DesignRatio 
		{
			get {
				if ( designRatio.Equals( Vector2.zero))
				{
					designRatio = new Vector2( 960, 640);
				}
				return designRatio;
			}
		}

		private Transform mTrans;

		private UICamera uiUICamera;
		private Camera uiCamera;

		private GameObject uiScene;
		private UIDialogManager dialogMng;
		private UIWidgetManager widgetMng;


		private DelayClear delayClear;
		public DelayClear DelayClear {
			get {
				return delayClear;
			}
		}


		void Awake()
		{
			mTrans = transform;
			delayClear = new DelayClear();
		}

		void Start()
		{
			if ( _instance == null)
			{
				CheckTheEnvironment();
			}
		}

		void Init()
		{
			_instance = this;

			//	对话框管理
			dialogMng = mTrans.FindChild("Dialog").GetComponent<UIDialogManager>();
			if ( dialogMng == null)
				dialogMng = mTrans.FindChild("Dialog").gameObject.AddComponent<UIDialogManager>();
			StartCoroutine( dialogMng.Init());

			//	窗口管理
			widgetMng = mTrans.FindChild("Widget").GetComponent<UIWidgetManager>();
			if ( widgetMng == null)
				widgetMng = mTrans.FindChild("Widget").gameObject.AddComponent<UIWidgetManager>();
			StartCoroutine( widgetMng.Init( this));
		}

		void Update()
		{
			if ( delayClear != null)
				delayClear.Update();
		}

		public static void CheckTheEnvironment()
		{
			if ( _instance == null)
			{
				UIRoot root = UIRoot.list.Count == 0 ? null : UIRoot.list[0];

				GameObject rt = null;

				if ( root == null)
				{
                    rt = NGUITools.AddChild(null, Resources.Load<GameObject>("UIPrefab/UIRoot"));
				}
				else
				{
					if ( root.gameObject.layer != LayerMask.NameToLayer("2D UI"))
					{
                        rt = NGUITools.AddChild(null, Resources.Load<GameObject>("UIPrefab/UIRoot"));
					}
					else
					{
						rt = root.gameObject;
						Transform ui = rt.transform.FindChild("UI");
						if ( ui == null)
						{
                            ui = NGUITools.AddChild(rt, Resources.Load<GameObject>("UIPrefab/UI")).transform;
							ui.name = "UI";
						}
					}
				}
				rt.name = "UIRoot";

				//	初始化窗口管理
				UIManager uiMng = rt.transform.FindChild("UI").GetComponent<UIManager>();
				if ( uiMng == null)
				{
					uiMng = rt.transform.FindChild("UI").gameObject.AddComponent<UIManager>();
				}
				uiMng.Init();
			}
		}

		#region IUIShow implementation

		public GameObject ShowWidget (string uiName)
		{
			if ( AssetPath.UIPath.ContainsKey(uiName))
			{
                return widgetMng.Show(uiName/*, AssetPath.UIPath[uiName]*/);
			}
            else if (AssetPath.ResourecesUIPath.ContainsKey(uiName))
            {
                USERDebug.Log(string.Format("{0} AssetBundle 没有该资源尝试从Resources里加载!", uiName));
                GameGlobalCommunity.Instance.IsAssetType = false;
                GameObject child = widgetMng.Show(uiName/*, AssetPath.ResourecesUIPath[uiName]*/);
                GameGlobalCommunity.Instance.IsAssetType = true;
                return child;
            }
            else
            {
                USERDebug.Log("ShowWidget(...) - UIPath without ui path");
            }
            return null;
		}

		public GameObject ShowDialog (string uiName)
		{
			//throw new System.NotImplementedException ();
            if (AssetPath.UIPath.ContainsKey(uiName))
			{
                return dialogMng.ShowDialog(uiName/*, AssetPath.UIPath[uiName]*/);
			}
            else if (AssetPath.ResourecesUIPath.ContainsKey(uiName))
            {
                USERDebug.Log(string.Format("{0} AssetBundle 没有该资源尝试从Resources里加载!", uiName));
                GameGlobalCommunity.Instance.IsAssetType = false;
                GameObject child = widgetMng.Show(uiName/*, AssetPath.ResourecesUIPath[uiName]*/);
                GameGlobalCommunity.Instance.IsAssetType = true;
                return child;
            }
			else
			{
				USERDebug.Log("ShowDialog(...) - UIPath without ui path");
			}
			return null;
		}

		public void CloseDialog ()
		{
			dialogMng.CloseDialog();
		}

		public void HideWidget()
		{
			widgetMng.Hide();
		}

        public void DestroyWidget( string uiName)
        {
            widgetMng.Destroy(uiName);
        }

		#endregion


		virtual public void OnWidgetInitFinish(UIWidgetBase wb)
		{
			wb.ShowTween();
		}

		virtual public void OnWidgetTweenFinish(UIWidgetBase wb)
		{
			// do something
		}

	}
}
