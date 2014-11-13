using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace CXLUI
{
	public class UIWidgetManager : UIBaseManager {

		List<WidgetInfo> widgetList = new List<WidgetInfo>();

		bool beforTweenOver = false; 

		readonly int maxCount = 1;

		WidgetInfo current;
		public WidgetInfo Current {
			get {
				return current;
			}
		}

		override protected void Awake ()
		{
			base.Awake ();
		}


		override public IEnumerator Init ( UIManager mng)
		{
			this.uiMng = mng;
			yield break;
		}

		private void MarkHurry(string info = "Start")
		{
			Debug.Log(string.Format ("---- {0} ----", info));
		}

		private void MarkIdle( string info = "Over")
		{
			Debug.Log(string.Format ("---- {0} ----", info));
		}

		public override void Hide ()
		{
			MarkHurry("Start All Hide");
			HideWidget();
		}

        public void Destroy(string uiName)
        {
            int index = Exist(uiName);
            if (index != -1)
            {
                Destroy(index);
            }
        }

        public override GameObject Show(string uiName, AssetResourecesInfo beforPath)
		{
			MarkHurry();

			//	if greater maxCount delete first
			if (widgetList.Count.Equals( maxCount) && !widgetList[widgetList.Count - 1].uiName.Equals(uiName))
			{
                Destroy(0);
                //if ( current != null && current.Equals(widgetList[0]))
                //{
                //    //	if widget show state, hide widget
                //    UITool.HideTransfrom(current.obj.transform);
                //    current = null;
                //}

                //uiMng.DelayClear.Add( widgetList[0].obj);
                //widgetList.RemoveAt(0);
			}


			GameObject child = null;
			int index = Exist( uiName);
			if ( index != -1)
			{
				return DisposeOriginalWidget( index, uiName);
			}
			else
			{
				//	load and hide current!
				child = UITool.Load( uiName, beforPath, mObject);

				//	sort panel depth
				UITool.SetStructOrderDepth( child, 20);

				WidgetInfo info = new WidgetInfo( child);

				HideWidget();

				InitWidget( ref info);
			}



			return child;
		}

		/// <summary>
		/// Disposes the original widget.
		/// </summary>
		/// <returns>The original widget.</returns>
		/// <param name="index">Index.</param>
		/// <param name="uiName">User interface name.</param>
		private GameObject DisposeOriginalWidget( int index, string uiName)
		{
			if ( current == null)
			{
				//	All widget hide, Directly show current widget.
				ActivateWidget( index);
				UITool.ShowTransfrom( current.obj.transform);
				current.wb.ShowTween();
				
				return current.obj;
			}

			//	if equal return now
			if ( current.uiName.Equals(uiName))
				return current.obj;

			//	hide current
			ActivateWidget( index);


			return widgetList[index].obj;
		}



		private void InitWidget( ref WidgetInfo info)
		{
			current = info;
			widgetList.Add( info);
			
			info.wb.tweenFinish = OnWidgetTweenFinish;
			info.wb.initFinish = OnWidgetInitFinish;
			info.wb.Init();
		}

		private void ActivateWidget( int index)
		{
			HideWidget();
			current = widgetList[index];
			widgetList.RemoveAt(index);
			widgetList.Add(current);
		}

		private void HideWidget()
		{
			if ( current != null)
			{
				current.wb.HideTween();
				current = null;
			}
			else
			{
				beforTweenOver = true;
			}
		}

        private void Destroy(int index)
        {
            if (current != null && current.Equals(widgetList[index]))
            {
                //	if widget show state, hide widget
                UITool.HideTransfrom(current.obj.transform);
                current = null;
            }
            uiMng.DelayClear.Add(widgetList[index].obj);
            widgetList.RemoveAt(index);
        }

		private int Exist( string uiName)
		{
			return widgetList.FindIndex( info => { return info.uiName.Equals(uiName);});
		}

		private void OnWidgetInitFinish( UIWidgetBase wb)
		{

			USERDebug.Log( wb.name + " Init Over!");

			if ( beforTweenOver && current.wb == wb)
			{
				current.wb.ShowTween();
				beforTweenOver = false;
				USERDebug.Log(current.uiName + " start tween!");
			}
		}

		private void OnWidgetTweenFinish(UIWidgetBase wb)
		{
			if ( current == null)
			{
				USERDebug.Log("all widget hide");
				UITool.HideTransfrom( wb.transform);

				MarkIdle();
			}
			else if ( current.wb == wb)
			{
				//	do something
				USERDebug.Log(current.uiName + " Show Animation Over!");

				MarkIdle();
			}
			else
			{
				UITool.HideTransfrom( wb.transform);

				if ( current.wb.IsInit)
				{
					UITool.ShowTransfrom( current.obj.transform);
					current.wb.ShowTween();
					beforTweenOver = false;
					USERDebug.Log("befor widget hide! play current animation!");
				}
				else
				{
					beforTweenOver = true;
					USERDebug.Log("befor widget hide! but current not init!");
				}
			}
		}
	}

	public class WidgetInfo
	{
		public WidgetInfo( GameObject go)
		{
			obj = go;
			uiName = obj.name;
			wb = obj.GetComponent<UIWidgetBase>();
			startTime = Time.realtimeSinceStartup;
		}
		public UIWidgetBase wb;
		public GameObject obj;
		public string uiName;
		public float startTime;
	}
}
