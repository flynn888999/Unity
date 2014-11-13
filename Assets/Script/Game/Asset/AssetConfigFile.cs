using UnityEngine;
using System.Collections;


/**
 * 
 *      资源文件数据类
 * 
 * **/

public class AssetConfigFile : AssetConfigFolder
{
    private string md5;
    public string Md5
    {
        get { return md5; }
        set { md5 = value; }
    }


    public AssetConfigFile(string filename, int p, string md5)
        : base(filename, p)
    {
        this.md5 = md5;
        this.isFolder = false;
    }

    /// <summary>
    /// 获取局部路径
    /// </summary>
    /// <returns></returns>
    public override string FilePath()
    {
        if (parentIndex == -1)
        {
            return /*AssetLoad.LocalAssetPath + "/" + */ FileName;
        }
        else
            return base.FilePath();
    }
}