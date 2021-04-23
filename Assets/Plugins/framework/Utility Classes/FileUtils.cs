using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{

    public static class FileUtils
    {
        public static string SanitizeFilePath(string path)
        {
            return path.Replace('\\', '/').Trim('/');
        }

        public static string GetLocalPath(string absolouteAssetPath)
        {
            string path = SanitizeFilePath(absolouteAssetPath);
            return path.Substring(path.IndexOf("Assets/"));
        }

        public static string GetResourcePath(string absolouteAssetPath)
        {
            string path = SanitizeFilePath(absolouteAssetPath);

            path = path.Substring(path.IndexOf("Assets/Resources/") + 17);
            int dotIndex = path.LastIndexOf('.');

            return dotIndex >= 0 ? path.Substring(0, dotIndex) : path;
        }

        public static string GetAbsolutePath(string localAssetPath)
        {
            string path = SanitizeFilePath(localAssetPath);

            if (path.StartsWith("Assets/"))
            {
                path = path.Substring(path.IndexOf("/Assets/") + 8);
            }

            return Application.dataPath + "/" + path;
        }

#if UNITY_EDITOR


        public static T[] GetAssetsInFolder<T>(string folderPath, bool includeChildDirectories) where T : Object
        {
            List<T> assets = new List<T>();

            if (UnityEditor.AssetDatabase.IsValidFolder(folderPath))
            {
                string[] paths = Directory.GetFiles(Application.dataPath.Substring(0, Application.dataPath.Length - 6) + folderPath, "*", includeChildDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                for (var i = 0; i < paths.Length; i++)
                {
                    string path = paths[i].Substring(Application.dataPath.Length - 6);
                    T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }
            }

            return assets.ToArray();
        }

        public static T[] GetAllAssets<T>() where T : UnityEngine.Object
        {
            List<T> assets = new List<T>();
            string[] guids = UnityEditor.AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
            for (int i = 0; i < guids.Length; i++)
            {
                string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }
            return assets.ToArray();
        }

        public static void CreateAssetFolders(string fileOrFolderPath)
        {
            if (fileOrFolderPath == "Assets") return;

            fileOrFolderPath = SanitizeFilePath(fileOrFolderPath);

            if (fileOrFolderPath.Contains('.'))
            {
                fileOrFolderPath = fileOrFolderPath.Substring(0, fileOrFolderPath.LastIndexOf('/'));
            }

            int numFolders = fileOrFolderPath.CountOccurencesOf('/');
            string lastPath = "";

            for (int i = 0; i < numFolders; i++)
            {
                string path = fileOrFolderPath.Substring(0, fileOrFolderPath.NthIndexOf(i + 1, '/'));
                string folderName = path.Substring(path.LastIndexOf('/') + 1);

                if (i != 0 || folderName != "Assets")
                {
                    if (!UnityEditor.AssetDatabase.IsValidFolder(path))
                    {
                        UnityEditor.AssetDatabase.CreateFolder(lastPath, folderName);
                    }
                }

                lastPath = path;
            }

            if (!UnityEditor.AssetDatabase.IsValidFolder(fileOrFolderPath))
            {
                UnityEditor.AssetDatabase.CreateFolder(lastPath, fileOrFolderPath.Substring(fileOrFolderPath.LastIndexOf('/') + 1));
            }
        }
#endif
    }
}
