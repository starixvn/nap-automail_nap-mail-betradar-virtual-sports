using System;

namespace Fanex.BetradarVirtualSports.Business.Models
{
    public class GrossWinningDaily
    {
        public DateTime Date { get; set; }

        public string Business { get; set; }

        public string GameType { get; set; }

        public string Currency { get; set; }

        public string DeviceGroup { get; set; }

        public int BetCount { get; set; }

        public decimal TotalBet { get; set; }

        public decimal TotalPayout { get; set; }

        public decimal TotalWinLoss { get; set; }

        public decimal Margin { get; set; }
    }
}