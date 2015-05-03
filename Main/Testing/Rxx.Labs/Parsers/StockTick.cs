using System;

namespace Rxx.Labs.Parsers
{
  public struct StockTick : IEquatable<StockTick>
  {
    #region Public Properties
    public static readonly StockTick Empty = new StockTick();

    public int Value
    {
      get
      {
        return value;
      }
    }

    public int Change
    {
      get
      {
        return change;
      }
    }
    #endregion

    #region Private / Protected
    private readonly int value, change;
    #endregion

    #region Constructors
    public StockTick(int value, int change)
    {
      this.value = value;
      this.change = change;
    }
    #endregion

    #region Methods
    public override int GetHashCode()
    {
      return value.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return obj is StockTick && Equals((StockTick)obj);
    }

    public bool Equals(StockTick other)
    {
      return value == other.value;
    }

    public static bool operator ==(StockTick first, StockTick second)
    {
      return first.Equals(second);
    }

    public static bool operator !=(StockTick first, StockTick second)
    {
      return !first.Equals(second);
    }

    public override string ToString()
    {
      return string.Format(
        System.Globalization.CultureInfo.CurrentCulture,
        "{0,2} ({1:+####;-####;})",
        value,
        change);
    }
    #endregion
  }
}
