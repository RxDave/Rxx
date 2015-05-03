using System.Diagnostics;

namespace Rxx.UnitTests
{
  public sealed class TestTraceSource : TraceSource
  {
    #region Public Properties
    #endregion

    #region Private / Protected
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new instance of the <see cref="TestTraceSource" /> class.
    /// </summary>
    public TestTraceSource()
      : this(SourceLevels.All)
    {
    }

    /// <summary>
    /// Constructs a new instance of the <see cref="TestTraceSource" /> class.
    /// </summary>
    public TestTraceSource(SourceLevels level)
      : base("Test", level)
    {
    }
    #endregion

    #region Methods
    #endregion
  }
}
