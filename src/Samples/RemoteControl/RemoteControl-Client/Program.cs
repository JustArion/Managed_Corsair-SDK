using Grpc.Net.Client;
using Dawn.Apps.RemoteControlHost;

Console.WriteLine("Starting...");
const string ADDRESS = "http://localhost:2424";

Console.WriteLine($"Connecting to {ADDRESS}");
using var channel = GrpcChannel.ForAddress(ADDRESS);

var client = new CorsairService.CorsairServiceClient(channel);

client.SetGlobal(new ColorMessage { A = 255, R = 255, B = 0, G = 0 });