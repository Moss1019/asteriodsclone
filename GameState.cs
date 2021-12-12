
using System.Collections.Generic;
using System.Text.Json;

namespace WebSockServer
{
    internal class GameState
    { 
        public List<Asteriod> Asteriods { get; set; } = new List<Asteriod>();

        public List<Ship> Ships { get; set; } = new List<Ship>();

        public List<Bullet> Bullets { get; set; } = new List<Bullet>();

        public string ToJson()
        { 
            return JsonSerializer.Serialize(this);
        }
    }
}
