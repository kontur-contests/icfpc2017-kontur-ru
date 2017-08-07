using System;
using Newtonsoft.Json;

namespace lib
{
    public class StatValue
    {
        public void Add(double value)
        {
            Sum += value;
            Sum2 += value*value;
            Count++;
        }

        public void AddAll(StatValue value)
        {
            Sum += value.Sum;
            Sum2 += value.Sum2;
            Count += value.Count;
        }
        
        public double Dispersion => Math.Sqrt(Count * Sum2 - Sum * Sum) / Count;
        public double ConfIntervalSize => 2 * Math.Sqrt(Count * Sum2 - Sum * Sum) / Count / Math.Sqrt(Count);
        public double Mean => Sum / Count;
        public double Sum { get; set; }
        public double Sum2 { get; set; }
        public long Count { get; set; }
        public bool ShowDispersion = false;
        public double Delta => ShowDispersion ? ConfIntervalSize : Dispersion;

        public StatValue Clone()
        {
            return new StatValue
            {
                ShowDispersion = ShowDispersion,
                Sum = Sum,
                Sum2 = Sum2
            };
        }
    }
}