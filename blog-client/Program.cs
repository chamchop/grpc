﻿using Greet;
using Grpc.Core;
using Prime;
using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace client_exercise
{
    internal class Program
    {
        const string target = "127.0.0.1:50052";

        static async Task Main(string[] args)
        {
            Channel channel = new Channel(target, ChannelCredentials.Insecure);

            await channel.ConnectAsync().ContinueWith((task)  =>
            {
                if (task.Status == TaskStatus.RanToCompletion) 
                    Console.WriteLine("client connected successfully");
            });

            var client = new PrimeNumberService.PrimeNumberServiceClient(channel);

            var request = new PrimeNumberDecompositionRequest()
            {
                Number = 120
            };

            var response = client.PrimeNumberDecomposition(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.PrimeFactor);
                await Task.Delay(200);
            }

            channel.ShutdownAsync().Wait();
            Console.ReadKey();
        }
    }
}
