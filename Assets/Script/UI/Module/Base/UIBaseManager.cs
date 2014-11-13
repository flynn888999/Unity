using UnityEngine;
using System.Collections;

namespace CXLUI
{
	public class UIBaseManager : MonoBehaviour {

		protected UIManager uiMng;
		protected Transform mTrans;
		protected GameObject mObject;

		virtual protected void Awake()
		{
			mTrans = transform;
			mObject = gameObject;
		}

		virtual public IEnumerator Init( UIManager mng)
		{
			uiMng = mng;
			yield break;
		}

        virtual public GameObject Show(string uiName/*, AssetResourecesInfo beforPath*/)
		{
			return null;
		}

		virtual public void Hide( )
		{

		}

	}	
}
