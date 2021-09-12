﻿namespace Thinksharp.TimeFlow
{
  using System;
  using System.Globalization;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;

  [TestClass]
  public class TimeSeriesTest
  {
    [TestMethod]
    public void TestConst_1_start_count()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, 96, Frequency.QuarterHours);
      
      Assert.AreEqual(96, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);      
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValue_1_start_end()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Frequency.QuarterHours);

      Assert.AreEqual(96, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);      
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValue_1_start_end_march()
    {
      var start = new DateTimeOffset(new DateTime(2021, 03, 28));
      var end = new DateTimeOffset(new DateTime(2021, 03, 28, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Frequency.QuarterHours);

      Assert.AreEqual(92, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }

    [TestMethod]
    public void TestFromValue_1_start_end_october()
    {
      var start = new DateTimeOffset(new DateTime(2021, 10, 31));
      var end = new DateTimeOffset(new DateTime(2021, 10, 31, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(1, start, end, Frequency.QuarterHours);

      Assert.AreEqual(100, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == 1M));
    }    

    [TestMethod]
    public void TestFromValue_Null()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(null, start, 96, Frequency.QuarterHours);

      Assert.AreEqual(96, ts.Count);
      Assert.AreEqual(start, ts.Start);
      Assert.AreEqual(end, ts.End);
      Assert.IsTrue(ts.All(x => x.Value == null));
    }

    [TestMethod]
    public void TestMapValue()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 45, 0));
      var ts = TimeSeries.Factory.FromValue(2, start, 96, Frequency.QuarterHours);

      Assert.AreEqual(96, ts.Count);
      Assert.IsTrue(ts.All(x => x.Value == 2));

      var ts2 = ts.ApplyValues(x => x * x);
      Assert.IsTrue(ts2.All(x => x.Value == 4));

      var ts3 = ts.Apply(x => (x ?? 0) * (x ?? 0));
      Assert.IsTrue(ts3.All(x => x.Value == 4));
    }

    [TestMethod]
    public void TestJoinLeft_right_within_left()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end2 = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Frequency.QuarterHours);

      var joined = left.JoinLeft(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinLeft_right_within_left_op()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end2 = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Frequency.QuarterHours);

      var joined = left.JoinLeft(right, JoinOperation.Add);

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinLeft_left_within_right()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Frequency.QuarterHours);

      var joined = left.JoinLeft(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(0, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(0, part3.Count);

      Assert.IsTrue(part2.All(x => x.Value == 4));
    }

    [TestMethod]
    public void TestJoinFull_right_within_left()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end2 = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Frequency.QuarterHours);

      var joined = left.JoinFull(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinFull_left_within_right()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Frequency.QuarterHours);

      var joined = left.JoinFull(right, (x, y) => (x ?? 0) + (y ?? 0));

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinFull_left_within_right_op()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 31, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(2, start2, end2, Frequency.QuarterHours);

      var joined = left.JoinFull(right, JoinOperation.Add);

      var part1 = joined.Slice(new DateTime(2021, 01, 01), new DateTime(2021, 01, 09, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 13), new DateTime(2021, 1, 31, 23, 45, 0));

      Assert.AreEqual(9 * 96, part1.Count);
      Assert.AreEqual(3 * 96, part2.Count);
      Assert.AreEqual(19 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 2));
      Assert.IsTrue(part2.All(x => x.Value == 4));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }

    [TestMethod]
    public void TestJoinFull_right_before_right()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 10));
      var end = new DateTimeOffset(new DateTime(2021, 01, 12, 23, 45, 0));
      var left = TimeSeries.Factory.FromValue(2, start, end, Frequency.QuarterHours);

      var start2 = new DateTimeOffset(new DateTime(2020, 12, 30));
      var end2 = new DateTimeOffset(new DateTime(2021, 1, 02, 23, 45, 0));
      var right = TimeSeries.Factory.FromValue(5, start2, end2, Frequency.QuarterHours);

      decimal? Agg(decimal? l, decimal? r) => l == null && r == null ? (decimal?) null : (l ?? 0) + (r ?? 0);
      var joined = left.JoinFull(right, Agg);

      var part1 = joined.Slice(new DateTime(2020, 12, 30), new DateTime(2021, 1, 02, 23, 45, 0));
      var part2 = joined.Slice(new DateTime(2021, 01, 03), new DateTime(2021, 01, 09, 23, 45, 0));
      var part3 = joined.Slice(new DateTime(2021, 01, 10), new DateTime(2021, 01, 12, 23, 45, 0));

      Assert.AreEqual(4 * 96, part1.Count);
      Assert.AreEqual(7 * 96, part2.Count);
      Assert.AreEqual(3 * 96, part3.Count);

      Assert.IsTrue(part1.All(x => x.Value == 5));
      Assert.IsTrue(part2.All(x => x.Value == null));
      Assert.IsTrue(part3.All(x => x.Value == 2));
    }
      
    [TestMethod]
    public void Test_DownSample_15min_to_hours()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 23, 0, 0));

      // Value: 1
      var ts = TimeSeries.Factory.FromValue(1M, start, 96, Frequency.QuarterHours);

      var ts_rs = ts.ReSample(Frequency.Hours, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 4M));

      ts_rs = ts.ReSample(Frequency.Hours, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 1M));

      // Value: null
      ts = TimeSeries.Factory.FromValue(null, start, 96, Frequency.QuarterHours);

      ts_rs = ts.ReSample(Frequency.Hours, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));

      ts_rs = ts.ReSample(Frequency.Hours, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 24);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));
    }

    [TestMethod]
    public void Test_DownSample_15min_to_days()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 1, 0, 0, 0));

      // Value: 1
      var ts = TimeSeries.Factory.FromValue(1M, start, 96, Frequency.QuarterHours);

      var ts_rs = ts.ReSample(Frequency.Days, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 96M));

      ts_rs = ts.ReSample(Frequency.Days, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 1M));

      // Value: null
      ts = TimeSeries.Factory.FromValue(null, start, 96, Frequency.QuarterHours);

      ts_rs = ts.ReSample(Frequency.Days, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));

      ts_rs = ts.ReSample(Frequency.Days, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 1);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));
    }

    [TestMethod]
    public void TestDownSample_15min_to_3days()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 1, 3, 0, 0, 0));

      // Value: 1
      var ts = TimeSeries.Factory.FromValue(1M, start, 96 * 3, Frequency.QuarterHours);

      var ts_rs = ts.ReSample(Frequency.Days, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 96M));

      ts_rs = ts.ReSample(Frequency.Days, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == 1M));

      // Value: null
      ts = TimeSeries.Factory.FromValue(null, start, 96 * 3, Frequency.QuarterHours);

      ts_rs = ts.ReSample(Frequency.Days, AggregationType.Sum);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));

      ts_rs = ts.ReSample(Frequency.Days, AggregationType.Mean);

      Assert.AreEqual(ts_rs.Count, 3);
      Assert.AreEqual(start, ts_rs.Start);
      Assert.AreEqual(end, ts_rs.End);
      Assert.IsTrue(ts_rs.All(x => x.Value == null));
    }

    [TestMethod]
    public void TestDownSample_15min_to_month()
    {
      var start = new DateTimeOffset(new DateTime(2021, 03, 01));
      var end = new DateTimeOffset(new DateTime(2021, 04, 01)) - Frequency.QuarterHours;

      var ts = TimeSeries.Factory.FromValue(1M, start, end, Frequency.QuarterHours);
      var ts_month_mean = ts.ReSample(Frequency.Months, AggregationType.Mean);
      var ts_month_sum = ts.ReSample(Frequency.Months, AggregationType.Sum);

      Assert.AreEqual(1, ts_month_mean.Count);
      Assert.AreEqual(1, ts_month_sum.Count);

      Assert.AreEqual(1M, ts_month_mean[0].Value);
      Assert.AreEqual((decimal)31 * 24 * 4 - 4, ts_month_sum[0].Value);
    }

    [TestMethod]
    public void TestUpSample_from_year_to_quarteryear()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Frequency.Years);

      var ts_quarter_sum = ts_year.ReSample(Frequency.QuarterYears, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Frequency.QuarterYears, AggregationType.Mean);

      Assert.AreEqual(4, ts_quarter_mean.Count);
      Assert.AreEqual(4, ts_quarter_sum.Count);

      Assert.AreEqual(365, ts_quarter_mean[0].Value);
      Assert.AreEqual(89.958, (double)ts_quarter_sum[0].Value, 0.001);

      Assert.AreEqual(365, ts_quarter_mean[1].Value);
      Assert.AreEqual(91M, ts_quarter_sum[1].Value);

      Assert.AreEqual(365, ts_quarter_mean[2].Value);
      Assert.AreEqual(92, (double)ts_quarter_sum[2].Value, 0.01);

      Assert.AreEqual(365, ts_quarter_mean[3].Value);
      Assert.AreEqual(92.0416, (double)ts_quarter_sum[3].Value, 0.01);
    }

    [TestMethod]
    public void TestUpSample_from_year_to_month()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Frequency.Years);

      var ts_quarter_sum = ts_year.ReSample(Frequency.Months, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Frequency.Months, AggregationType.Mean);

      Assert.AreEqual(12, ts_quarter_mean.Count);
      Assert.AreEqual(12, ts_quarter_sum.Count);

      for (int i = 1; i <= 12; i++)
      {
        if (i == 3)
        {
          Assert.AreEqual(30.9583333333333, (double)ts_quarter_sum[i - 1].Value, 0.001);
          
        }
        else if (i == 10)
        {
          Assert.AreEqual(31.0416666666667, (double)ts_quarter_sum[i - 1].Value, 0.001);
        }
        else
        {
          Assert.AreEqual(DateTime.DaysInMonth(2021, i), (double)ts_quarter_sum[i - 1].Value, 0.001);
        }
        Assert.AreEqual(365, ts_quarter_mean[i - 1].Value);
      }
    }

    [TestMethod]
    public void TestUpSample_from_year_to_day()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Frequency.Years);

      var ts_quarter_sum = ts_year.ReSample(Frequency.Days, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Frequency.Days, AggregationType.Mean);

      Assert.AreEqual(365, ts_quarter_mean.Count);
      Assert.AreEqual(365, ts_quarter_sum.Count);

      Assert.IsTrue(ts_quarter_mean.All(x => x.Value == 365));

      foreach (var item in ts_quarter_sum)
      {
        if (item.Key == new DateTimeOffset(new DateTime(2021, 03, 28)))
        {
          Assert.AreEqual(0.9533333, (double)item.Value, 0.01);
        }
        else if (item.Key == new DateTimeOffset(new DateTime(2021, 10, 31)))
        {
          Assert.AreEqual(1.04166666666, (double)item.Value, 0.01);
        }
        else
        {
          Assert.AreEqual(1M, item.Value);
        }
      }
    }

    [TestMethod]
    public void TestUpSample_from_year_to_hour()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Frequency.Years);

      var ts_quarter_sum = ts_year.ReSample(Frequency.Hours, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Frequency.Hours, AggregationType.Mean);

      Assert.AreEqual(365 * 24, ts_quarter_mean.Count);
      Assert.AreEqual(365 * 24, ts_quarter_sum.Count);

      Assert.IsTrue(ts_quarter_mean.All(x => x.Value == 365));
      Assert.IsTrue(ts_quarter_sum.All(x => x.Value == (1 / 24M)));
    }

    [TestMethod]
    public void TestUpSample_from_year_to_quarterhour()
    {
      var start = new DateTimeOffset(new DateTime(2021, 01, 01));
      var end = new DateTimeOffset(new DateTime(2021, 12, 31, 23, 45, 00));

      var ts_year = TimeSeries.Factory.FromValue(365, start, end, Frequency.Years);

      var ts_quarter_sum = ts_year.ReSample(Frequency.QuarterHours, AggregationType.Sum);
      var ts_quarter_mean = ts_year.ReSample(Frequency.QuarterHours, AggregationType.Mean);

      Assert.AreEqual(365 * 96, ts_quarter_mean.Count);
      Assert.AreEqual(365 * 96, ts_quarter_sum.Count);

      Assert.IsTrue(ts_quarter_mean.All(x => x.Value == 365));
      Assert.IsTrue(ts_quarter_sum.All(x => x.Value == (1 / 96M)));
    }

    [TestMethod]
    public void TestTrim()
    {
      var ts1 = TimeSeries.Factory.FromValue(null, new DateTimeOffset(new DateTime(2021, 01, 01)), 48, Frequency.Hours);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTimeOffset(new DateTime(2021, 01, 01, 2, 0, 0)), 44, Frequency.Hours);

      var ts = ts1.JoinLeft(ts2, (l, r) => r);

      Assert.AreEqual(48, ts1.Count);
      Assert.IsTrue(ts.Values.Any(v => v == null));
      Assert.IsTrue(ts.Values.Any(v => v == 1));

      // trim full
      var tsTrimFull = ts.Trim();

      Assert.AreEqual(44, tsTrimFull.Count);
      Assert.IsFalse(tsTrimFull.Values.Any(v => v == null));
      Assert.IsTrue(tsTrimFull.Values.Any(v => v == 1));
      Assert.AreEqual(1M, tsTrimFull[0].Value);
      Assert.AreEqual(1M, tsTrimFull[tsTrimFull.Count - 1].Value);

      // trim left
      var tsTrimLeft = ts.Trim(dropTrailing: false);

      Assert.AreEqual(46, tsTrimLeft.Count);
      Assert.AreEqual(1M, tsTrimLeft[0].Value);
      Assert.AreEqual(null, tsTrimLeft[tsTrimLeft.Count - 1].Value);

      // trim right
      var tsTrimRight = ts.Trim(false);

      Assert.AreEqual(46, tsTrimRight.Count);
      Assert.AreEqual(null, tsTrimRight[0].Value);
      Assert.AreEqual(1M, tsTrimRight[tsTrimRight.Count - 1].Value);

      // trim only 0 values
      var tsZeros = TimeSeries.Factory.FromValue(0, new DateTimeOffset(new DateTime(2021, 01, 01)), 48, Frequency.Hours);

      var tsTrimZeros = tsZeros.Trim(valuesToTrim: new decimal?[] {0M});
      Assert.AreEqual(0, tsTrimZeros.Count);
    }

    [TestMethod]
    public void Test_Operator_Equals()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);

      Assert.IsTrue(ts1 == ts2);
      Assert.IsFalse(ts1 != ts2);
    }

    [TestMethod]
    public void Test_Operator_Equals_Null()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);

      Assert.IsTrue(ts1 != null);
      Assert.IsTrue(null != ts1);
      Assert.IsFalse(ts1 == null);
      Assert.IsFalse(null == ts1);
      Assert.IsFalse((TimeSeries) null != null);
    }

    [TestMethod]
    public void Test_Operator_Add()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_expected = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_actual = ts1 + ts1;

      Assert.AreEqual(ts_expected, ts_actual);
      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);

      Assert.AreNotEqual(ts1, ts_actual);
      Assert.IsFalse(ts1 == ts_actual);
      Assert.IsTrue(ts1 != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_AddEmpty()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_expected = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_actual = ts1 + TimeSeries.Factory.Empty(Frequency.Days);

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_AddTwoEmpty()
    {
      var ts_actual = TimeSeries.Factory.Empty(Frequency.Days) + TimeSeries.Factory.Empty(Frequency.Days);

      Assert.IsTrue(ts_actual == TimeSeries.Factory.Empty(Frequency.Days));
    }

    [TestMethod]
    public void Test_Operator_Subtract()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Frequency.Days);
      var ts2 = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_expected = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 5, Frequency.Days) +
                        TimeSeries.Factory.FromValue(-1, new DateTime(2001, 01, 06), 5, Frequency.Days);
      var ts_actual = ts1 - ts2;

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_Multiply()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Frequency.Days);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_expected = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Frequency.Days) +
                        TimeSeries.Factory.FromValue(0, new DateTime(2001, 01, 06), 5, Frequency.Days);
      var ts_actual = ts1 * ts2;

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_Divide()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Frequency.Days);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 10, Frequency.Days);
      var ts_expected = TimeSeries.Factory.FromValue(1, new DateTime(2001, 01, 01), 5, Frequency.Days)
                        + TimeSeries.Factory.FromValue(0, new DateTime(2001, 01, 06), 5, Frequency.Days);
      var ts_actual = ts1 / ts2;

      Assert.IsTrue(ts_expected == ts_actual);
      Assert.IsFalse(ts_expected != ts_actual);
    }

    [TestMethod]
    public void Test_Operator_Numbers()
    {
      var ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Frequency.Days);
      var ts_expected = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Frequency.Days);
      var ts_actual = ts1 * 2;

      Assert.IsTrue(ts_expected == ts_actual);

      ts1 = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Frequency.Days);
      ts_expected = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Frequency.Days);
      ts_actual = ts1 + 2;

      Assert.IsTrue(ts_expected == ts_actual);

      ts1 = TimeSeries.Factory.FromValue(4, new DateTime(2001, 01, 01), 5, Frequency.Days);
      ts_expected = TimeSeries.Factory.FromValue(2, new DateTime(2001, 01, 01), 5, Frequency.Days);
      ts_actual = ts1 - 2;

      Assert.IsTrue(ts_expected == ts_actual);
    }

    [TestMethod]
    public void Test_Join_2()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts3 = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), 10, Frequency.Days);

      var ts_expected = TimeSeries.Factory.FromValue(6, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts_actual = ts1.JoinFull(ts2, ts3, (x1, x2, x3) => x1 + x2 + x3);

      Assert.IsTrue(ts_actual == ts_expected);
    }

    [TestMethod]
    public void Test_Join_3()
    {
      var ts1 = TimeSeries.Factory.FromValue(1, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts2 = TimeSeries.Factory.FromValue(2, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts3 = TimeSeries.Factory.FromValue(3, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts4 = TimeSeries.Factory.FromValue(4, new DateTime(2021, 01, 01), 10, Frequency.Days);

      var ts_expected = TimeSeries.Factory.FromValue(10, new DateTime(2021, 01, 01), 10, Frequency.Days);
      var ts_actual = ts1.JoinFull(ts2, ts3, ts4, (x1, x2, x3, x4) => x1 + x2 + x3 + x4);

      Assert.IsTrue(ts_actual == ts_expected);
    }
    
    [TestMethod]
    public void Test_Function()
    {
      var ts = TimeSeries.Factory.FromGenerator(new DateTime(2021, 01, 01), new DateTime(2021, 01, 05), Frequency.Days, x => x.Day);

      Assert.AreEqual(5M, ts.Count);
      Assert.AreEqual(1M, ts[new DateTime(2021, 01, 01)]);
      Assert.AreEqual(2M, ts[new DateTime(2021, 01, 02)]);
      Assert.AreEqual(3M, ts[new DateTime(2021, 01, 03)]);
      Assert.AreEqual(4M, ts[new DateTime(2021, 01, 04)]);
      Assert.AreEqual(5M, ts[new DateTime(2021, 01, 05)]);
    }

    [TestMethod]
    public void Test_ToTsv()
    {
      var ts = TimeSeries.Factory.FromGenerator(new DateTime(2021, 01, 01), new DateTime(2021, 01, 05), Frequency.Days, x => x.Day);

      var expected =
        @"01.01.2021 00:00:00 +01:00	1
02.01.2021 00:00:00 +01:00	2
03.01.2021 00:00:00 +01:00	3
04.01.2021 00:00:00 +01:00	4
05.01.2021 00:00:00 +01:00	5";

      Assert.AreEqual(expected, ts.ToTsv(new CultureInfo("de-DE")));
    }
  }
}