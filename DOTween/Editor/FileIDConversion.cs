using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class FileIDConversion 
{

    private static readonly (string, int)[] originalIds = 
    {
        ("a811bde74b26b53498b4f6d872b09b6d", 16995157),
        ("aa0b1eebb5db27a419fa4564bbe5c9c5", 40306178),
    };
    
    [MenuItem("Tools/Demigiant/ConvertFileIDs")]
    public static async void Convert() 
    {
        List<Task> tasks = new List<Task>();
        string[] guids = AssetDatabase.FindAssets("t:Object");
        HashSet<string> handledIds = new HashSet<string>();

        for (int i = 0; i < guids.Length; i++) 
        {
            string guid = guids[i];
            if (handledIds.Add(guid)) 
            {
                tasks.Add(ReplaceFileId(guid));
            }
        }

        await Task.WhenAll(tasks);
        AssetDatabase.Refresh();
    }
    
    private static async Task ReplaceFileId(string assetGuid) 
    {
        string path = AssetDatabase.GUIDToAssetPath(assetGuid);
        if (!path.EndsWith(".asset") && !path.EndsWith(".prefab") && !path.EndsWith(".unity"))
            return;

        Type assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

        // Filter out file types we are not interested in
        if (assetType == typeof(DefaultAsset) || assetType == typeof(MonoScript) || assetType == typeof(Texture2D) || assetType == typeof(UnityEditor.Animations.AnimatorController) ||
            assetType == typeof(TextAsset) || assetType == typeof(Shader) || assetType == typeof(Font) || assetType == typeof(UnityEditorInternal.AssemblyDefinitionAsset) ||
            assetType == typeof(GUISkin) || assetType == typeof(PhysicsMaterial2D) || assetType == typeof(UnityEngine.U2D.SpriteAtlas) || assetType == typeof(UnityEngine.Tilemaps.Tile) ||
            assetType == typeof(AudioClip) || assetType == typeof(ComputeShader) || assetType == typeof(Material))
            return;
            
        try 
        {
            using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None)) 
            {
                byte[] buffer = new byte[stream.Length];
                
                await stream.ReadAsync(buffer, 0, buffer.Length);
                string content = System.Text.Encoding.ASCII.GetString(buffer);

                bool changed = false;
                foreach(var (guid, fileId) in originalIds)
                {
                    string newContent = content.Replace($"fileID: {fileId}, guid: {guid}", $"fileID: 11500000, guid: {guid}");
                    if (newContent != content) 
                    {
                        changed = true;
                        content = newContent;
                    }
                }
                
                if (changed) 
                {
                    stream.Position = 0;
                    buffer = System.Text.Encoding.ASCII.GetBytes(content);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
        } 
        catch (UnauthorizedAccessException) 
        {
            
        }
        catch (Exception ex) 
        {
            Debug.LogException(ex);
        }
    }
}
