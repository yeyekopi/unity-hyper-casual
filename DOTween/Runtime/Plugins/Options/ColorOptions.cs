﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/28 12:27
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#pragma warning disable 1591
// using MessagePack;

namespace DG.Tweening.Plugins.Options
{
    // [MessagePackObject]
    public struct ColorOptions : IPlugOptions
    {
        // [Key(0)] 
        public bool alphaOnly;

        public void Reset()
        {
            alphaOnly = false;
        }
    }
}