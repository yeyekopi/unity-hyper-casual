﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/05/30 13:00
// License Copyright (c) Daniele Giardini
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#pragma warning disable 1591
// using MessagePack;

namespace DG.Tweening.Plugins.Options
{
    // [MessagePackObject]
    public struct UintOptions : IPlugOptions
    {
        // [Key(0)] 
        public bool isNegativeChangeValue; // Necessary because uints can't obviously be negative

        public void Reset()
        {
            isNegativeChangeValue = false;
        }
    }
}