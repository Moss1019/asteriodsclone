
const SHIP_INDEX_OFFSET = 0;
const SHIP_THUSTER_INDEX_OFFSET = 3;
const ASTERIOD_INDEX_OFFSET = 6;
const BULLET_INDEX_OFFSET = 19;

let s = null;
let connected = false;

let canvas = null;
let gl = null;

let vertexBuffer = null;
let indexBuffer = null;

let aVertexPosition = null;

let uPMatrix = null;
let uMVMatrix = null;
let uVMatrix = null;

let pMatrix = null;
let vMatrix = null;
let mvMatrix = null;

let connection = null;

let asteriods = [];
let bullets = [];
let ships = [];

let vertices = [
    // ship
    -0.25, -0.5, 0.0,
    0.0, 1.0, 0.0,
    0.25, -0.5, 0.0,

    // ship thruster
    -0.125, -0.5, 0.0,
    0.0, -1.07, 0.0,
    0.125, -0.5, 0.0,

    // asteriod
    -0.1, 0.5, 0.0,
    0.23, 0.3, 0.0,
    0.24, 0.28, 0.0,
    0.5, -0.1, 0.0,
    0.3, -0.4, 0.0,
    0.2, -0.5, 0.0,
    -0.2, -0.5, 0.0,
    -0.4, -0.3, 0.0,
    -0.3, -0.25, 0.0,
    -0.5, 0.0, 0.0,
    -0.4, 0.2, 0.0,
    -0.4, 0.3, 0.0,
    -0.3, 0.5, 0.0,

    // bullet
    0.0, 0.12, 0.0,
    0.12, 0.0, 0.0,
    -0.12, 0.0, 0.0,
    0.0, -0.12, 0.0
]

let indices = [
    // ship
    0, 1, 2,

    // thuster
    3, 4, 5,

    // asteriod
    6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18,

    // bullet
    19, 20, 22, 21
]
