// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2014/07/13 16:37
// 
// License Copyright (c) Daniele Giardini.
// This work is subject to the terms at http://dotween.demigiant.com/license.php

#pragma warning disable 1591
// using MessagePack;

namespace DG.Tweening.Plugins.Options
{
    // [MessagePackObject]
    public struct StringOptions : IPlugOptions
    {
        // [Key(0)] 
        public bool richTextEnabled;
        // [Key(1)] 
        public ScrambleMode scrambleMode;
        // [Key(2)] 
        public char[] scrambledChars; // If empty uses default scramble characters

        // Stored by StringPlugin
        // [Key(3)] 
        public int startValueStrippedLength;
        // [Key(4)] 
        public int changeValueStrippedLength; // No-tag lengths of start and change value

        public void Reset()
        {
            richTextEnabled = false;
            scrambleMode = ScrambleMode.None;
            scrambledChars = null;
            startValueStrippedLength = changeValueStrippedLength = 0;
        }
    }
}