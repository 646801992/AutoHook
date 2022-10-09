using System.Diagnostics;
using AutoHook.Data;
using AutoHook.Utils;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Client.Game;
using ImGuiNET;

namespace AutoHook.Ui;

internal class TabGeneral : TabBaseConfig
{
    public override bool Enabled => true;
    public override string TabName => "ͨ��";

    public override void DrawHeader()
    {
        ImGui.Text("ͨ������");

        ImGui.Separator();
        ImGui.Spacing();
        if (ImGui.Button("Click here to report an issue or make a suggestion"))
        {
            Process.Start(new ProcessStartInfo { FileName = "https://github.com/InitialDet/AutoHook/issues", UseShellExecute = true });
        }
        ImGui.Spacing();

#if DEBUG

        if (ImGui.Button("����"))
        {
            PluginLog.Debug($"IdenticalCast = {PlayerResources.HasStatus(IDs.Status.IdenticalCast)}");

        }
#endif
    }
    public override void Draw()
    {

        if (ImGui.BeginTabBar("TabBarsGeneral", ImGuiTabBarFlags.NoTooltip))
        {
            if (ImGui.BeginTabItem("Ĭ��ֱ���׸�###DC1"))
            {
                ImGui.PushID("TabDefaultCast");
                DrawDefaultCast();
                ImGui.PopID();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Ĭ����С����###DM1"))
            {
                ImGui.PushID("TabDefaultMooch");
                DrawDefaultMooch();
                ImGui.PopID();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }

    }

    public void DrawDefaultCast()
    {
        ImGui.Spacing();
        ImGui.Checkbox("ʹ��Ĭ���׸�", ref Service.Configuration.DefaultCastConfig.Enabled);
        ImGuiComponents.HelpMarker("�Ҳ����ض����������ʱʹ�ø�Ĭ�����á�");

        ImGui.Indent();

        DrawInputDoubleMinTime(Service.Configuration.DefaultCastConfig);
        DrawInputDoubleMaxTime(Service.Configuration.DefaultCastConfig);
        DrawHookCheckboxes(Service.Configuration.DefaultCastConfig);
        DrawFishersIntuitionConfig(Service.Configuration.DefaultCastConfig);
        DrawCheckBoxDoubleTripleHook(Service.Configuration.DefaultCastConfig);
        //DrawPatienceConfig(Service.Configuration.DefaultCastConfig);

        ImGui.Unindent();

    }

    public void DrawDefaultMooch()
    {
        ImGui.Spacing();
        ImGui.Checkbox("ʹ��Ĭ����С����", ref Service.Configuration.DefaultMoochConfig.Enabled);
        ImGuiComponents.HelpMarker("�Ҳ����ض��������С��������ʱʹ�ø�Ĭ�����á�");

        ImGui.Indent();

        DrawInputDoubleMinTime(Service.Configuration.DefaultMoochConfig);
        DrawInputDoubleMaxTime(Service.Configuration.DefaultMoochConfig);
        DrawHookCheckboxes(Service.Configuration.DefaultMoochConfig);
        DrawFishersIntuitionConfig(Service.Configuration.DefaultMoochConfig);
        DrawCheckBoxDoubleTripleHook(Service.Configuration.DefaultMoochConfig);
        //DrawPatienceConfig(Service.Configuration.DefaultMoochConfig);

        ImGui.Unindent();
    }
}