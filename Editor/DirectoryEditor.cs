using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class DirectoryEditor : Editor {

    /// <summary>
    /// 将文件夹里面的文件放到文件夹外面
    /// </summary>
   // [MenuItem("Tools/AAAA")]
    static void AAA()
    {
        return;
        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets | SelectionMode.ExcludePrefab);
        if (!(SelectedAsset.Length == 1)) return;
        string fullPath = AssetBundleConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
        Debug.Log("Click fullPath=" + fullPath);
        DirectoryInfo root = new DirectoryInfo(fullPath);
        Search( root, 0);

        AssetDatabase.Refresh();
    }

    private static void Search( DirectoryInfo root, int level)
    {
        level++;
        foreach (DirectoryInfo subFolder in root.GetDirectories())
        {
            if (subFolder.Name.Equals("fbm") || subFolder.Name.Equals("Materials") || subFolder.Name.Equals("Texture"))
            {
                continue;
            }
        //    subFolder.Name = level.ToString("%2d") +"_"+ subFolder.Name;

            Search(subFolder, level);
        }
    }

    /*
    /// <summary>
    /// 文件夹内的文件的名字都改成 文件夹的名字
    /// </summary>
    [MenuItem("Tools/ChangeName")]
    static void ChangeName0()
    {

        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets | SelectionMode.ExcludePrefab);
        if (!(SelectedAsset.Length == 1)) return;
        //   Debug.Log("AssetBundleConfig.PROJECT_PATH=" + AssetBundleConfig.PROJECT_PATH);
        //   Debug.Log("AssetDatabase.GetAssetPath(SelectedAsset[0])=" + AssetDatabase.GetAssetPath(SelectedAsset[0]));
        string fullPath = AssetBundleConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);
        Debug.Log("Click fullPath=" + fullPath);



        //  DirectoryInfo root = new DirectoryInfo(Application.dataPath + "/Resources/PageItemModel");
        DirectoryInfo root = new DirectoryInfo(fullPath);
        DirectoryInfo textureDir = new DirectoryInfo("C:/Users/007/Desktop/AssetBundleProject1014/Assets/Resources/PageItemModel/Texture");
        List<FileInfo> textures = new List<FileInfo>();
        foreach (FileInfo item in textureDir.GetFiles())
        {
            textures.Add(item);
            // Debug.Log(item.Name);
        }
        Debug.Log("Total=" + textures.Count);
        SearchRoot(root, textures);
    }

    static void SearchRoot(DirectoryInfo root, List<FileInfo> textures)
    {
        bool flag = false;
        //遍历元素文件夹下文件(示意图,模型prefab)
        //是否 是资源文件夹
        foreach (FileInfo itemFile in root.GetFiles())
        {
            if (itemFile.Extension.Contains("jpg") || itemFile.Extension.Contains("png")
                || itemFile.Extension.Contains("JPG") || itemFile.Extension.Contains("PNG"))
            {
                flag = true;
            }
        }
        if (flag)
        {
            ChangeName(root, textures);
            return;
        }

        foreach (DirectoryInfo subFolder in root.GetDirectories())
        {
            if (subFolder.Name.Equals("fbm") || subFolder.Name.Equals("Materials") || subFolder.Name.Equals("Texture"))
            {
                continue;
            }
            SearchRoot(subFolder, textures);
        }
    }

    static void ChangeName(DirectoryInfo directory, List<FileInfo> textures)
    {

        foreach (FileInfo item in directory.GetFiles())
        {
            Debug.Log("itemFile.Name=" + item.Name);
            for (int i = 0; i < textures.Count; i++)
            {
                if (item.Name.Equals(textures[i].Name))
                {
                    Debug.Log("Texture-itemFile.Name=" + item.Name);
                    FileInfo fileInfo = new FileInfo("C:/Users/007/Desktop/AssetBundleProject1014/Assets/Resources/PageItemModel/Texture/" + item.Name);
                    if (fileInfo.Extension.Equals(".meta"))
                    {
                        string str = fileInfo.Name.Replace(fileInfo.Extension, "");
                        string name = str.Substring(str.LastIndexOf('.') + 1);
                        fileInfo.MoveTo("C:/Users/007/Desktop/AssetBundleProject1014/Assets/Resources/PageItemModel/Texture/" + directory.Name + "." + name + fileInfo.Extension);
                    }
                    else
                    {
                        fileInfo.MoveTo("C:/Users/007/Desktop/AssetBundleProject1014/Assets/Resources/PageItemModel/Texture/" + directory.Name + fileInfo.Extension);
                    }
                }
            }

            try
            {
                if (item.Extension.Equals(".meta"))
                {
                    string str = item.Name.Replace(item.Extension, "");
                    string name = str.Substring(str.LastIndexOf('.') + 1);
                    item.MoveTo(directory.FullName + "\\" + directory.Name + "." + name + item.Extension);
                }
                else
                {
                    item.MoveTo(directory.FullName + "\\" + directory.Name + item.Extension);
                }

            }
            catch (System.Exception e)
            {


                if (!item.Extension.Equals(".meta"))
                {
                    Debug.LogError("!!!!@@@@@@");
                    throw;
                }
                else
                {
                    Debug.LogError(e);
                    Debug.LogError(directory.FullName + "\\" + directory.Name + item.Extension);
                }
            }
            finally
            {
                Debug.Log("finally");
            }

        }
        // FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        AssetDatabase.Refresh();
    }
    */

}
