using Fanex.Data.Repository;

namespace Fanex.BetradarVirtualSports.Business.Criteria
{
    public class DWStatusByCubeCriteria : CriteriaBase
    {
        public string TaskIDList => "38";

        public override string GetSettingKey()
        {
            return "DailyJob_CheckTasksStatus";
        }

        public override bool IsValid() => true;
    }
}