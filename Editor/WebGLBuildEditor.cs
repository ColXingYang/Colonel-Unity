#if UNITY_WEBGL
using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using System.Runtime.InteropServices;

public class WebGLBuildEditor : EditorWindow
{
    enum PlatformType
    {
        Online,
        Test,
    }
    
    private static PlatformType platformType = PlatformType.Test;
    private static string _processorDirectivesTemp = null;

    [MenuItem(GlobalSettings.MenuName+"/WebGLBuildEditor")]
    private static void Init() {
		var window = GetWindow<WebGLBuildEditor>("WebGLBuildEditor", true);
		window.position = new Rect(200, 200, 600, 600);
		window.Show();
	}

    private void OnGUI()
    {
        int i = 0;
        platformType = (PlatformType)EditorGUI.EnumPopup(new Rect(0,  i += 30, position.width, 16), "平台类型：", platformType);        

        if (GUI.Button(new Rect(3, i += 30, position.width - 6, 24), "开始打包"))
        {           
            this.Close();
            Build();
        }

        if (GUI.Button(new Rect(3, 300, position.width - 6, 24), "Close"))
        {
            this.Close();
        }
    }

    private static void Build()
    {
        if (platformType == PlatformType.Online)
        {
            _processorDirectivesTemp = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, _processorDirectivesTemp + ";Online");
        }

        BulidTarget();
    }

    static void BulidTarget()
    {
        string applicationPath = Application.dataPath.Replace("/Assets", "");
        string target_dir = applicationPath + "/0830";
        
        if (!Directory.Exists(target_dir))
        {
            Directory.CreateDirectory(target_dir);
        }
        
        GenericBuild(FindEnabledEditorScenes(), target_dir, BuildTarget.WebGL, BuildOptions.None);
    }   

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void GenericBuild(string[] scenes, string target_dir, BuildTarget build_target, BuildOptions build_options)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        BuildReport buildReport = BuildPipeline.BuildPlayer(scenes, target_dir, build_target, build_options);
        if (buildReport.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("打包成功！-->"+platformType+"\n Size=" + buildReport.summary.totalSize + " bytes");
            if (platformType == PlatformType.Online)
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, _processorDirectivesTemp);

            FileDialog.OpenFileDialog(Application.dataPath.Replace("Assets", "").Replace("/", "\\") + "0830");
            
        }
        else if (buildReport.summary.result == BuildResult.Failed)
        {
            Debug.Log("打包失败！");
        }
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    static void CopyAndroidLib(string name)
    {
        string path = Application.dataPath.Replace("/Assets", "");
        DeleteFolder(Application.dataPath + "/Plgns");
    }

    public static void DeleteFolder(string dir)
    {
        if (!Directory.Exists(dir)) return;
        foreach (string d in Directory.GetFileSystemEntries(dir))
        {
            if (File.Exists(d))
            {
                FileInfo fi = new FileInfo(d);
                if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    fi.Attributes = FileAttributes.Normal;
                File.Delete(d);
            }
            else
            {
                DirectoryInfo d1 = new DirectoryInfo(d);
                if (d1.GetFiles().Length != 0)
                {
                    DeleteFolder(d1.FullName);////递归删除子文件夹
                }
                Directory.Delete(d,true);
            }
        }
    }

    public static void CopyDirectory(string sourcePath, string destinationPath)
    {
        DirectoryInfo info = new DirectoryInfo(sourcePath);
        if (Directory.Exists(destinationPath))
            Directory.CreateDirectory(destinationPath);
        foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
        {
            string destName = Path.Combine(destinationPath, fsi.Name);
            if (fsi is System.IO.FileInfo)
                File.Copy(fsi.FullName, destName,true);
            else
            {
                Directory.CreateDirectory(destName);
                CopyDirectory(fsi.FullName, destName);
            }
        }
    }

    /// <summary>
    /// 文件日志类
    /// </summary>
    // [特性(布局种类.有序,字符集=字符集.自动)]
	//参考资料：https://www.jianshu.com/p/5a930906df03?utm_campaign
	//          https://www.jianshu.com/p/8cc34b2a7377
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class FileDialog
    {
        public int structSize = 0;
        public IntPtr dlgOwner = IntPtr.Zero;
        public IntPtr instance = IntPtr.Zero;
        public String filter = null;
        public String customFilter = null;
        public int maxCustFilter = 0;
        public int filterIndex = 0;
        public String file = null;
        public int maxFile = 0;
        public String fileTitle = null;
        public int maxFileTitle = 0;
        public String initialDir = null;
        public String title = null;
        public int flags = 0;
        public short fileOffset = 0;
        public short fileExtension = 0;
        public String defExt = null;
        public IntPtr custData = IntPtr.Zero;
        public IntPtr hook = IntPtr.Zero;
        public String templateName = null;
        public IntPtr reservedPtr = IntPtr.Zero;
        public int reservedInt = 0;
        public int flagsEx = 0;

        #region Win32API WRAP
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetOpenFileName([In, Out]FileDialog dialog);  //这个方法名称必须为GetOpenFileName

        [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern bool GetSaveFileName([In, Out]FileDialog dialog);  //这个方法名称必须为GetSaveFileName


        public static void OpenFileDialog(string dir)
        {
            try
            {
                FileDialog pth = new FileDialog();
                pth.structSize = Marshal.SizeOf(pth);
                pth.filter = "All files (*.*)|*.*";
                pth.file = new string(new char[256]);
                pth.maxFile = pth.file.Length;
                pth.fileTitle = new string(new char[64]);
                pth.maxFileTitle = pth.fileTitle.Length;
                pth.initialDir = dir; //默认路径
                pth.title = "打开文件夹";
                pth.defExt = ".html";
                pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
                pth.dlgOwner = FileDialog.GetForegroundWindow();//这一步将文件选择窗口置顶。
                FileDialog.GetOpenFileName(pth);
            }
            catch (Exception e)
            {
                Debug.Log("e=" + e.ToString());

                throw;
            }
        }

        /// <summary>
        /// 保存文件选择窗口
        /// </summary>
        public static string SaveDialog(string dir)
        {
            FileDialog ofn = new FileDialog();
            ofn.structSize = Marshal.SizeOf(ofn);
            ofn.filter = "All files (*.*)|*.*";
            ofn.file = new string(new char[256]); ;
            ofn.maxFile = ofn.file.Length;
            ofn.fileTitle = new string(new char[64]);
            ofn.maxFileTitle = ofn.fileTitle.Length;
            ofn.initialDir = dir;
            ofn.title = "保存";
            ofn.flags = 0x00000002 | 0x00000004; // OFN_OVERWRITEPROMPT | OFN_HIDEREADONLY;
            ofn.dlgOwner = GetForegroundWindow(); //这一步将文件选择窗口置顶。
            if (!GetSaveFileName(ofn))
            {
                return null;
            }
            return ofn.file;
        }
    }

    
    #endregion
}
#endif
