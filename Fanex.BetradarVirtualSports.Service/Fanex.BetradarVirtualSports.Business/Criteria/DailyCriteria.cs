using Fanex.Data.Repository;
using System;

namespace Fanex.BetradarVirtualSports.Business.Criteria;

public class DailyCriteria : CriteriaBase
{
    public DateTime RptDate { get; set; }

    public override string GetSettingKey() => "Acc_Rpt_Automail_BetradarVS_GrossWinning_Daily";

    public override bool IsValid() => true;
}