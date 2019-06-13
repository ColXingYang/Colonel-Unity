using UnityEngine;
using UnityEditor;
using System.IO;
public class NewAssetBundleEditor : Editor
{
    [MenuItem("Tools/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        //AssetBundleConfig.ASSETBUNDLE_PATH.Substring(AssetBundleConfig.PROJECT_PATH.Length) 获取的是AssetBundle存放的相对地址。
        Debug.Log("-->" + AssetBundleConfig.ASSETBUNDLE_PATH);
        Debug.Log("-->"+AssetBundleConfig.APPLICATION_PATH);
        Debug.Log("-->" + AssetBundleConfig.PROJECT_PATH);
        Debug.Log("-->" + AssetBundleConfig.PROJECT_PATH.Length);
        Debug.Log("-->" + AssetBundleConfig.ASSETBUNDLE_PATH.Substring(AssetBundleConfig.PROJECT_PATH.Length));
        BuildPipeline.BuildAssetBundles(AssetBundleConfig.ASSETBUNDLE_PATH.Substring(AssetBundleConfig.PROJECT_PATH.Length), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.WebGL);
        AssetDatabase.Refresh();
    }

    //对 PageItemModel 该文件夹中的所有PREFAB的bundlename 赋值
    [MenuItem("Tools/SetAssetBundleName")]
    static void SetResourcesAssetBundleName()
    {
        DirectoryInfo root = new DirectoryInfo(Application.dataPath + "/Resources/PageItemModel");
       
        SearchRoot(root, "");
        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();      
    }

    static void SearchRoot(DirectoryInfo root,string foldeIndex)
    {
        foreach (FileInfo itemFile in root.GetFiles())
        {
            if (itemFile.Extension.Contains(".prefab"))
            {
                string path = itemFile.FullName.Replace('\\', '/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
                Debug.Log("path=" + path);
                var importer = AssetImporter.GetAtPath(path);
                if (importer)
                {
                    //string name = path.Substring("Assets/Resources/PageItemModel/".Length);
                    string name = foldeIndex+"/"+itemFile.Name;
                    name = name.Substring(1, name.LastIndexOf('.')-1);
                    Debug.Log("name=" + (GetLower(name)+ AssetBundleConfig.SUFFIX));
                    importer.assetBundleName = GetLower(name) + AssetBundleConfig.SUFFIX;
                }
            }
        }

        foreach (DirectoryInfo subFolder in root.GetDirectories())
        {
            if (subFolder.Name.Equals("fbm") || subFolder.Name.Equals("Materials") || subFolder.Name.Equals("Texture"))
            {
                continue;
            }
            SearchRoot(subFolder, foldeIndex+"/"+subFolder.Name.Substring(2));
        }          
    }

    static string GetLower(string str)
    {
        char[] ch = str.ToCharArray();
        if (str != null)
        {
            for (int i = 0; i < ch.Length; i++)
            {
                if (!isChinese(ch[i]))
                {
                    if ((int)ch[i] > 64 && (int)ch[i] < 91)
                    {
                        ch[i] = System.Char.ToLower(ch[i]);
                    }
                }

            }
        }
        string lower = new string(ch);
        return lower;
    }

    static bool isChinese(char c)
    {
        return c >= 0x4E00 && c <= 0x9FA5;
    }


    [MenuItem("New AB Editor/GetAllAssetBundleName")]
    static void GetAllAssetBundleName()
    {

        string[] names = AssetDatabase.GetAllAssetBundleNames();

        foreach (var name in names)
        {
            Debug.Log(name);
        }

    }


    [MenuItem("New AB Editor/ClearAssetBundleName")]
    static void ClearResourcesAssetBundleName()
    {
        UnityEngine.Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.Assets | SelectionMode.ExcludePrefab);

        string[] Filtersuffix = new string[] { ".prefab", ".mat", ".jpg", ".png" };   //此处添加需要清除的资源后缀名,注意大小写。

        if (!(SelectedAsset.Length == 1)) return;

        string fullPath = AssetBundleConfig.PROJECT_PATH + AssetDatabase.GetAssetPath(SelectedAsset[0]);

        if (Directory.Exists(fullPath))
        {
            DirectoryInfo dir = new DirectoryInfo(fullPath);

            var files = dir.GetFiles("*", SearchOption.AllDirectories);
            ;
            for (var i = 0; i < files.Length; ++i)
            {
                var fileInfo = files[i];
                EditorUtility.DisplayProgressBar("清除AssetName名称", "正在清除AssetName名称中...", 1f * i / files.Length);
                foreach (string suffix in Filtersuffix)
                {
                    if (fileInfo.Name.EndsWith(suffix))
                    {
                        string path = fileInfo.FullName.Replace('\\', '/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
                        var importer = AssetImporter.GetAtPath(path);
                        if (importer)
                        {
                            importer.assetBundleName = null;

                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

}