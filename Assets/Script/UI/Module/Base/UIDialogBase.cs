using UnityEngine;
using System.Collections;

namespace CXLUI
{

	public class UIDialogBase : MonoBehaviour 
	{
		public int tweenID = 1;

		virtual public void OnTweenFinish()
		{}

		virtual public void OnBarrierClick()
		{}
	}
}


