// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/08 11:50
// License Copyright (c) Daniele Giardini
// This work is subject to the terms at http://dotween.demigiant.com/license.php
// using MessagePack;

namespace DG.Tweening.Plugins.Options
{
    /// <summary>
    /// Base interface for all tween plugins options
    /// </summary>
    // [Union(0, typeof(ColorOptions))]
    // [Union(1, typeof(FloatOptions))]
    // [Union(2, typeof(NoOptions))]
    // [Union(3, typeof(PathOptions))]
    // [Union(4, typeof(QuaternionOptions))]
    // [Union(5, typeof(RectOptions))]
    // [Union(6, typeof(StringOptions))]
    // [Union(7, typeof(UintOptions))]
    // [Union(8, typeof(Vector3ArrayOptions))]
    // [Union(9, typeof(VectorOptions))]
    public interface IPlugOptions
    {
        /// <summary>Resets the plugin</summary>
        void Reset();
    }
}