using UnityEngine;
using System.Collections;

public class GameConfigResourcesAccess : GameConfigState {

	BetterList<string> downData;
	string currentDownName;

	public GameConfigResourcesAccess( BetterList<string> data)
	{
		downData = data;
	}

	public override void OnStart ()
	{
		MLWWWHelp.Instance.OnFinish = DownLoadFinish;
        GameGlobalCommunity.Instance.StartCoroutine(DownData());
	}


	private IEnumerator DownData()
	{
		if (downData == null || downData.size == 0) yield break;

		yield return null;

        NGUIDebug.Log("准备开始下载资源!");

		int length = downData.size;
		for (int i = 0; i < length; i++)
		{
			currentDownName = downData[i];
            string path = AssetLoad.Instance.GetHttpDownUrl(currentDownName);
            Infor("下载路径 " + path);

			//	进度
			progress = (float)i / length;
			Infor( progress + "");


			MLWWWHelp.Instance.DownLoadAssetBundle(path);
			while (currentDownName != null)
			{
				yield return null;
			}
		}

		if ( onFinish != null)
		{

            NGUIDebug.Log("资源全部下载完毕!");
			onFinish();
		}
	}

	private void DownLoadFinish( bool isFinish)
	{
		if ( isFinish)
		{
            GameGlobalCommunity.Instance.StartCoroutine(DisposeDownFinish());
		}
		else
		{
            Infor("[ff0000]GameConfigResourcesAccess ==> DownLoadFinish(). 下载出现错误![-]");
		}
	}

	/// <summary>
	/// 存储资源到本地
	/// </summary>
	/// <returns>The down finish.</returns>
	private IEnumerator DisposeDownFinish()
	{
		yield return null;
        NGUIDebug.Log(string.Format("{0} 资源下载完毕! 准备写入本地!", currentDownName));

        SaveLocal(MLWWWHelp.Instance.Binary, AssetLoad.Instance.GetLocalDownUrl(currentDownName));
		currentDownName = null;

        NGUIDebug.Log(string.Format("{0} 本地写入完毕! 准备下一个下载!", currentDownName));
	}

	private void SaveLocal(byte[] info, string name, string prefixPath)
	{
		MLFileHelp.CreateBinaryFile(AssetLoad.LocalAssetPath + "/" + prefixPath, name, info, info.Length);
	}

    private void SaveLocal(byte[] info, string path)
    {
        MLFileHelp.CreateBinaryFile(path, info, info.Length);
    }
}
