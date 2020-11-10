using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SimpleRSIbot : Robot
    {
        [Parameter("source")]
        public DataSeries Source { get; set; }

        [Parameter("period", DefaultValue = 14, MinValue = 1)]
        public int period { get; set; }

        [Parameter("lots", DefaultValue = 1000, MinValue = 0.01, MaxValue = 10000)]
        public double lots { get; set; }

        [Parameter("stopLoss", DefaultValue = 0, MinValue = 0)]
        public double stopLoss { get; set; }

        [Parameter("takeProfit", DefaultValue = 0, MinValue = 0)]
        public double takeProfit { get; set; }

        private RelativeStrengthIndex rsi1;

        protected override void OnStart()
        {
            rsi1 = Indicators.RelativeStrengthIndex(Source, period);
        }

        protected override void OnTick()
        {
            if (rsi1.Result.LastValue > 80)
            {
                openPosition(TradeType.Sell);
                closePosition(TradeType.Buy);
            }
            else if (rsi1.Result.LastValue < 20)
            {
                openPosition(TradeType.Buy);
                closePosition(TradeType.Sell);
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }

        private void closePosition(TradeType tradeType)
        {
            foreach (var positionsInfo in Positions.FindAll("SimpleRSIbot", Symbol, tradeType))
            {
                ClosePosition(positionsInfo);
            }
        }

        private void openPosition(TradeType tradeType)
        {
            var positionInfo = Positions.Find("SimpleRSIbot", Symbol, tradeType);
            if (positionInfo == null)
            {
                ExecuteMarketOrder(tradeType, Symbol, lots, "SimpleRSIbot");
            }
        }
    }
}
