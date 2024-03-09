using AutoHook.Configurations;
using AutoHook.FishTimer;
using AutoHook.SeFunctions;
using AutoHook.Spearfishing;
using AutoHook.Utils;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Plugin;
using SeFunctions;

namespace AutoHook;

public class AutoHook : IDalamudPlugin
{
    public string Name => "AutoHook";

    private const string CmdAHCfg = "/ahcfg";
    private const string CmdAH = "/ah";
    private const string CmdAHOn = "/ahon";
    private const string CmdAHOff = "/ahoff";
    private const string CmdAHTG = "/ahtg";

    private static PluginUI PluginUI = null!;

    private static AutoGig AutoGig = null!;

    public HookingManager HookManager;

    PlayerResources PlayerResources;

    public AutoHook(DalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        Service.EventFramework = new EventFramework(Service.SigScanner);
        Service.CurrentBait = new CurrentBait(Service.SigScanner);
        Service.TugType = new SeTugType(Service.SigScanner);
        Service.PluginInterface!.UiBuilder.Draw += Service.WindowSystem.Draw;
        Service.PluginInterface!.UiBuilder.OpenConfigUi += OnOpenConfigUi;
        Service.Configuration = Configuration.Load();
        Service.Language = Service.ClientState.ClientLanguage;

        PlayerResources = new PlayerResources();
        PlayerResources.Initialize();

        PluginUI = new PluginUI();
        AutoGig = new AutoGig();

        Service.Commands.AddHandler(CmdAHOff, new CommandInfo(OnCommand)
        {
            HelpMessage = "天才妹妹停止钓鱼"
        });

        Service.Commands.AddHandler(CmdAHOn, new CommandInfo(OnCommand)
        {
            HelpMessage = "天才妹妹开始钓鱼"
        });

        Service.Commands.AddHandler(CmdAHCfg, new CommandInfo(OnCommand)
        {
            HelpMessage = "打开设置窗口"
        });

        Service.Commands.AddHandler(CmdAH, new CommandInfo(OnCommand)
        {
            HelpMessage = "打开设置窗口"
        });

        Service.Commands.AddHandler(CmdAHTG, new CommandInfo(OnCommand)
        {
            HelpMessage = "切换天才妹妹状态"
        });

        HookManager = new HookingManager();

#if (DEBUG)
        OnOpenConfigUi();
#endif
    }

    private void OnCommand(string command, string args)
    {
        if (command.Trim().Equals(CmdAHCfg))
            OnOpenConfigUi();

        if (command.Trim().Equals(CmdAH))
            OnOpenConfigUi();

        if (command.Trim().Equals(CmdAHOn))
        {
            Service.Chat.Print("天才妹妹开始钓鱼");
            Service.Configuration.PluginEnabled = true;
        }

        if (command.Trim().Equals(CmdAHOff))
        {
            Service.Chat.Print("天才妹妹停止钓鱼");
            Service.Configuration.PluginEnabled = false;
        }

        if (command.Trim().Equals(CmdAHTG))
        {
            if (Service.Configuration.PluginEnabled)
            {
                Service.Chat.Print("天才妹妹停止钓鱼");
                Service.Configuration.PluginEnabled = false;
            }
            else
            {
                Service.Chat.Print("天才妹妹开始钓鱼");
                Service.Configuration.PluginEnabled = true;
            }
        }
    }

    public void Dispose()
    {
        PluginUI.Dispose();
        AutoGig.Dispose();
        HookManager.Dispose();
        PlayerResources.Dispose();
        Service.Configuration.Save();
        Service.PluginInterface!.UiBuilder.Draw -= Service.WindowSystem.Draw;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
        Service.Commands.RemoveHandler(CmdAHCfg);
        Service.Commands.RemoveHandler(CmdAH);
        Service.Commands.RemoveHandler(CmdAHOn);
        Service.Commands.RemoveHandler(CmdAHOff);
        Service.Commands.RemoveHandler(CmdAHTG);
    }

    private void OnOpenConfigUi() => PluginUI.Toggle();
}

