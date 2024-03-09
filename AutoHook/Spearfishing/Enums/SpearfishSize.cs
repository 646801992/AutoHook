using System;

namespace AutoHook.Spearfishing.Enums;

public enum SpearfishSize : byte
{
    All = 0,
    Small   = 1,
    Average = 2,
    Large   = 3,
   
}

public static class SpearFishSizeExtensions
{
    public static string ToName(this SpearfishSize size)
        => size switch
        {
            SpearfishSize.All => "全部",
            SpearfishSize.Small => "小型",
            SpearfishSize.Average => "中型",
            SpearfishSize.Large => "大型",

            _                     => throw new ArgumentOutOfRangeException(nameof(size), size, null),
        };
}