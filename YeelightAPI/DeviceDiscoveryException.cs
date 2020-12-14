using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;

namespace YeelightAPI
{
  /// <summary>
  /// Exception thrown when device discovery has failed.
  /// </summary>
  public class DeviceDiscoveryException : Exception
  {
    /// <summary>
    /// A collection of <see cref="SocketException"/> objects that are thrown during instantiation or access of a <see cref="Socket"/> instance.
    /// </summary>
    public IEnumerable<SocketException> SocketExceptions { get; }
    /// <summary>
    /// Returns if this exception contains one or more <see cref="SocketException"/> stored in <see cref="SocketExceptions"/> property.
    /// </summary>
    public bool HasSocketException => this.SocketExceptions.Any();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceDiscoveryException"/> class with a specified error message.
    /// </summary>
    /// <param name="message"></param>
    public DeviceDiscoveryException(string message) : base(message) => this.SocketExceptions = new List<SocketException>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceDiscoveryException"/> class with a specified error message and an inner exception.
    /// </summary>
    public DeviceDiscoveryException(string message, Exception innerException) : base(message, innerException) => this.SocketExceptions = new List<SocketException>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceDiscoveryException"/> class with a specified error message and a collection of occurred <see cref="SocketException"/>.
    /// </summary>
    public DeviceDiscoveryException(string message, IEnumerable<SocketException> innerExceptions) : base(message) =>
      this.SocketExceptions = innerExceptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceDiscoveryException"/> class with serialized data.
    /// </summary>
    protected DeviceDiscoveryException(SerializationInfo info, StreamingContext context) : base(info, context) => this.SocketExceptions = new List<SocketException>();

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceDiscoveryException"/> class.
    /// </summary>
    public DeviceDiscoveryException() =>
      this.SocketExceptions = new List<SocketException>();
  }
}
