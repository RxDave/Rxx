using System;
using System.Runtime.Serialization;

namespace Rxx.UnitTests
{
  [Serializable]
  internal sealed class RxxMockException : Exception
  {
    public RxxMockException()
    {
    }

    public RxxMockException(string message)
      : base(message)
    {
    }

    public RxxMockException(string message, Exception inner)
      : base(message, inner)
    {
    }

    private RxxMockException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public override string ToString()
    {
      return "Mock Exception: " + Message;
    }
  }
}
