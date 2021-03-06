﻿using UnityEngine;
using System.Collections;

namespace CXLUI
{
	public class UITool  {
		
		public static void HideTransfrom( Transform trans)
		{
			trans.localPosition = new Vector3( 10000, 10000 , 0f);
		}
		
		public static void ShowTransfrom( Transform trans)
		{
			trans.localPosition = Vector3.zero;
		}
		
		public static int SetStructOrderDepth( GameObject obj, int depth)
		{
			UIPanel[] panels = obj.GetComponents<UIPanel>();
			
			if ( panels == null || panels.Length == 0) 
				throw new System.Exception( "UIDialogManager - SetStructOrderDepth Dialog there is no panel");
			
			foreach (UIPanel item in panels) 
			{
				item.depth = depth++;
			}
			return depth;
		}
		
        public static void AddChild( GameObject parent, GameObject child)
        {
            if (child != null && parent != null)
            {
                Transform t = child.transform;
			    t.parent = parent.transform;
			    t.localPosition = Vector3.zero;
			    t.localRotation = Quaternion.identity;
			    t.localScale = Vector3.one;
			    child.layer = parent.layer;
            }
        }

        /// <summary>
        /// 所有UI加载预设入口
        /// </summary>
        /// <param name="uiName">   实例化后名称
        /// Resources   状态下     表示预设名称和实例化后名称
        /// Asset       状态下     实例化后名称</param>
        /// <param name="info">    信息 
        /// Resources   状态下 assetName表示前路径
        /// Asset       状态下 assetName表示资源映射Key,resName表示AssetBundle里的资源名称</param>
        /// <param name="parent"></param>
        /// <returns></returns>
		public static GameObject Load(string uiName, /*AssetResourecesInfo info,*/ GameObject parent = null)
		{
            GameObject child = null;

            if (GameGlobalCommunity.Instance.IsAssetType)
            {
                if (AssetPath.AssetUIPath.ContainsKey(uiName))
                {
                    AssetResourecesInfo info = AssetPath.AssetUIPath[uiName];
                    if (AssetLoad.IsCache(info.assetName))
                    {
                        child = GameObject.Instantiate(AssetLoad.Load<GameObject>(info)) as GameObject;
                        AddChild(parent, child);
                        child.name = uiName;
                        //return child;
                    }
                    else
                    {
                        USERDebug.LogError(string.Format("{0}({1}) 资源不在缓存中!", info.assetName, uiName));
                    }
                }
                else
                {
                    USERDebug.LogError(string.Format("{0} 资源不在AssetPath.AssetUIPath中!", uiName));
                }
            }
            else
            {
                if (AssetPath.ResourecesUIPath.ContainsKey(uiName))
                {
                    AssetResourecesInfo info = AssetPath.ResourecesUIPath[uiName];

                    string path = info.assetName + uiName;
                    child = NGUITools.AddChild(parent, Resources.Load<GameObject>(path));
                    child.name = uiName;
                }
                else
                {
                    USERDebug.LogError(string.Format("{0} 资源不在AssetPath.ResourecesUIPath中!", uiName));
                }
            }
            return child;

            //AssetResourecesInfo info = AssetPath.UIPath[uiName];

            //if (GameGlobalCommunity.Instance.IsAssetType)
            //{
            //    //  在缓存中
            //    if (AssetLoad.IsCache(info.assetName))
            //    {
            //        child = GameObject.Instantiate(AssetLoad.Load<GameObject>(info)) as GameObject;
            //        AddChild(parent, child);
            //        child.name = uiName;
            //    }
            //    else
            //    {
            //        USERDebug.LogError("目标资源部在缓存中!");           
            //    }
            //}
            //else
            //{
            //    string path = info.assetName + uiName;
            //    child = NGUITools.AddChild(parent, Resources.Load<GameObject>(path));
            //    child.name = uiName;
            //}
            //return child;
		}
		
	
	}
}
