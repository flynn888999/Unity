using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class MLTool : Editor {



    
    [MenuItem("Custom Editor/Create AssetBunldes ALL")]
    //将所有对象打包在一个Assetbundle中
    static void CreateAssetBunldesALL()
    {

        Caching.CleanCache();

        //string path = Application.dataPath + "/StreamingAssets/ALL.assetbundle";
        //if (!Directory.Exists(Path)) Directory.CreateDirectory(Path);
        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");



        if (path.Length != 0)
        {
            Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

            foreach (Object obj in SelectedAsset)
            {
                Debug.Log("Create AssetBunldes name :" + obj);
            }

            //这里注意第二个参数就行
            if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, path, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.Android))
            {
                AssetDatabase.Refresh();
            }
            else
            {

            }
        }
    }


    [MenuItem("Custom Editor/Create AssetBunldes Main")]
    //  单独打包
    static void CreateAssetBunldesMain()
    {
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);


        string path = EditorUtility.SaveFilePanel("Save Resource", "", "New Resource", "assetbundle");


        if (path.Length != 0)
        {
            //遍历所有的游戏对象
            foreach (Object obj in SelectedAsset)
            {
                //string sourcePath = AssetDatabase.GetAssetPath(obj);
                //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
                //StreamingAssets是只读路径，不能写入
                //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。

                //string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";

                if (BuildPipeline.BuildAssetBundle(obj, null, path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android))
                {
                    Debug.Log(obj.name + "资源打包成功");
                }
                else
                {
                    Debug.Log(obj.name + "资源打包失败");
                }
            }
            //刷新编辑器
            AssetDatabase.Refresh();
        }
       

    }


}
