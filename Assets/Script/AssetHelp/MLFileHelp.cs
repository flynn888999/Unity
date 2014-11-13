using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;



/**
 * 
 *      IO操作帮助
 * 
 * **/
public class MLFileHelp : MonoBehaviour {

    /// <summary>
    /// 计算字节的MD5值
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string GetMD5HashFromBytes( byte[] bytes)
    {
        try
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();

            int Length = retVal.Length;
            for (int i = 0; i < Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("GetMD5HashFromBytes() fail!,error. " + ex.Message);
        }
    }


    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    /// <param name="fileName">文件路径</param>
    /// <returns></returns>
    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            int length = retVal.Length;
            for (int i = 0; i < length; i++)
            {
                sb.Append(retVal[i].ToString("X2"));
            }

            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception(" GetMD5HashFromFile fail! error. " + ex.Message);
        }
        
    }



    /// <summary>
    /// 是否拥有此目录,没有则创建目录
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static void NoDirectoryCreate(string path)
    { 
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }



    /// <summary>
    /// 写入二进制文件
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="fileName">文件名称</param>
    /// <param name="info">写入的数据</param>
    /// <param name="length">长度</param>
    public static void CreateBinaryFile(string path, string fileName, byte[] info, int length)
    {
        CreateBinaryFile(path + "/" + fileName, info, length);
    }


    public static void CreateBinaryFile(string path, byte[] info, int length)
    {
        Stream sw;
        FileInfo file = new FileInfo(path);


        //  没有创建目录
        //if (!Directory.Exists(path))
        //{
        //    Directory.CreateDirectory(path);
        //}

        //  打开文件
        if (!file.Exists)
        {
            sw = file.Create();
        }
        else
        {
            sw = file.OpenWrite();
        }

        sw.Write(info, 0, length);
        sw.Close();
        sw.Dispose();
    }
}
