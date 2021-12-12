
function setupWebGl() {
    canvas = document.getElementById('canvas');
    gl = canvas.getContext('experimental-webgl') || canvas.getContext('webgl');
    gl.viewport(0, 0, canvas.clientWidth, canvas.clientHeight);
    gl.clearColor(0.1, 0.1, 0.1, 1.0);
    gl.enable(gl.DEPTH_TEST);

    pMatrix = glMatrix.mat4.create();
    glMatrix.mat4.perspective(pMatrix, 45, canvas.width / canvas.height, 0.1, 100.0);
    vMatrix = glMatrix.mat4.create();
    glMatrix.mat4.translate(vMatrix, vMatrix, [0, 0, -10]);
    mvMatrix = glMatrix.mat4.create();
}

function initProgram() {
    const compileShader = (shaderType, shaderTagId) => {
        const shader = gl.createShader(shaderType);
        const shaderSource = document.getElementById(shaderTagId).innerHTML;
        gl.shaderSource(shader, shaderSource);
        gl.compileShader(shader);
        const compiled = gl.getShaderParameter(shader, gl.COMPILE_STATUS);
        if(!compiled) {
            console.log('error compiling', shaderTagId);
            console.log(gl.getShaderInfoLog(shader));
            gl.deleteShader(shader);
        }
        return shader;
    }
    const vertexShader = compileShader(gl.VERTEX_SHADER, 'shader-vs');
    const fragmentShader = compileShader(gl.FRAGMENT_SHADER, 'shader-fs');
    if(!vertexShader || !fragmentShader) {
        return;
    }
    const program = gl.createProgram();
    gl.attachShader(program, vertexShader);
    gl.attachShader(program, fragmentShader);
    gl.linkProgram(program);
    const linked = gl.getProgramParameter(program, gl.LINK_STATUS);
    if(!linked) {
        console.log('program not linked');
        gl.deleteShader(vertexShader);
        gl.deleteShader(fragmentShader);
        gl.deleteProgram(program);
        return;
    } 
    gl.useProgram(program);
    gl.program = program;
}

function initBuffers() {
    vertexBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);

    indexBuffer = gl.createBuffer();
    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
    gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(indices), gl.STATIC_DRAW);
}

function initLocations() {
    aVertexPosition = gl.getAttribLocation(gl.program, 'aVertexPosition');
    gl.enableVertexAttribArray(aVertexPosition);
    gl.bindBuffer(gl.ARRAY_BUFFER, vertexBuffer);
    gl.vertexAttribPointer(aVertexPosition, 3, gl.FLOAT, false, 0, 0);

    uPMatrix = gl.getUniformLocation(gl.program, 'uPMatrix');
    gl.uniformMatrix4fv(uPMatrix, false, pMatrix);
    uVMatrix = gl.getUniformLocation(gl.program, 'uVMatrix');
    gl.uniformMatrix4fv(uVMatrix, false, vMatrix);
    uMVMatrix = gl.getUniformLocation(gl.program, 'uMVMatrix');
}

function drawScene() {
    gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

    for(let i = 0; i < bullets.length; ++i) {
        const bullet = bullets[i];
        glMatrix.mat4.identity(mvMatrix);
        glMatrix.mat4.translate(mvMatrix, mvMatrix, bullet.position);
        gl.uniformMatrix4fv(uMVMatrix, false, mvMatrix);
        gl.drawElements(gl.LINE_LOOP, 4, gl.UNSIGNED_SHORT, 2 * BULLET_INDEX_OFFSET);
    }

    for (let i = 0; i < ships.length; ++i) {
        const ship = ships[i];
        glMatrix.mat4.identity(mvMatrix);
        glMatrix.mat4.translate(mvMatrix, mvMatrix, ship.position);
        glMatrix.mat4.rotate(mvMatrix, mvMatrix, ship.rotation, [0, 0, 1]);
        gl.uniformMatrix4fv(uMVMatrix, false, mvMatrix);
        gl.drawElements(gl.LINE_LOOP, 3, gl.UNSIGNED_SHORT, 2 * SHIP_INDEX_OFFSET);
        if(ship.thrust) {
            gl.drawElements(gl.LINE_LOOP, 3, gl.UNSIGNED_SHORT, 2 * SHIP_THUSTER_INDEX_OFFSET);
        }
    }

    for(let i = 0; i < asteriods.length; ++i) {
        const asteriod = asteriods[i];
        glMatrix.mat4.identity(mvMatrix);
        glMatrix.mat4.translate(mvMatrix, mvMatrix, asteriod.position);
        gl.uniformMatrix4fv(uMVMatrix, false, mvMatrix);
        gl.drawElements(gl.LINE_LOOP, 13, gl.UNSIGNED_SHORT, 2 * ASTERIOD_INDEX_OFFSET);
    }
}
