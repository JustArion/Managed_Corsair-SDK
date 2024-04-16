using Dawn.Apps.RemoteControlHost;
using Grpc.Net.Client;

Console.WriteLine("Starting...");
// const string ADDRESS = "http://localhost:2424";
const string ADDRESS = "https://split-favorite-our-amongst.trycloudflare.com";

Console.WriteLine($"Connecting to {ADDRESS}");
using var channel = GrpcChannel.ForAddress(ADDRESS);

var client = new Corsair.CorsairClient(channel);

client.SetGlobal(new Color { A = 255, R = 255, B = 0, G = 0 });