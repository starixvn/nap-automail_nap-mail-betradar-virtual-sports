using Fanex.Data.Repository;
using System;

namespace Fanex.BetradarVirtualSports.Business.Criteria;

public class MonthlyCriteria : CriteriaBase
{
    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public override string GetSettingKey() => "Acc_Rpt_Automail_BetradarVS_GrossWinning_Monthly";

    public override bool IsValid() => true;
}