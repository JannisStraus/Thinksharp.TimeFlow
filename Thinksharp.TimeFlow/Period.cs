﻿namespace Thinksharp.TimeFlow
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;

  public class Period
  {
    public Period(int value, PeriodUnit period)
    {
      if (value <= 0)
      {
        throw new ArgumentException("Frequency value must be greater than 0");
      }

      this.Value = value;
      this.Unit = period;
    }

    public int Value { get; }
    public PeriodUnit Unit { get; }
    public static Period Milliseconds { get; } = new Period(1, PeriodUnit.Millisecond);
    public static Period Seconds { get; } = new Period(1, PeriodUnit.Second);
    public static Period Minutes { get; } = new Period(1, PeriodUnit.Minute);
    public static Period QuarterHour { get; } = new Period(15, PeriodUnit.Minute);
    public static Period Hour { get; } = new Period(1, PeriodUnit.Hour);
    public static Period Day { get; } = new Period(1, PeriodUnit.Day);
    public static Period Month { get; } = new Period(1, PeriodUnit.Month);
    public static Period QuarterYear { get; } = new Period(3, PeriodUnit.Month);
    public static Period Year { get; } = new Period(1, PeriodUnit.Year);

    /// <summary>
    /// Creates an period from 2 time points.
    /// </summary>
    /// <param name="first">The first time point.</param>
    /// <param name="second">The second time point.</param>
    /// <returns>The period that describes the difference between the 2 specified time points.</returns>
    public static Period FromTimePoints(DateTimeOffset first, DateTimeOffset second)
    {
      if (first == second)
      {
        throw new ArgumentException("Can not create period from identical time points.");
      }

      if (first > second)
      {
        var tmp = first;
        first = second;
        second = tmp;        
      }

      if (first.Date.Month == 1 && second.Month == 1 && first.Day == 1 && second.Day == 1 && first.Year < second.Year)
      {
        return new Period(second.Year - first.Year, PeriodUnit.Year);
      }
      if (first.Date.Year == second.Date.Year)
      {
        if (first.Day == 1 && second.Day == 1 && first.Month < second.Month)
        {
          var numberOfMonths = second.Month - first.Month;

          return new Period(numberOfMonths, PeriodUnit.Month);
        }
      }

      var span = second - first;

      if (span.TotalSeconds < 1.0)
      {
        return new Period((int)span.TotalMilliseconds, PeriodUnit.Millisecond);
      }
      if (span.TotalMinutes < 1.0 || span.TotalMinutes % 1 != 0)
      {
        return new Period((int)span.TotalSeconds, PeriodUnit.Second);
      }
      if (span.TotalHours < 1.0 || span.TotalHours % 1 != 0)
      {
        return new Period((int)span.TotalMinutes, PeriodUnit.Minute);
      }
      if (span.TotalDays < 1.0 || span.TotalDays % 1 != 0)
      {
        return new Period((int)span.TotalHours, PeriodUnit.Hour);
      }     

      return new Period(span.Days, PeriodUnit.Day);
    }
    public DateTimeOffset AddPeriod(DateTimeOffset dt, TimeZoneInfo timeZone = null)
      => this.Unit.AddPeriod(dt, this.Value, timeZone);
    public DateTimeOffset SubtractPeriod(DateTimeOffset dt, TimeZoneInfo timeZone = null)
      => this.Unit.AddPeriod(dt, -this.Value, timeZone);

    /// <summary>
    /// Creates an infinite sequence of time points
    /// </summary>
    /// <param name="start">The first time point to use.</param>
    /// <param name="timeZone">The time zone to use.</param>
    /// <returns>
    /// An infinite sequence of time points
    /// </returns>
    public IEnumerable<DateTimeOffset> GenerateTimePointSequence(DateTimeOffset start, TimeZoneInfo timeZone = null)
    {
      if (timeZone == null)
      {
        timeZone = DateHelper.GetDefaultTimeZone();
      }

      var current = start;
      while(true)
      {
        yield return current;
        current = this.AddPeriod(current, timeZone);
      }
    }

    /// <summary>
    /// Creates an infinite sequence of time points
    /// </summary>
    /// <param name="start">The first time point to use.</param>
    /// <param name="timeZone">The time zone to use.</param>
    /// <returns>
    /// An infinite sequence of time points
    /// </returns>
    public IEnumerable<DateTimeOffset> GenerateTimePointSequence(DateTime start, TimeZoneInfo timeZone = null)
      => GenerateTimePointSequence(new DateTimeOffset(start), timeZone);

    public static DateTimeOffset operator +(DateTimeOffset dt, Period period) => period.AddPeriod(dt);
    public static DateTimeOffset operator +(Period period, DateTimeOffset dt) => period.AddPeriod(dt);
    public static DateTimeOffset operator -(DateTimeOffset dt, Period period) => period.SubtractPeriod(dt);

    public static bool operator ==(Period left, Period right) => Compare(left, right, true, false, (l, r) => l.Equals(r));
    public static bool operator !=(Period left, Period right) => Compare(left, right, false, true, (l, r) => !l.Equals(r));
    public static bool operator >(Period left, Period right) => Compare(left, right, false, false, (l, r) => l > r);
    public static bool operator <(Period left, Period right) => Compare(left, right, false, false, (l, r) => l < r);
    public static bool operator >=(Period left, Period right) => Compare(left, right, true, false, (l, r) => l >= r);
    public static bool operator <=(Period left, Period right) => Compare(left, right, true, false, (l, r) => l <= r);

    public override string ToString()
    {
      return $"{this.Value} {this.Unit}";
    }

    private static Regex periodRegex = new Regex(@"^\s*(?<value>[0-9]+)?\s*(?<unit>(ms|s|min|h|d|mth|yr))\s*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static Period Parse(string periodString)
    {
      if (periodString == null)
      {
        throw new ArgumentNullException(nameof(periodString));
      }

      if (Period.TryParse(periodString, out var period))
      {
        return period;
      }

      throw new FormatException($"'{periodString}' is not a valid period.");
    }

    public static bool TryParse(string periodString, out Period period)
    {
      period = null;
      if (periodString == null)
      {
        return false;
      }

      var match = periodRegex.Match(periodString);

      if (!match.Success)
      {
        return false;
      }

      var valueStr = match.Groups["value"].Value;
      var value = string.IsNullOrEmpty(valueStr) ? 1 : int.Parse(valueStr);
      var unit = match.Groups["unit"].Value;

      period = new Period(value, PeriodUnit.Parse(unit));
      return true;
    }

    public override bool Equals(object obj)
    {
      var other = obj as Period;

      if (other is null ||
          this.Unit != other.Unit ||
          this.Value != other.Value)
      {
        return false;
      }

      return true;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        var hashcode = 1430287;
        hashcode = (hashcode * 7302013) ^ this.Unit.GetHashCode();
        hashcode = (hashcode * 7302013) ^ this.Value.GetHashCode();
        return hashcode;
      }
    }

    private static bool Compare(Period left, Period right, bool bothNull, bool oneNull, Func<DateTimeOffset, DateTimeOffset, bool> noneNullFunc)
    {
      var isLeftNull = ReferenceEquals(left, null);
      var isRightNull = ReferenceEquals(right, null);
      if (isLeftNull && isRightNull)
      {
        return bothNull;
      }

      if (isLeftNull || isRightNull)
      {
        return oneNull;
      }

      var leftInterval = left.AddPeriod(DateTimeOffset.MinValue);
      var rightInterval = right.AddPeriod(DateTimeOffset.MinValue);

      return noneNullFunc(leftInterval, rightInterval);
    }
  }
}