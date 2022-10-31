using System;
using System.Numerics;
using AutoHook.Configurations;
using AutoHook.Enums;
using AutoHook.Utils;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace AutoHook.Ui;
abstract class TabBaseConfig : IDisposable
{
    public abstract string TabName { get; }
    public abstract bool Enabled { get; }

    public static string StrHookWeak => "ҧ����� (!)";
    public static string StrHookStrong => "ҧ���и� (!!)";
    public static string StrHookLegendary => "ҧ���ظ� (!!!)";

    public abstract void DrawHeader();

    public abstract void Draw();

    public virtual void Dispose() { }

    public void DrawDeleteBaitButton(HookConfig cfg)
    {
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconChar()}", new Vector2(ImGui.GetFrameHeight(), 0)) && ImGui.GetIO().KeyShift)
        {
            Service.Configuration.CustomBait.RemoveAll(x => x.BaitName == cfg.BaitName);
            Service.Configuration.Save();
        }
        ImGui.PopFont();

        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("��ס SHIFT ɾ����");
    }

    public void DrawHookCheckboxes(HookConfig cfg)
    {
        DrawSelectTugs(StrHookWeak, ref cfg.HookWeakEnabled, ref cfg.HookTypeWeak);
        DrawSelectTugs(StrHookStrong, ref cfg.HookStrongEnabled, ref cfg.HookTypeStrong);
        DrawSelectTugs(StrHookLegendary, ref cfg.HookLegendaryEnabled, ref cfg.HookTypeLegendary);
    }

    public void DrawSelectTugs(string hook, ref bool enabled, ref HookType type)
    {
       
        ImGui.Checkbox(hook, ref enabled);
        if (ImGui.IsItemHovered())
            ImGui.SetTooltip("δ��������ʱ��ֱ��ʹ�� \"�ṳ\"");

        if (enabled)
        {
            ImGui.Indent();
            if (ImGui.RadioButton($"��׼�ṳ###{TabName}{hook}1", type == HookType.Precision))
            {
                type = HookType.Precision;
                Service.Configuration.Save();
            }

            if (ImGui.RadioButton($"ǿ���ṳ###{TabName}{hook}2", type == HookType.Powerful))
            {
                type = HookType.Powerful;
                Service.Configuration.Save();
            }
            ImGui.Unindent();
        }
    }

    public void DrawInputTextName(HookConfig cfg)
    {
        string matchText = new string(cfg.BaitName);
        ImGui.SetNextItemWidth(-260 * ImGuiHelpers.GlobalScale);
        if (ImGui.InputText("��С����/��� ����", ref matchText, 64, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
        {
            if (cfg.BaitName != matchText && Service.Configuration.CustomBait.Contains(new HookConfig(matchText)))
                cfg.BaitName = "����Ѵ���";
            else
                cfg.BaitName = matchText;

            Service.Configuration.Save();
        };
    }

    public void DrawInputDoubleMaxTime(HookConfig cfg)
    {
        ImGui.SetNextItemWidth(100 * ImGuiHelpers.GlobalScale);
        if (ImGui.InputDouble("��ȴ�", ref cfg.MaxTimeDelay, .1, 1, "%.1f%"))
        {
            switch (cfg.MaxTimeDelay)
            {
                case 0.1:
                    cfg.MaxTimeDelay = 2;
                    break;
                case <= 0:
                case <= 1.9: //This makes the option turn off if delay is 2 seconds when clicking the minus.
                    cfg.MaxTimeDelay = 0;
                    break;
                case > 99:
                    cfg.MaxTimeDelay = 99;
                    break;
            }
        }
        ImGui.SameLine();
        ImGuiComponents.HelpMarker("Hook will be used after the defined amount of time has passed\nMin. time: 2s (because of animation lock)\n\nSet Zero (0) to disable, and dont make this lower than the Min. Wait");
    }

    public void DrawInputDoubleMinTime(HookConfig cfg)
    {
        ImGui.SetNextItemWidth(100 * ImGuiHelpers.GlobalScale);
        if (ImGui.InputDouble("��̵ȴ�", ref cfg.MinTimeDelay, .1, 1, "%.1f%"))
        {
            switch (cfg.MinTimeDelay)
            {
                case <= 0:
                    cfg.MinTimeDelay = 0;
                    break;
                case > 99:
                    cfg.MinTimeDelay = 99;
                    break;
            }
        }

        ImGui.SameLine();
        ImGuiComponents.HelpMarker("Hook will NOT be used until the minimum time has passed.\n\nEx: If you set the number as 14 and something bites after 8 seconds, the fish will not to be hooked\n\nSet Zero (0) to disable");
    }

    public void DrawEnabledButtonCustomBait(HookConfig cfg)
    {
        ImGui.Checkbox("�������� ->", ref cfg.Enabled);
        ImGuiComponents.HelpMarker("Important!!!\n\nIf disabled, the fish will NOT be hooked or Mooched.\nTo use the default behavior (General Tab), please delete this configuration.");
    }

    public void DrawCheckBoxDoubleTripleHook(HookConfig cfg)
    {

        if (ImGui.Button("�����ṳ ����###DHTH"))
        {
            ImGui.OpenPopup("�����ṳ###DHTH");
        }
        if (ImGui.BeginPopup("�����ṳ###DHTH"))
        {

            ImGui.TextColored(ImGuiColors.DalamudYellow, "�����ṳ����");
            ImGui.Spacing();

            ImGui.Checkbox("����רһ����ʱ����", ref cfg.UseDHTHOnlySurfaceSlap);
            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if (ImGui.Checkbox("ʹ��˫�� (�� gp > 400)", ref cfg.UseDoubleHook))
            {
                if (cfg.UseDoubleHook) cfg.UseTripleHook = false;
                Service.Configuration.Save();
            }
            if (ImGui.Checkbox("ʹ������ (�� gp > 700)", ref cfg.UseTripleHook))
            {
                if (cfg.UseTripleHook) cfg.UseDoubleHook = false;
                Service.Configuration.Save();
            }

            if (cfg.UseTripleHook || cfg.UseDoubleHook)
            {
                ImGui.Indent();

                ImGui.Checkbox("��������ʱҲʹ�� (���Ƽ�)", ref cfg.UseDHTHPatience);
                ImGuiComponents.HelpMarker("ע��!!!\n\n����ʱ����������ʹ�þ�׼�ṳ��ǿ���ṳ��");
                ImGui.Checkbox("�� GP ����Ҫ��ʱ�������", ref cfg.LetFishEscape);
                ImGui.Unindent();

                ImGui.Separator();
                ImGui.Spacing();

                ImGui.Checkbox(StrHookWeak, ref cfg.HookWeakDHTHEnabled);
                ImGui.Checkbox(StrHookStrong, ref cfg.HookStrongDHTHEnabled);
                ImGui.Checkbox(StrHookLegendary, ref cfg.HookLegendaryDHTHEnabled);
            }

            ImGui.EndPopup();
        }

    }

    public void DrawFishersIntuitionConfig(HookConfig cfg)
    {
        if (ImGui.Button("������֮ʶ ����###FishersIntuition"))
        {
            ImGui.OpenPopup("fisher_intuition_settings");
        }

        if (ImGui.BeginPopup("fisher_intuition_settings"))
        {
            ImGui.TextColored(ImGuiColors.DalamudYellow, "������֮ʶ");
            ImGui.Spacing();
            Utils.DrawUtil.Checkbox("Enable", ref cfg.UseCustomIntuitionHook, "����⵽������֮ʶʱ���������ṳ");
            ImGui.Separator();

            DrawSelectTugs(StrHookWeak, ref cfg.HookWeakIntuitionEnabled, ref cfg.HookTypeWeakIntuition);
            DrawSelectTugs(StrHookStrong, ref cfg.HookStrongIntuitionEnabled, ref cfg.HookTypeStrongIntuition);
            DrawSelectTugs(StrHookLegendary, ref cfg.HookLegendaryIntuitionEnabled, ref cfg.HookTypeLegendaryIntuition);

            ImGui.EndPopup();
        }
    }

    public void DrawAutoMooch(HookConfig cfg)
    {

        if (ImGui.Button("�Զ���С����"))
        {
            ImGui.OpenPopup("auto_mooch");
        }

        if (ImGui.BeginPopup("auto_mooch"))
        {
            ImGui.TextColored(ImGuiColors.DalamudYellow, "�Զ���С����");
            ImGui.Spacing();
            ImGui.Text("- If this is a Bait, all fish caught by this bait will be mooched");
            ImGui.Text("- If this is a Fish/Mooch (Ex: Harbor Herring), it'll be mooched when caught");
            ImGui.Text("If this option is disabled, it will NOT be mooched even if Auto Mooch is also enabled in the general tab");
            if (Utils.DrawUtil.Checkbox("�Զ���С����", ref cfg.UseAutoMooch, "This option takes priority over the Auto Cast Line"))
            {
                if (!cfg.UseAutoMooch)
                    cfg.UseAutoMooch2 = false;
            }

            if (cfg.UseAutoMooch)
            {
                ImGui.Indent();
                ImGui.Checkbox("ʹ����С����II", ref cfg.UseAutoMooch2);
                ImGui.Unindent();
            }
            ImGui.EndPopup();
        }
    }

    public void DrawSurfaceSlapIdenticalCast(HookConfig cfg)
    {

        if (ImGui.Button("�Ļ�ˮ���רһ����"))
        {
            ImGui.OpenPopup("surface_slap_identical_cast");
        }

        if (ImGui.BeginPopup("surface_slap_identical_cast"))
        {
            ImGui.TextColored(ImGuiColors.DalamudYellow, "�Ļ�ˮ���רһ����");
            ImGui.Spacing();
            if (DrawUtil.Checkbox("ʹ���Ļ�ˮ��", ref cfg.UseSurfaceSlap, "����רһ����"))
            {
                cfg.UseIdenticalCast = false;
            }

            if (DrawUtil.Checkbox("ʹ��רһ����", ref cfg.UseIdenticalCast, "�����Ļ�ˮ��"))
            {
                cfg.UseSurfaceSlap = false;
            }

            ImGui.EndPopup();
        }
    }

    public void DrawStopAfter(HookConfig cfg)
    {

        if (ImGui.Button("ֹͣ������..."))
        {
            ImGui.OpenPopup(str_id: "stop_after");
        }

        if (ImGui.BeginPopupContextWindow("stop_after"))
        {
            ImGui.TextColored(ImGuiColors.DalamudYellow, "ֹͣ����");
            ImGui.Spacing();
            if (DrawUtil.Checkbox("����", ref cfg.StopAfterCaught, "- ��������������: ������ṳ��������ֹͣ��\n- ����ڵ���������: ����������������ֹͣ��"))
            {

            }
            if (cfg.StopAfterCaught)
            {
                ImGui.Indent();
                ImGui.SetNextItemWidth(90 * ImGuiHelpers.GlobalScale);
                if (ImGui.InputInt("�κ�", ref cfg.StopAfterCaughtLimit))
                {
                    if (cfg.StopAfterCaughtLimit < 1)
                        cfg.StopAfterCaughtLimit = 1;
                }

                ImGui.Unindent();
            }

            ImGui.EndPopup();
        }
    }
}
