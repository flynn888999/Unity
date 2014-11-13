using UnityEngine;
using System.Collections;

public class UILoginLogic 
{
    public GameObject gameObject
    {
        get;
        private set;
    }

    private UIPopupList accountPopup;
    private UIInput accountInput;
    private UIInput passwordInput;

    public void Start( )
    {
        Transform trans = gameObject.transform.Find("CenterAnchor/Animation/Widget");

        accountPopup = trans.FindChild("Account").GetComponent<UIPopupList>();
        accountInput = accountPopup.transform.GetComponentInChildren<UIInput>();
        passwordInput = trans.FindChild("Password").GetComponent<UIInput>();

        accountPopup.onChange.Add(new EventDelegate(SetCurrentSelection));

        UIEventListener.Get(trans.FindChild("Login").gameObject).onClick = OnClick;
    }

    public void SetCurrentSelection()
    {
        if (UIPopupList.current != null)
        {
            accountInput.value = UIPopupList.current.isLocalized ?
                             Localization.Get(UIPopupList.current.value) :
                             UIPopupList.current.value;

            //accountInput.value = popupListValue;
        }
    }

    public void OnClick(GameObject go)
    {
        if (go.name.Equals("Login"))
        {
            passwordInput.gameObject.SetActive(!passwordInput.gameObject.activeSelf);
        }
        else
        { 
            
        }
    }
	
}
