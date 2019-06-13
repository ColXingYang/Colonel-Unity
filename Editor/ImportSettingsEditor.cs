using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportSettingsEditor : Editor {

    
    /// <summary>
    /// 将所选目录中的的图片设置为特定的格式；
    /// </summary>
   // [MenuItem("Tools/ChangeImportSettings")]
    static void ChangeImportSettings()
    {
        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets | SelectionMode.ExcludePrefab);
        //string[] Filtersuffix = new string[] { ".prefab", ".jpg", ".png" };   //此处添加需要命名的资源后缀名,注意大小写。
        if (!(SelectedAsset.Length == 1)) return;
        string fullPath = AssetBundleConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);
            foreach (FileInfo itemFile in dir.GetFiles())
            {
                if (itemFile.Extension.Equals(".jpg") || itemFile.Extension.Equals(".png"))
                {
                    //   Debug.Log(itemFile.FullName);
                    Debug.Log(itemFile.FullName.Substring(AssetBundleConfig.PROJECT_PATH.Length).Replace("\\", "/"));
                    TextureImporter textureImporter = AssetImporter.GetAtPath(itemFile.FullName.Substring(AssetBundleConfig.PROJECT_PATH.Length).Replace("\\", "/")) as TextureImporter;
                    textureImporter.textureType = TextureImporterType.GUI;
                    textureImporter.maxTextureSize = 64;
                    AssetDatabase.ImportAsset(itemFile.FullName.Substring(AssetBundleConfig.PROJECT_PATH.Length).Replace("\\", "/"));
                }
            }
        }
        else
        {
            Debug.LogError(fullPath);
        }

    }
    

}
