// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/09/01 18:50
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

// using MessagePack;
using UnityEngine;

#pragma warning disable 1591
namespace DG.Tweening.Plugins.Options
{
    // [MessagePackObject]
    public struct QuaternionOptions : IPlugOptions
    {
        // [Key(0)] 
        public RotateMode rotateMode; // Accessed by shortcuts and Modules
        // [Key(1)] 
        public AxisConstraint axisConstraint; // Used by SpecialStartupMode SetLookAt, accessed by shortcuts and Modules
        // [Key(2)] 
        public Vector3 up; // Used by SpecialStartupMode SetLookAt, accessed by shortcuts and Modules

        public void Reset()
        {
            rotateMode = RotateMode.Fast;
            axisConstraint = AxisConstraint.None;
            up = Vector3.zero;
        }
    }
}