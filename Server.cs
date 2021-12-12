using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;

namespace WebSockServer
{
    internal class Server
    {
        private static readonly double ASTERIOD_RADIUS = 0.5;
        private static readonly double BULLET_RADIUS = 0.12;
        private static readonly double BULLET_ASTERIOD_IMPACT = Math.Pow(ASTERIOD_RADIUS + BULLET_RADIUS, 2);

        private TcpListener listener = null!;

        private IList<ClientConnection> clients = new List<ClientConnection>();

        private Thread worker = null!;
        private Thread listenWorker = null!;

        private bool running = false;
        private int interval = 1000 / 60;
        private double deltaTime = 0.016;

        private GameState gameState = null!;
        private IDictionary<int, Ship> shipIndices = new Dictionary<int, Ship>();

        public bool Running
        {
            get {  return running; }
        }

        public Server()
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            gameState = new GameState();
        }

        public void Start()
        {
            running = true;
            worker = new Thread(() =>
            {
                while(running)
                {
                    SpawnAsteriods();
                    UpdateAsteriods();
                    UpdateBullets();
                    UpdateShips();
                    CheckCollisions();
                    for (var i = 0; i < clients.Count; ++i)
                    { 
                        clients[i].SendData(gameState.ToJson());
                        if(!clients[i].Running)
                        {
                            gameState.Ships.Remove(shipIndices[clients[i].Id]);
                            shipIndices.Remove(clients[i].Id);
                            clients.RemoveAt(i);
                            --i;
                        }
                        if (clients.Count == 0)
                        {
                            running = false;
                            break;
                        }
                    }
                    Thread.Sleep(interval);
                }
            });
            worker.Start();
            listenWorker = new Thread(() =>
            {
                listener.Start();
                while (running)
                {
                    TcpClient newConnection = null!;
                    try
                    {
                        newConnection = listener.AcceptTcpClient();
                    }
                    catch
                    {
                        continue;
                    }
                    var newClient = new ClientConnection(newConnection, OnReceiveHandler);
                    newClient.Start();
                    clients.Add(newClient);
                    var newShip = new Ship();
                    gameState.Ships.Add(newShip);
                    shipIndices[newClient.Id] = newShip;
                }
            });
            listenWorker.Start();
        }

        public void Stop()
        {
            running = false;
            listener.Stop();
            listenWorker.Join();
            worker.Join();
            foreach(var client in clients)
            {
                client.Stop();
            }
        }

        private void OnReceiveHandler(int id, string msg)
        {
            var command = JsonSerializer.Deserialize<Command>(msg);
            switch(command!.Type)
            {
                case 38:
                    shipIndices[id].Accelerating = true;
                    break;
                case -38:
                    shipIndices[id].Accelerating = false;
                    break;
                case 37:
                    shipIndices[id].Rotating = true;
                    shipIndices[id].RotateDir = 1;
                    break;
                case 39:
                    shipIndices[id].Rotating = true;
                    shipIndices[id].RotateDir = -1;
                    break;
                case -37:
                case -39:
                    shipIndices[id].Rotating = false;
                    shipIndices[id].RotateDir = 0;
                    shipIndices[id].ThrustTimer = 0;
                    break;
                case 32:
                    SpawnBullet(id);
                    break;
                default:
                    break;
            }
        }

        private void UpdateShips()
        {
            for(var i = 0; i < gameState.Ships.Count; ++i)
            {
                gameState.Ships[i].Update(deltaTime);
            }
        }

        private void UpdateBullets()
        {
            for(var i = 0; i < gameState.Bullets.Count; ++i)
            {
                if(!gameState.Bullets[i].Update(deltaTime))
                {
                    gameState.Bullets.RemoveAt(i);
                    --i;
                }
            }
        }

        private void SpawnBullet(int shipId)
        {
            var ship = shipIndices[shipId];
            gameState.Bullets.Add(new Bullet()
            {
                OwnerId = shipId,
                Position = new Vector3(ship.Position),
                Direction = new Vector3(ship.Direction)
            });
        }

        private void UpdateAsteriods()
        {
            for(var i = 0; i < gameState.Asteriods.Count; ++i)
            {
                gameState.Asteriods[i].Update(deltaTime);
            }
        }

        private void SpawnAsteriods()
        {
            if (gameState.Asteriods.Count < 7)
            {
                Random rand = new Random();
                var x = rand.Next(34) - 17;
                var y = rand.Next(10) > 5 ? -17 : 17;
                var position = new Vector3(x, y, 0);
                var direction = position.Direction(new Vector3()).Normalize();
                gameState.Asteriods.Add(new Asteriod()
                {
                    Position = new Vector3(x, y, 0),
                    Direction = direction
                });
            }
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < gameState.Bullets.Count; ++i)
            {
                for (int j = 0; j < gameState.Asteriods.Count; ++j)
                {
                    var distanceBetween = gameState.Bullets[i].Position.DistanceSqr(gameState.Asteriods[j].Position);
                    if (distanceBetween < BULLET_ASTERIOD_IMPACT)
                    {
                        shipIndices[gameState.Bullets[i].OwnerId].Score += 1;
                        gameState.Bullets.RemoveAt(i);
                        gameState.Asteriods.RemoveAt(j);
                        --i;
                        break;
                    }
                }
            }
        }
    }
}
