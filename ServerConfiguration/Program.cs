using System;
using ServerConfiguration;

var config = ServerConfig.ConnectTo("127.0.0.1")
    .AtPort(8080)
    .WithEncryption()
    .WithMaxConnections(500)
    .WithLogging()
    .Build();

Console.WriteLine("Server started with configuration:");
Console.WriteLine(config);

// config.Port = 90; // Compilation error!
