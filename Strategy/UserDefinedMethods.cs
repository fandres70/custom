using System.Collections.Generic;
using System.Threading;

#region Using declarations

using System;
using System.ComponentModel;
using System.Drawing;
using NinjaTrader.Cbi;
using NinjaTrader.Data;
using NinjaTrader.Indicator;
using NinjaTrader.Strategy;

#endregion

// This namespace holds all strategies and is required. Do not change it.

namespace NinjaTrader.Strategy
{
    /// <summary>
    /// This file holds all user defined strategy methods.
    /// </summary>
    partial class Strategy
    {
        //private enum PositionDirection
        //{
        //    Flat,
        //    Long,
        //    Short
        //};

        public bool NtAODifferentColor(int barsAgo)
        {
            int negCol = 0;
            int posCol = 0;
            for (int i = 1; i <= barsAgo; i++)
            {
                if (bwAO().AOValue[i] < 0)
                {
                    negCol++;
                }
                else if (bwAO().AOValue[i] > 0)
                {
                    posCol++;
                }
            }
            if ((negCol != 0) && (posCol != 0))
            {
                return true;
            }
            return false;
        }

        public bool NtRagheeDifferentColor(int barsAgo)
        {
            int negCol = 0;
            int posCol = 0;
            for (int i = 1; i <= barsAgo; i++)
            {
                if ((Open[i] <= Close[i]
                     && Close[i] > EMA(High, 34)[i]) ||
                    (Open[i] >= Close[i]
                     && Close[i] > EMA(High, 34)[i]))
                {
                    posCol++;
                }

                else if ((Open[i] <= Close[i]
                          && Close[i] < EMA(Low, 34)[i]) ||
                         (Open[i] >= Close[i]
                          && Close[i] < EMA(Low, 34)[i]))
                {
                    negCol++;
                }
            }
            if ((negCol != 0) && (posCol != 0))
            {
                return true;
            }
            return false;
        }

        public double NtGetHighest(int numOfBars)
        {
            double curHigh;
            double mostHigh = High[0];
            for (int i = 1; i <= numOfBars; i++)
            {
                curHigh = High[i];
                if (curHigh > mostHigh)
                {
                    mostHigh = curHigh;
                }
            }
            return mostHigh;
        }

        public double NtGetLowest(int numOfBars)
        {
            double curLow;
            double mostLow = Low[0];
            for (int i = 1; i <= numOfBars; i++)
            {
                curLow = Low[i];
                if (curLow < mostLow)
                {
                    mostLow = curLow;
                }
            }
            return mostLow;
        }

        /// <summary>
        /// Series Trend
        /// </summary>
        /// <param name="series"> input series</param>
        /// <param name="numOfBars"> number of bars </param>
        /// <returns>1 - rising, -1 falling, 0 flat or unknown</returns>
        public int NtSeriesTrend(IDataSeries series, int numOfBars)
        {
            bool rising = false;
            bool falling = false;
//            bool flat = false;

            int cnt = series.Count;
            for (int i = 1; i < cnt; i++)
            {
                if (series[i - 1] > series[i])
                {
                    rising = true;
                }
                else if (series[i - 1] < series[i])
                {
                    falling = true;
                }
//                else if (series[i - 1] == series[i])
//                {
//                    flat = true;
//                }
                if (rising && falling)
                {
                    return 0;
                }
            }
            if (rising && falling)
                return 0;

            if (rising)
                return 1;

            if (falling)
                return -1;

            return 0;
        }

        #region TTMWave

        /// <summary>
        /// Return whether the wave crossed
        /// </summary>
        /// <param name="direction">true for Long and false for Short</param>
        /// <param name="waveLongOrShort">true for UP and false for DOWN</param>
        /// <param name="waveA_B_or_C">could be A, B or C</param>
        /// <param name="numOfBars">Number of Bars ago </param>
        /// <returns>1 for long, -1 for short, 0 didn't cross</returns>
        public int NtIsWaveCrossed(bool direction, bool waveLongOrShort, string waveA_B_or_C, int numOfBars)
        {
            List<double> listValues = new List<double>();
            bool aboveZero = false;
            bool belowZero = false;
            switch (waveA_B_or_C)
            {
                case "A":
                    for (int i = 0; i <= numOfBars; i++)
                        listValues.Add(waveLongOrShort ? NtGetWaveALong(i) : NtGetWaveAShort(i));
                    break;
                case "B":
                    for (int i = 0; i <= numOfBars; i++)
                        listValues.Add(waveLongOrShort ? NtGetWaveBLong(i) : NtGetWaveBShort(i));
                    break;
                case "C":
                    for (int i = 0; i <= numOfBars; i++)
                        listValues.Add(waveLongOrShort ? NtGetWaveCLong(i) : NtGetWaveCShort(i));
                    break;
            }

            foreach (double val in listValues)
            {
                if (val > 0)
                    aboveZero = true;
                else if (val < 0)
                    belowZero = true;
                if (aboveZero && belowZero)
                {
                    if (val > 0)
                        return -1;
                    else
                        return 1;
                }
            }

            return 0;
        }

        public double NtGetWaveAShort(int barsAgo)
        {
            double rVal = 0.0;
            rVal = TTMWaveAOC(false).Wave1[barsAgo];
            return rVal;
        }

        public double NtGetWaveALong(int barsAgo)
        {
            double rVal = 0.0;
            rVal = TTMWaveAOC(false).Wave2[barsAgo];
            return rVal;
        }

        public double NtGetWaveBShort(int barsAgo)
        {
            double rVal = 0.0;
            rVal = TTMWaveBOC(false).Wave1[barsAgo];
            return rVal;
        }

        public double NtGetWaveBLong(int barsAgo)
        {
            double rVal = 0.0;
            rVal = TTMWaveBOC(false).Wave2[barsAgo];
            return rVal;
        }

        public double NtGetWaveCShort(int barsAgo)
        {
            double rVal = 0.0;
            rVal = TTMWaveCOC(false).Wave1[barsAgo];
            return rVal;
        }

        public double NtGetWaveCLong(int barsAgo)
        {
            double rVal = 0.0;
            rVal = TTMWaveCOC(false).Wave2[barsAgo];
            return rVal;
        }

        #endregion

        #region OMS

        public double NtGetPositionTest(Account account, Instrument instrument)
        {
            var x1 = NtGetUnrealizedNotional(account, instrument);
            var x2 = NtGetAvgPrice(account, instrument);
            var x3 = NtGetPositionDirection(account, instrument);
            return 0;
        }

        public double NtGetUnrealizedNotional(Account account, Instrument instrument)
        {
            Position myPosition = account.Positions.FindByInstrument(instrument);
            if (myPosition == null)
            {
                return 0;
            }
            return myPosition.GetProfitLoss(Close[0], PerformanceUnit.Currency);
        }
        public int NtGetUnrealizedQuantity(Account account, Instrument instrument)
        {
            Position myPosition = account.Positions.FindByInstrument(instrument);
            if (myPosition == null)
            {
                return 0;
            }
            return myPosition.Quantity;
        }

        private double NtGetAvgPrice(Account account, Instrument instrument)
        {
            Position myPosition = account.Positions.FindByInstrument(instrument);
            if (myPosition == null)
            {
                return 0;
            }
            return myPosition.AvgPrice;
        }

        public MarketPosition NtGetPositionDirection(Account account, Instrument instrument)
        {
            Position myPosition = account.Positions.FindByInstrument(instrument);
            if (myPosition == null)
            {
                return MarketPosition.Flat;
            }

            return myPosition.MarketPosition;

            //int iOrderCount = Account.Orders.Count;
            //Print("Total Open Orders: " + iOrderCount);
            //System.Collections.IEnumerator ListOrders = Account.Orders.GetEnumerator();
            //for (int i = 0; i < iOrderCount; i++)
            //{
            //    ListOrders.MoveNext();
            //    Print(" Open Orders: " + ListOrders.Current);
            //    Order myOrder = ListOrders.Current as NinjaTrader.Cbi.Order;
            //    //if (myOrder.OrderState == OrderState.Working)
            //    //    myOrder.Cancel();
            //}

            //return PositionDirection.Flat;
        }

        public void NtPopulateManualOrders(Account account, Instrument instrument, ref List<Order> orders)
        {
            int iOrderCount = Account.Orders.Count;
            System.Collections.IEnumerator ListOrders = Account.Orders.GetEnumerator();
            for (int i = 0; i < iOrderCount; i++)
            {
                ListOrders.MoveNext();
                Order myOrder = ListOrders.Current as NinjaTrader.Cbi.Order;
                if ((myOrder != null) 
                    && ((myOrder.OrderState == OrderState.Working) 
                    || (myOrder.OrderState == OrderState.PartFilled)))
                {
                    orders.Add(myOrder);
                }
            }
        }
        //private double NtGetUnrealizedNotional(Account account, Instrument instrument)
        //{
        //    MarketPosition marketPosition = NtGetPositionDirection(account, instrument);
        //    if (marketPosition == MarketPosition.Flat)
        //        return 0;
        //    double averagePrice = NtGetAvgPrice(account, instrument);
        //    double workingQty = NtGetUnrealizedNotional()
        //}

        public int NtGetAccountQuantity(string accountName)
        {
            IOrder ord = EnterLong(10000, "testorder");
            while (ord.OrderState == OrderState.Working)
            {
                Thread.Sleep(2000);
                Log("Working", LogLevel.Warning);
            }
            foreach (Account acct in Cbi.Globals.Accounts)
            {
                Log("acct.Name=" + acct.Name + ", curAcctName=" + accountName, LogLevel.Error);
                if (acct.Name != accountName) continue;
                var x = GetAccountValue(AccountItem.CashValue);
                var x2 = GetAccountValue(AccountItem.TotalCashBalance);
                var x3 = GetAccountValue(AccountItem.BuyingPower);
                Log(String.Format("CashValue is {0}", x), LogLevel.Warning);
                var xa = acct.GetAccountValue(AccountItem.CashValue, Currency.UsDollar);
                var xa2 = acct.GetAccountValue(AccountItem.TotalCashBalance, Currency.UsDollar);
                var xa3 = acct.GetAccountValue(AccountItem.BuyingPower, Currency.UsDollar);
                if (acct.Positions != null)
                {
                    PositionCollection positions = acct.Positions;
                    foreach (Position pos in positions)
                    {
                        Print(pos.Account.Name + " " + pos.Instrument + " " + pos.MarketPosition + " " + pos.Quantity +
                              " " + pos.AvgPrice);
                        Log(
                            pos.Account.Name + " " + pos.Instrument + " " + pos.MarketPosition + " " + pos.Quantity +
                            " " + pos.AvgPrice, LogLevel.Error);
                        return pos.Quantity;
                    }
                }
                else
                    return 0;
            }
            return -1;
        }

        #endregion
    }
}