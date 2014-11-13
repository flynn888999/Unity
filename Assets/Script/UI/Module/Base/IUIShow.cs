using UnityEngine;
using System.Collections;

namespace CXLUI
{
	public interface IUIShow 
	{
		GameObject ShowWidget( string uiName);
		
		GameObject ShowDialog( string uiName);

		void CloseDialog();	
	}
}


