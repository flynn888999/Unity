using UnityEngine;
using System.Collections;


public class GameConfigState {

	/// <summary>
	/// 状态回调
	/// </summary>
	public delegate void InforState( string msg);
	public InforState selfState;

	/// <summary>
	/// 完成回调
	/// </summary>
	public EventDelegate.Callback onFinish;

	/// <summary>
	/// 数据传递
	/// </summary>
	public object data;

	/// <summary>
	/// 执行进度
	/// </summary>
	public float progress = 0f;

	virtual public void OnStart()
	{

	}

    virtual public T GetData<T>()
	{
		return (T)data;
	}

	/// <summary>
	/// 内部执行状态通知给外部
	/// </summary>
	/// <param name="msg">Message.</param>
	protected void Infor( string msg)
	{
		if ( selfState != null)
		{
			selfState( msg);
		}
	}
}
