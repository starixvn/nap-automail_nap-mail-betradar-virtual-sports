using Fanex.BetradarVirtualSports.Business.Models;
using System.Collections.Generic;

namespace Fanex.BetradarVirtualSports.Business.Services;

public static class AppConfigService
{
    public static ConfigSettingModel Config { get; set; }

    public static bool SafeToRestart { get; set; }

    public static List<int> Counter { get; set; } = [];
}