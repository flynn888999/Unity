using UnityEngine;
using System.Collections;

/**
 * 
 *      资源目录类
 * 
 * **/
public class AssetConfigFolder
{
    public AssetConfigFolder(string filename)
    {
        this.fileName = filename;
        this.isFolder = true;
    }

    public AssetConfigFolder(string filename, int p)
    {
        this.fileName = filename;
        this.parentIndex = p;
        this.isFolder = true;
    }

    private string path;

    protected string fileName = "";

    protected int parentIndex = -1;

    protected bool isFolder = true;

    #region Attribute
    public int ParentIndex
    {
        get { return parentIndex; }
    }

    public string FileName
    {
        get { return fileName; }
    }

    public bool IsFolder
    {
        get { return isFolder; }
    }

    #endregion


    virtual public string FilePath()
    {
        if (string.IsNullOrEmpty(path))
            path = GetFullFile(this);
        return path;
    }

    private string GetFullFile(AssetConfigFolder cf)
    {
        string file = "";
        return GetFile(this, file);
    }

    /// <summary>
    /// 获取局部路径名称
    /// </summary>
    /// <param name="cf"></param>
    /// <param name="fullFileName"></param>
    /// <returns></returns>
    private string GetFile(AssetConfigFolder cf, string fullFileName)
    {
        if (cf.parentIndex != -1)
            fullFileName = GetFile(AssetLoad.Instance.ResourcesList[cf.parentIndex], fullFileName) + "/" + cf.fileName;
        else
            fullFileName = /*AssetLoad.LocalAssetPath + "/" +*/ cf.fileName;

        return fullFileName;
    }
}