using System;

namespace ServerConfiguration
{
    /// <summary>
    /// Immutable server configuration class.
    /// Values are set at creation time and cannot be modified.
    /// Uses the Step Builder pattern to enforce mandatory parameters and readability.
    /// </summary>
    public sealed class ServerConfig
    {
        // Mandatory parameters
        public string Address { get; }
        public int Port { get; }

        // Optional parameters (with defaults)
        public bool UseEncryption { get; }
        public int MaxConnections { get; }
        public int TimeoutSeconds { get; }
        public bool LoggingEnabled { get; }

        private ServerConfig(Builder builder)
        {
            Address = builder.Address;
            Port = builder.Port;
            UseEncryption = builder.UseEncryption;
            MaxConnections = builder.MaxConnections;
            TimeoutSeconds = builder.TimeoutSeconds;
            LoggingEnabled = builder.LoggingEnabled;
        }

        /// <summary>
        /// Entry point for the fluent builder.
        /// </summary>
        public static IAddressStep ConnectTo(string address) => new Builder(address);

        // --- Step Interfaces ---

        public interface IAddressStep
        {
            IPortStep AtPort(int port);
        }

        public interface IPortStep
        {
            IOptionalSteps WithEncryption();
            IOptionalSteps WithMaxConnections(int maxConnections);
            IOptionalSteps WithTimeout(int seconds);
            IOptionalSteps WithLogging();
            ServerConfig Build();
        }

        public interface IOptionalSteps : IPortStep
        {
        }

        // --- Builder Implementation ---

        private class Builder : IAddressStep, IPortStep, IOptionalSteps
        {
            public string Address { get; }
            public int Port { get; private set; }
            public bool UseEncryption { get; private set; } = false;
            public int MaxConnections { get; private set; } = 100;
            public int TimeoutSeconds { get; private set; } = 30;
            public bool LoggingEnabled { get; private set; } = false;

            public Builder(string address)
            {
                Address = string.IsNullOrWhiteSpace(address) 
                    ? throw new ArgumentException("Address cannot be empty.", nameof(address)) 
                    : address;
            }

            public IPortStep AtPort(int port)
            {
                if (port < 1 || port > 65535)
                    throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535.");
                
                Port = port;
                return this;
            }

            public IOptionalSteps WithEncryption()
            {
                UseEncryption = true;
                return this;
            }

            public IOptionalSteps WithMaxConnections(int maxConnections)
            {
                if (maxConnections <= 0)
                    throw new ArgumentOutOfRangeException(nameof(maxConnections), "Max connections must be positive.");
                
                MaxConnections = maxConnections;
                return this;
            }

            public IOptionalSteps WithTimeout(int seconds)
            {
                if (seconds < 0)
                    throw new ArgumentOutOfRangeException(nameof(seconds), "Timeout cannot be negative.");

                TimeoutSeconds = seconds;
                return this;
            }

            public IOptionalSteps WithLogging()
            {
                LoggingEnabled = true;
                return this;
            }

            public ServerConfig Build() => new ServerConfig(this);
        }

        public override string ToString()
        {
            return $"ServerConfig [Address={Address}, Port={Port}, Encryption={UseEncryption}, " +
                   $"MaxConnections={MaxConnections}, Timeout={TimeoutSeconds}s, Logging={LoggingEnabled}]";
        }
    }
}
