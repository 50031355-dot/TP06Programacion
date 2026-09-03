document.addEventListener('DOMContentLoaded', function () {
    // --- Minijuego 1: Secuencia ---
    (function () {
        const objetivo = ['1','3','5','2','4'];
        let seleccion = [];
        const botones = document.querySelectorAll('.boton');
        const mensaje = document.getElementById('mensaje');
        const miSecuencia = document.getElementById('miSecuencia');
        const progreso = document.getElementById('progreso');
        const codigoDiv = document.getElementById('codigoDiv');
        const reiniciarBtn = document.getElementById('reiniciar');

        if (!botones || !miSecuencia) return;

        function actualizarUI() {
            miSecuencia.textContent = seleccion.length ? seleccion.join(' → ') : '-';
            const pct = Math.round((seleccion.length / objetivo.length) * 100);
            progreso.style.width = pct + '%';
        }

        function mostrarMensaje(text, type='info'){
            if(!mensaje) return;
            mensaje.className = 'alert mb-3';
            mensaje.classList.add(type === 'error' ? 'alert-danger' : (type === 'success' ? 'alert-success' : 'alert-info'));
            mensaje.textContent = text;
            mensaje.classList.remove('d-none');
        }

        function ocultarMensaje(){ if(mensaje) mensaje.classList.add('d-none'); }

        botones.forEach(b => {
            b.addEventListener('click', () => {
                if (codigoDiv && !codigoDiv.classList.contains('d-none')) return;
                const n = b.dataset.numero;
                if (seleccion.length >= objetivo.length) return;
                seleccion.push(n);
                actualizarUI();

                const idx = seleccion.length - 1;
                if (seleccion[idx] !== objetivo[idx]) {
                    mostrarMensaje('Secuencia incorrecta. Reinicia y prueba otra vez.', 'error');
                    botones.forEach(x => x.disabled = true);
                } else {
                    ocultarMensaje();
                }

                if (seleccion.length === objetivo.length) {
                    // Verificar completa
                    if (seleccion.join('') === objetivo.join('')) {
                        mostrarMensaje('¡Secuencia correcta! Has desbloqueado el código.', 'success');
                        if (codigoDiv) codigoDiv.classList.remove('d-none');
                        botones.forEach(x => x.disabled = true);
                    } else {
                        mostrarMensaje('Secuencia incorrecta. Reinicia y prueba otra vez.', 'error');
                        botones.forEach(x => x.disabled = true);
                    }
                }
            });
        });

        if (reiniciarBtn) reiniciarBtn.addEventListener('click', () => {
            seleccion = [];
            botones.forEach(x => x.disabled = false);
            if (codigoDiv) codigoDiv.classList.add('d-none');
            ocultarMensaje();
            actualizarUI();
        });

        actualizarUI();
    })();

    // --- Minijuego 2: Adivina la palabra (estilo Wordle simplificado) ---
    (function () {
        const filas = Array.from(document.querySelectorAll('.cuadricula .fila'));
        const teclas = Array.from(document.querySelectorAll('.tecla'));
        const btnBorrar = document.getElementById('borrar');
        const btnConfirmar = document.getElementById('confirmar');
        const intentosRestantesEl = document.getElementById('intentosRestantes');
        const palabraActualEl = document.getElementById('palabraActual');
        const mensaje = document.getElementById('mensaje');
        const resultadoDiv = document.getElementById('resultadoDiv');
        const derrotaDiv = document.getElementById('derrrotaDiv');
        const palabraSecretaEl = document.getElementById('palabraSecreta');
        const codigoVictoriaEl = document.getElementById('codigoVictoria');

        if (!filas.length) return;

        const WORDS = ['PLATA','CASAS','ROBOT','MARCO','LIMON','SUELO','AMIGO','FIESTA'];
        const secreto = (WORDS[Math.floor(Math.random()*WORDS.length)] || 'PLATA').toUpperCase();
        const ROWS = 6, COLS = 5;
        let filaIdx = 0;
        let colIdx = 0;
        let grid = Array.from({length: ROWS}, () => Array(COLS).fill(''));
        let intentosRestantes = ROWS;

        function actualizarPalabraActual() {
            const current = grid[filaIdx].map(c => c || '_').join('');
            if (palabraActualEl) palabraActualEl.textContent = current;
            if (intentosRestantesEl) intentosRestantesEl.textContent = intentosRestantes;
        }

        function clearMensaje(){ if(mensaje) mensaje.classList.add('d-none'); }
        function showMensaje(text, type='info'){
            if(!mensaje) return;
            mensaje.className = 'alert mb-3';
            mensaje.classList.add(type === 'error' ? 'alert-danger' : (type === 'success' ? 'alert-success' : 'alert-info'));
            mensaje.textContent = text;
            mensaje.classList.remove('d-none');
        }

        function ponerLetra(letra) {
            if (colIdx >= COLS) return;
            grid[filaIdx][colIdx] = letra;
            const celda = filas[filaIdx].children[colIdx];
            if (celda) celda.textContent = letra;
            colIdx++;
            actualizarPalabraActual();
        }

        function borrarLetra() {
            if (colIdx <= 0) return;
            colIdx--;
            grid[filaIdx][colIdx] = '';
            const celda = filas[filaIdx].children[colIdx];
            if (celda) celda.textContent = '';
            actualizarPalabraActual();
        }

        function confirmar() {
            if (colIdx < COLS) { showMensaje('La palabra debe tener 5 letras.', 'error'); return; }
            const intento = grid[filaIdx].join('');
            // Evaluar
            const secretoArr = secreto.split('');
            const resultado = Array(COLS).fill('absent');
            const temp = secretoArr.slice();

            // primeras pasadas: correct
            for (let i=0;i<COLS;i++){
                if (intento[i] === secreto[i]){ resultado[i] = 'correct'; temp[i] = null; }
            }
            // segundas pasadas: present
            for (let i=0;i<COLS;i++){
                if (resultado[i] === 'correct') continue;
                const idx = temp.indexOf(intento[i]);
                if (idx !== -1){ resultado[i] = 'present'; temp[idx] = null; }
            }

            // aplicar clases en celdas y teclado
            for (let i=0;i<COLS;i++){
                const celda = filas[filaIdx].children[i];
                if (!celda) continue;
                celda.classList.remove('correct','present','absent');
                celda.classList.add(resultado[i]);
            }

            // teclado (no degradar estado: correct > present > absent)
            function setKeyState(tecla, state){
                if(!tecla) return;
                if(tecla.classList.contains('correct')) return; // nunca degradar
                if(tecla.classList.contains('present') && state === 'absent') return; // no degradar
                tecla.classList.remove('correct','present','absent');
                tecla.classList.add(state);
            }
            resultado.forEach((r,i) => {
                const letra = intento[i];
                const tecla = teclas.find(t => t.dataset.letra === letra);
                if (!tecla) return;
                setKeyState(tecla, r);
            });

            if (intento === secreto) {
                // victoria
                if (resultadoDiv) resultadoDiv.classList.remove('d-none');
                if (codigoVictoriaEl) codigoVictoriaEl.textContent = 'GANA-2026';
                clearGame();
                return;
            }

            // siguiente fila
            filaIdx++;
            intentosRestantes--;
            colIdx = 0;
            actualizarPalabraActual();

            if (intentosRestantes <= 0) {
                // derrota
                if (derrotaDiv) derrotaDiv.classList.remove('d-none');
                if (palabraSecretaEl) palabraSecretaEl.textContent = secreto;
                clearGame();
            }
        }

        function clearGame(){
            // deshabilitar teclado
            teclas.forEach(t => t.disabled = true);
            if (btnBorrar) btnBorrar.disabled = true;
            if (btnConfirmar) btnConfirmar.disabled = true;
        }

        teclas.forEach(t => t.addEventListener('click', () => {
            if (resultadoDiv && !resultadoDiv.classList.contains('d-none')) return;
            if (derrotaDiv && !derrotaDiv.classList.contains('d-none')) return;
            ponerLetra((t.dataset.letra||'').toUpperCase());
        }));

        if (btnBorrar) btnBorrar.addEventListener('click', () => { borrarLetra(); });
        if (btnConfirmar) btnConfirmar.addEventListener('click', () => { confirmar(); });

        // Reiniciar desde la vista de derrota
        const reiniciarBtn = document.querySelectorAll('#reiniciar');
        reiniciarBtn.forEach(btn => btn.addEventListener('click', () => location.reload()));

        // Permitir escribir desde teclado físico
        document.addEventListener('keydown', (e) => {
            if (resultadoDiv && !resultadoDiv.classList.contains('d-none')) return;
            if (derrotaDiv && !derrotaDiv.classList.contains('d-none')) return;
            const k = e.key.toUpperCase();
            if (/^[A-Z]$/.test(k)) {
                if (k.length === 1 && k >= 'A' && k <= 'Z') ponerLetra(k);
            } else if (e.key === 'Backspace') {
                borrarLetra();
            } else if (e.key === 'Enter') {
                confirmar();
            }
        });

        actualizarPalabraActual();
    })();
});
