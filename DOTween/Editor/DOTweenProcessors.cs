// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/07/16 18:07
// License Copyright (c) Daniele Giardini
// This work is subject to the terms at http://dotween.demigiant.com/license.php

using System.IO;
using DG.DOTweenEditor.UI;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEditor;
using UnityEngine;

namespace DG.DOTweenEditor
{
    public class UtilityWindowModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        // Checks if deleted folder contains DOTween Pro and in case removes scripting define symbols
        static AssetDeleteResult OnWillDeleteAsset(string asset, RemoveAssetOptions options)
        {
            // Check if asset is a directory
            string dir = EditorUtils.ADBPathToFullPath(asset);
            if (!Directory.Exists(dir)) return AssetDeleteResult.DidNotDelete;
            // Check if directory contains DOTween.dll
            string[] files = Directory.GetFiles(dir, "DOTween.dll", SearchOption.AllDirectories);
            int len = files.Length;
            bool containsDOTween = false;
            for (int i = 0; i < len; ++i) {
                if (!files[i].EndsWith("DOTween.dll")) continue;
                containsDOTween = true;
                break;
            }
            if (!containsDOTween) return AssetDeleteResult.DidNotDelete;
            Debug.Log("::: DOTween deleted");
            // DOTween is being deleted: deal with it
            // Remove EditorPrefs
            EditorPrefs.DeleteKey(Application.dataPath + DOTweenUtilityWindow.Id);
            EditorPrefs.DeleteKey(Application.dataPath + DOTweenUtilityWindow.IdPro);

//            // The following is not necessary anymore since the Modules update
//            // Remove scripting define symbols
//            DOTweenDefines.RemoveAllDefines();
//            //
//            EditorUtility.DisplayDialog("DOTween Deleted",
//                "DOTween was deleted and all of its scripting define symbols removed." +
//                "\n\nThis might show an error depending on your previous setup." +
//                " If this happens, please close and reopen Unity or reimport DOTween.",
//                "Ok"
//            );
            return AssetDeleteResult.DidNotDelete;
        }


    }
}