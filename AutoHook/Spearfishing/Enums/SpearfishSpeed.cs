namespace AutoHook.Spearfishing.Enums;

public enum SpearfishSpeed : ushort
{
    All = 0,
    SuperSlow     = 100,
    ExtremelySlow = 150,
    VerySlow      = 200,
    Slow          = 250,
    Average       = 300,
    Fast          = 350,
    VeryFast      = 400,
    ExtremelyFast = 450,
    SuperFast     = 500,
    HyperFast     = 550,
    LynFast       = 600,

    
}

public static class SpearFishSpeedExtensions
{
    public static string ToName(this SpearfishSpeed speed)
        => speed switch
        {
            SpearfishSpeed.All => "ȫ��",
            SpearfishSpeed.SuperSlow => "�����޵���",
            SpearfishSpeed.ExtremelySlow => "������",
            SpearfishSpeed.VerySlow => "�ǳ���",
            SpearfishSpeed.Slow => "�е���",
            SpearfishSpeed.Average => "һ���",
            SpearfishSpeed.Fast => "�е��",
            SpearfishSpeed.VeryFast => "�ǳ���",
            SpearfishSpeed.ExtremelyFast => "������",
            SpearfishSpeed.SuperFast => "�����޵п�",
            SpearfishSpeed.HyperFast => "�����޵п�",
            SpearfishSpeed.LynFast => "�쵽ģ��",

            _                            => $"{(ushort)speed}",
        };
}
