using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CXLUI
{
	public class CXLUIEditor : AssetPostprocessor {

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (string s in importedAssets) {
                if (s.Contains("CXLUIEditor"))	
				{
					AddTag("UI");
					AddLayer("2D UI");
					return;
				}
			}
		}
		
		public static void AddTag(string tag)
		{
			if ( !IsHasTag(tag))
			{
				SerializedObject tagManager = new SerializedObject( AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
				SerializedProperty it = tagManager.GetIterator();
				while(it.NextVisible(true))
				{
					if ( it.name.Equals("tags"))
					{
						for (int i = 0; i < it.arraySize; i++) 
                        {
							SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);
							dataPoint.stringValue = tag;
							tagManager.ApplyModifiedProperties();
							return;
						}
					}
				}
			}
		}
		
		public static void AddLayer( string layer)
		{
			if(!IsHasLayer(layer))
			{
				SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
				SerializedProperty it = tagManager.GetIterator();
				while (it.NextVisible(true))
				{
					if(it.name.StartsWith("User Layer"))
					{
						if(it.type == "string" )
						{
							if(string.IsNullOrEmpty(it.stringValue)){
								it.stringValue  = layer;
								tagManager.ApplyModifiedProperties();
								return;
							}
						}
					}
				}
			}
		}
		
		private static bool IsHasTag( string tag)
		{
			int length = UnityEditorInternal.InternalEditorUtility.tags.Length;
			for (int i = 0; i < length; i++) {
				if ( UnityEditorInternal.InternalEditorUtility.tags[i].Contains(tag))
					return true;
			}
			return false;
		}
		
		private static bool IsHasLayer( string layer)
		{
			int length = UnityEditorInternal.InternalEditorUtility.layers.Length;
			for (int i = 0; i < length; i++) {
				if ( UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
					return true;
			}
			return false;
		}
	}
}