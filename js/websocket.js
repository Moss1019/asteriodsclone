
function initWebsocket() {
    s = new WebSocket("ws://127.0.0.1:8080");
    s.onmessage = (msg) => {
        const gameState = JSON.parse(msg.data);
        ships = gameState.Ships.map(s => ({
            position: vecToArr(s.Position),
            direction: vecToArr(s.Direction),
            rotation: s.Rotation,
            thrust: s.Thrust,
            score: s.Score
        }));
        asteriods = gameState.Asteriods.map(a => ({
            position: vecToArr(a.Position)
        }));
        bullets = gameState.Bullets.map(b => ({
            position: vecToArr(b.Position)
        }));
        document.getElementById('asteriods').innerHTML = JSON.stringify(ships.map(s => s.score));
    };
    s.onclose = () => {
        console.log('closing down');
        connected = false;
    };
    s.onopen = () => {
        connected = true;
    }
    window.onbeforeunload = (ev) => {
        s.close();
    }
}

