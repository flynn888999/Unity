using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CXLUI
{
	/// <summary>
	/// 
	/// </summary>
	public class UIDialogManager : MonoBehaviour {

		private Transform mTrans;
		private GameObject mObject;

		private UIPanel barrierPanel;
		private Transform barrierTrans;

		Stack<DialogInfo> dialogStack;
		Queue<GameObject> destoryQueue;

		private int baseDepth = 31;

		void Awake()
		{
			mTrans = transform;
			mObject = gameObject;
		}


		public IEnumerator Init()
		{
			barrierTrans = mTrans.FindChild("Barrier").transform;
			barrierPanel = barrierTrans.GetComponent<UIPanel>();
			UITool.HideTransfrom( barrierTrans);

			if ( dialogStack == null)
				dialogStack = new Stack<DialogInfo>();

			if ( destoryQueue == null)
				destoryQueue = new Queue<GameObject>();

			yield break;
		}

        public GameObject ShowDialog(string uiName/*, AssetResourecesInfo beforPath*/)
		{
			try 
			{
				GameObject child = UITool.Load( uiName, /*beforPath,*/ mObject);

				Add( child);

				Tween( child);

				return child;

			} catch (System.Exception ex) {
				USERDebug.LogError( string.Format("UIDialogManager - ShowDialog(...) {0}", ex.Message));
			}

			return null;
		}


		/// <summary>
		/// Add the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		public void Add( GameObject obj)
		{
			int depth = baseDepth;
			if ( dialogStack.Count != 0)
				depth = dialogStack.Peek().depth;

			DialogInfo info = new DialogInfo( obj);
			info.depth = UITool.SetStructOrderDepth( obj, depth);
			info.barrierDepth = depth - 1;
			dialogStack.Push(info);

			UITool.ShowTransfrom(barrierTrans);
			barrierPanel.depth = info.barrierDepth;
		}

		/// <summary>
		/// Closes the dialog.
		/// </summary>
		public void CloseDialog()
		{
			if ( dialogStack.Count == 0)
				return;

			DialogInfo info = dialogStack.Pop();

		

			UITweener tween = info.obj.GetComponent<UITweener>();
			if ( tween != null)
			{
				tween.Toggle();
				tween.SetOnFinished( OnTweenFinish);
				//	destory queue
				destoryQueue.Enqueue(info.obj);
				//	destory interval time > tween time
				UIManager.Instance.DelayClear.Add( info.obj);
			}
			else
			{
				Destroy( info.obj);
			}
		}

		/// <summary>
		/// Tween the specified obj.
		/// </summary>
		/// <param name="obj">Object.</param>
		private void Tween(GameObject obj)
		{
			UIDialogBase ui = obj.GetComponent<UIDialogBase>();
			if ( ui != null)
			{
				UITweenFactory.Instance.CreateTween( obj, UITweenEffectType.ScaleSmallToBig, ui.OnTweenFinish);
			}
		}

		/// <summary>
		/// Raises the tween finish event.
		/// </summary>
		private void OnTweenFinish()
		{
			if ( destoryQueue.Count == 0)
				return;

			//hide gameobject
			GameObject obj = destoryQueue.Dequeue();
			UITool.HideTransfrom(obj.transform);

			//tween over sort barrier panel depth
			if ( dialogStack.Count == 0)
			{
				UITool.HideTransfrom(barrierTrans);
			}
			else
			{
				barrierPanel.depth = dialogStack.Peek().barrierDepth;
			}
		}

		/// <summary>
		/// Raises the barrier click event.
		/// </summary>
		/// <param name="go">Go.</param>
		private void OnBarrierClick( GameObject go)
		{
			if ( dialogStack.Count == 0)
				throw new System.Exception( "UIDialogManager - OnBarrierClick there is no dialog");
			UIDialogBase ui = dialogStack.Peek().obj.GetComponent<UIDialogBase>();
			ui.OnBarrierClick();
		}
	}


	public class DialogInfo
	{
		public DialogInfo( GameObject obj)
		{
			this.obj = obj;
		}
		public GameObject obj;
		public int depth;
		public int barrierDepth;
	}
}