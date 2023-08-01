using System;

namespace Fanex.BetradarVirtualSports.Business.Models;

public class ConfigSettingModel
{
    public string SentryClientKey { get; set; }

    public string CategorySentryLog { get; set; }

    public bool IsManual { get; set; }

    public bool IsCheckEnoughData { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public DateTime RptDate { get; set; }

    public string Password { get; set; }

    public string Path { get; set; }
}