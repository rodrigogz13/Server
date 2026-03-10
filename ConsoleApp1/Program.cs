
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

class ChatServer
{
    static TcpListener? server;

    static List<TcpClient> clients = new List<TcpClient>();



    static void Main()
    {
        server = new TcpListener(IPAddress.Any, 5000);
        server.Start();
        Console.WriteLine("Chat server started on port 5000.");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            
            Console.WriteLine("New client connected.");

            Thread t = new Thread(HandleClient);
            t.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            int bytesRead = stream.Read(buffer, 0 , buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Console.WriteLine("Mensaje: " + message);

            Broadcast(message);
        }
    }

    static void Broadcast(string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);

        foreach (var client in clients.ToList())
        {
            try
            {
                NetworkStream stream = client.GetStream();
                stream.Write(buffer, 0, buffer.Length);
            }
            catch
            {
                clients.Remove(client);
            }
        }
    }

}
