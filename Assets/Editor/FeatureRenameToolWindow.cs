#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class FeatureRenameToolWindow : EditorWindow
{
    private string rootFolder = "Assets/Scripts/Features";
    private string oldBaseName = "";
    private string newBaseName = "";

    private bool moveFolder = true;
    private bool renameFileNames = true;
    private bool replaceInCs = true;
    private bool replaceInYaml = true;
    private bool replaceAcrossAssets = true;
    private bool dryRun = false;

    [MenuItem("Tools/Feature Rename Tool")]
    private static void OpenWindow()
    {
        GetWindow<FeatureRenameToolWindow>("Feature Rename Tool");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Feature Rename Tool (Folder / Class / Namespace)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        rootFolder = EditorGUILayout.TextField("Root Folder", rootFolder);
        oldBaseName = EditorGUILayout.TextField("Current Base Name", oldBaseName);
        newBaseName = EditorGUILayout.TextField("New Base Name", newBaseName);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
        moveFolder = EditorGUILayout.ToggleLeft("Move/Rename Feature Folder", moveFolder);
        renameFileNames = EditorGUILayout.ToggleLeft("Rename C# Filenames", renameFileNames);
        replaceInCs = EditorGUILayout.ToggleLeft("Replace In C# (.cs)", replaceInCs);
        replaceInYaml = EditorGUILayout.ToggleLeft("Replace In Unity YAML (.unity/.prefab/.asset)", replaceInYaml);
        replaceAcrossAssets = EditorGUILayout.ToggleLeft("Replace Across All Assets (not only target folder)", replaceAcrossAssets);
        dryRun = EditorGUILayout.ToggleLeft("Dry Run (log only)", dryRun);

        EditorGUILayout.Space();
        DrawPreview();

        EditorGUILayout.Space();
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Run Rename"))
            {
                RunRename();
            }
        }
    }

    private void DrawPreview()
    {
        var normalizedRoot = NormalizeFolder(rootFolder);
        var oldBase = NormalizePascal(oldBaseName);
        var newBase = NormalizePascal(newBaseName);
        if (string.IsNullOrEmpty(normalizedRoot) || string.IsNullOrEmpty(oldBase) || string.IsNullOrEmpty(newBase))
        {
            return;
        }

        var oldFolder = CombineAssetPath(normalizedRoot, oldBase);
        var newFolder = CombineAssetPath(normalizedRoot, newBase);
        var oldNamespace = ToNamespace(oldFolder);
        var newNamespace = ToNamespace(newFolder);

        EditorGUILayout.HelpBox(
            $"Old Folder: {oldFolder}\n" +
            $"New Folder: {newFolder}\n" +
            $"Old Namespace: {oldNamespace}\n" +
            $"New Namespace: {newNamespace}",
            MessageType.Info);
    }

    private void RunRename()
    {
        var normalizedRoot = NormalizeFolder(rootFolder);
        var oldBase = NormalizePascal(oldBaseName);
        var newBase = NormalizePascal(newBaseName);

        if (string.IsNullOrEmpty(normalizedRoot))
        {
            Debug.LogError("Root Folder is empty.");
            return;
        }
        if (string.IsNullOrEmpty(oldBase))
        {
            Debug.LogError("Current Base Name is empty.");
            return;
        }
        if (string.IsNullOrEmpty(newBase))
        {
            Debug.LogError("New Base Name is empty.");
            return;
        }
        if (oldBase == newBase)
        {
            Debug.LogError("Current and New Base Name are the same.");
            return;
        }

        var oldFolder = CombineAssetPath(normalizedRoot, oldBase);
        var newFolder = CombineAssetPath(normalizedRoot, newBase);
        var oldExists = AssetDatabase.IsValidFolder(oldFolder);
        var newExists = AssetDatabase.IsValidFolder(newFolder);
        var workingFolder = oldFolder;
        var shouldMoveFolder = moveFolder;

        if (!oldExists)
        {
            if (newExists)
            {
                workingFolder = newFolder;
                shouldMoveFolder = false;
                Debug.LogWarning($"Old folder not found. Using existing folder: {workingFolder}");
            }
            else
            {
                Debug.LogError($"Target folder does not exist: {oldFolder}");
                return;
            }
        }
        else
        {
            if (shouldMoveFolder && newExists)
            {
                Debug.LogError($"Destination folder already exists: {newFolder}");
                return;
            }
        }

        var folderLine = shouldMoveFolder
            ? $"Folder: {oldFolder} -> {newFolder}"
            : $"Folder: (skip) using {workingFolder}";

        var confirmMessage =
            $"Rename feature?\n\n" +
            $"Old: {oldBase}\n" +
            $"New: {newBase}\n\n" +
            $"{folderLine}\n" +
            $"Move Folder: {shouldMoveFolder}\n" +
            $"Rename Filenames: {renameFileNames}\n" +
            $"Replace In C#: {replaceInCs}\n" +
            $"Replace In YAML: {replaceInYaml}\n" +
            $"Replace Across Assets: {replaceAcrossAssets}\n" +
            $"Dry Run: {dryRun}";

        if (!EditorUtility.DisplayDialog("Feature Rename Tool", confirmMessage, "Run", "Cancel"))
        {
            return;
        }

        var errors = new List<string>();
        int renamedFiles = 0;
        int modifiedFiles = 0;

        try
        {
            if (shouldMoveFolder)
            {
                var moveError = MoveAssetSafely(oldFolder, newFolder, dryRun);
                if (!string.IsNullOrEmpty(moveError))
                {
                    errors.Add(moveError);
                    return;
                }
                if (!dryRun)
                    AssetDatabase.Refresh();
                workingFolder = newFolder;
            }

            if (renameFileNames)
            {
                renamedFiles = RenameFilesInFolder(workingFolder, oldBase, newBase, dryRun, errors);
            }

            if (replaceInCs || replaceInYaml)
            {
                modifiedFiles = ReplaceInFiles(workingFolder, oldBase, newBase, replaceInCs, replaceInYaml, replaceAcrossAssets, dryRun, errors);
            }
        }
        finally
        {
            if (!dryRun)
                AssetDatabase.Refresh();
        }

        if (errors.Count > 0)
        {
            foreach (var e in errors)
            {
                Debug.LogError(e);
            }
        }

        Debug.Log(
            $"Feature rename done. Renamed files: {renamedFiles}, Modified files: {modifiedFiles}, " +
            $"DryRun: {dryRun}");
    }

    private static string MoveAssetSafely(string oldAssetPath, string newAssetPath, bool dryRun)
    {
        if (dryRun)
        {
            Debug.Log($"[DryRun] Move: {oldAssetPath} -> {newAssetPath}");
            return "";
        }

        var error = AssetDatabase.MoveAsset(oldAssetPath, newAssetPath);
        if (!string.IsNullOrEmpty(error))
        {
            return $"MoveAsset failed: {error}";
        }

        Debug.Log($"Moved folder: {oldAssetPath} -> {newAssetPath}");
        return "";
    }

    private static int RenameFilesInFolder(string assetFolder, string oldBase, string newBase, bool dryRun, List<string> errors)
    {
        if (!AssetDatabase.IsValidFolder(assetFolder))
        {
            errors.Add($"Folder not found: {assetFolder}");
            return 0;
        }

        int count = 0;
        var guids = AssetDatabase.FindAssets("t:MonoScript", new[] { assetFolder });
        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var fileNameNoExt = Path.GetFileNameWithoutExtension(assetPath);
            if (fileNameNoExt.IndexOf(oldBase, StringComparison.Ordinal) < 0)
                continue;

            var newFileNameNoExt = fileNameNoExt.Replace(oldBase, newBase);
            if (newFileNameNoExt == fileNameNoExt)
                continue;

            if (dryRun)
            {
                Debug.Log($"[DryRun] Rename file: {assetPath} -> {newFileNameNoExt}.cs");
                count++;
                continue;
            }

            var error = AssetDatabase.RenameAsset(assetPath, newFileNameNoExt);
            if (!string.IsNullOrEmpty(error))
            {
                errors.Add($"Rename failed: {assetPath} -> {newFileNameNoExt}.cs : {error}");
                continue;
            }
            count++;
        }

        return count;
    }

    private static int ReplaceInFiles(
        string assetFolder,
        string oldBase,
        string newBase,
        bool includeCs,
        bool includeYaml,
        bool acrossAssets,
        bool dryRun,
        List<string> errors)
    {
        string searchRoot = acrossAssets ? "Assets" : assetFolder;
        var fullRoot = ToFullPath(searchRoot);
        if (!Directory.Exists(fullRoot))
        {
            errors.Add($"Search root not found: {searchRoot}");
            return 0;
        }

        var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (includeCs) extensions.Add(".cs");
        if (includeYaml)
        {
            extensions.Add(".unity");
            extensions.Add(".prefab");
            extensions.Add(".asset");
        }

        int modified = 0;
        var files = Directory.GetFiles(fullRoot, "*.*", SearchOption.AllDirectories)
            .Where(f => extensions.Contains(Path.GetExtension(f)));

        foreach (var file in files)
        {
            string text;
            try
            {
                text = File.ReadAllText(file);
            }
            catch (Exception e)
            {
                errors.Add($"Read failed: {file} : {e.Message}");
                continue;
            }

            if (text.IndexOf(oldBase, StringComparison.Ordinal) < 0)
                continue;

            var newText = text.Replace(oldBase, newBase);
            if (newText == text)
                continue;

            if (dryRun)
            {
                Debug.Log($"[DryRun] Replace in file: {ToAssetPath(file)}");
                modified++;
                continue;
            }

            try
            {
                File.WriteAllText(file, newText);
                modified++;
            }
            catch (Exception e)
            {
                errors.Add($"Write failed: {file} : {e.Message}");
            }
        }

        return modified;
    }

    private static string NormalizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
            return "";
        var normalized = folder.Trim().Replace("\\", "/");
        while (normalized.EndsWith("/", StringComparison.Ordinal))
            normalized = normalized.Substring(0, normalized.Length - 1);
        return normalized;
    }

    private static string NormalizePascal(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return "";
        raw = raw.Trim();
        return char.ToUpper(raw[0]) + raw.Substring(1);
    }

    private static string CombineAssetPath(string root, string leaf)
    {
        return $"{root}/{leaf}".Replace("\\", "/");
    }

    private static string ToNamespace(string assetFolderPath)
    {
        if (string.IsNullOrEmpty(assetFolderPath))
            return "DefaultNamespace";

        var path = assetFolderPath.Replace("\\", "/");
        var assetsPrefix = "Assets/";
        var index = path.IndexOf(assetsPrefix, StringComparison.Ordinal);
        if (index >= 0)
        {
            path = path.Substring(index + assetsPrefix.Length);
        }

        if (path.EndsWith("/", StringComparison.Ordinal))
        {
            path = path.Substring(0, path.Length - 1);
        }

        path = path.Replace("/", ".");
        return string.IsNullOrEmpty(path) ? "DefaultNamespace" : path;
    }

    private static string ToAssetPath(string fullPath)
    {
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? "";
        var normalized = fullPath.Replace("\\", "/");
        var rootNormalized = projectRoot.Replace("\\", "/").TrimEnd('/');
        if (normalized.StartsWith(rootNormalized, StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Substring(rootNormalized.Length + 1).Replace("\\", "/");
        }
        return normalized;
    }

    private static string ToFullPath(string assetPath)
    {
        var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? "";
        var normalized = assetPath.Replace("\\", "/").TrimStart('/');
        return Path.GetFullPath(Path.Combine(projectRoot, normalized));
    }
}
#endif
