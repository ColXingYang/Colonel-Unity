using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

public class PageItemConfEditor : Editor {

    private const string ModelItemsDirPath = "Assets\\TzVR\\Assets\\Resources\\PageItemModel";
    [MenuItem("Tools/Build PageItemConfTXT")]
    static void initAllPageItemsFromDir()
    {
        DirectoryInfo root = new DirectoryInfo(Application.dataPath+ "/Resources/PageItemModel");
        FileStream fs = new FileStream(Application.dataPath + "/Resources/Text/pageItemConf.txt", FileMode.Create);
        Debug.Log("-->" + Application.dataPath + "/Resources/Text/pageItemConf.txt");

        StreamWriter sw = new StreamWriter(fs);
        ////获得字节数组
        //byte[] data = System.Text.Encoding.Default.GetBytes("Hello World!");
        Search(sw, root, 0);
       
        sw.Close();
        fs.Close();
        AssetDatabase.Refresh();
    }

    private static void Search(StreamWriter sw,DirectoryInfo root, int level) {

        level++;
        bool flag = false;
        //遍历元素文件夹下文件(示意图,模型prefab)
        //是否 是资源文件夹
        foreach (FileInfo itemFile in root.GetFiles())
        {
            if (itemFile.Extension.Contains("jpg") || itemFile.Extension.Contains("png") 
                || itemFile.Extension.Contains("JPG") || itemFile.Extension.Contains("PNG")
                || itemFile.Extension.Contains("prefab")) {
                flag = true;
            }           
        }
        if (flag)
        {
            Dictionary<string, List<string>> fileNameDic = new Dictionary<string, List<string>>();
            foreach (FileInfo itemFile in root.GetFiles())
            {
                if (itemFile.Extension.Contains(".meta"))
                {
                    continue;
                }

                if (!fileNameDic.ContainsKey(itemFile.Name.Replace(itemFile.Extension, "")))
                    fileNameDic.Add(itemFile.Name.Replace(itemFile.Extension, ""), new List<string>());

                fileNameDic[itemFile.Name.Replace(itemFile.Extension, "")].Add(itemFile.Extension);
            }

            foreach (string item in fileNameDic.Keys)
            {
                for (int i = 0; i < level; i++)
                {
                    sw.Write('x');
                }
                sw.Write(":" + item);//名字

                //类型
                WriteType(sw, root);

                foreach (string extension in fileNameDic[item])
                {
                    sw.Write(":" + item+extension);
                }

                sw.Write("\r\n");
            }         
            return;
        }

        foreach (DirectoryInfo subFolder in root.GetDirectories())
        {
            if (subFolder.Name.Equals("fbm")|| subFolder.Name.Equals("Materials") ||subFolder.Name.Equals("Texture"))
            {
                continue;
            }
            for (int i = 0; i < level; i++)
            {
                sw.Write('x');                
            }
            sw.Write(":" + subFolder.Name.Substring(2)+"\r\n");

            Search(sw, subFolder, level); 
        }
    }

    private static void WriteType(StreamWriter sw, DirectoryInfo root)
    {
        if (root.Name.Contains("地面材质"))
        {
            sw.Write(":BuildingElem_DiMianCaiZhi");
        }
        else if (root.Name.Contains("墙面材质"))
        {
            sw.Write(":BuildingElem_QianMianCaiZhi");
        }
        else if (root.Name.Contains("安全线"))
        {
            sw.Write(":BuildingElem_AnQuanXian");
        }
        else if (root.Name.Contains("屋顶材质"))
        {
            sw.Write(":BuildingElem_WuDingCaiZhi");
        }
        else if (root.Name.Contains("画线"))
        {
            sw.Write(":BuildingElem_HuaXian");
        }
        else if (root.Name.Contains("门窗创建"))
        {
            sw.Write(":BuildingElem_MenChuangChuangJian");
        }
        else if (root.Name.Contains("用户上传"))
        {
            sw.Write(":UserUpload");
        }
        else if (root.FullName.Contains("临时实体对象") && root.FullName.Contains("基本类临时实体"))
        {
            sw.Write(":BasicTempEntity");
        }
        else if (root.FullName.Contains("临时实体对象") && root.FullName.Contains("容器类临时实体"))
        {
            sw.Write(":ContainerTempEntity");
        }
        else if (root.Name.Contains("仓库")|| root.Name.Contains("护栏") || root.Name.Contains("机柜电箱") || root.Name.Contains("消防"))
        {
            sw.Write(":Facility");
        }
        else
        {
            sw.Write(":Other");
        }
    }

           
    /*
           
                var fileInfo = files[i];
                EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                foreach (string suffix in Filtersuffix)
                {
                    if (fileInfo.Name.EndsWith(suffix))
                    {
                        string path = fileInfo.FullName.Replace('\\', '/').Substring(AssetBundleConfig.PROJECT_PATH.Length);
                        Debug.Log(path);
                        var importer = AssetImporter.GetAtPath(path);
                        if (importer)
                        {
                            string name = path.Substring(fullPath.Substring(AssetBundleConfig.PROJECT_PATH.Length).Length + 1);
                            Debug.Log("name=" + name);
                            importer.assetBundleName = name.Substring(0, name.LastIndexOf('.')) + AssetBundleConfig.SUFFIX;
                        }
                    }
                }
            }
            AssetDatabase.RemoveUnusedAssetBundleNames();
        }
        EditorUtility.ClearProgressBar();
    }
    */
}
