document.addEventListener('DOMContentLoaded', function () {
    // --- Minijuego 1: Secuencia ---
    (function () {
        const objetivo = ['1', '3', '5', '2', '4'];
        let seleccion = [];
        const botones = document.querySelectorAll('.boton');
        const mensaje = document.getElementById('mensaje');
        const miSecuencia = document.getElementById('miSecuencia');
        const progreso = document.getElementById('progreso');
        const codigoDiv = document.getElementById('codigoDiv');
        const codigoInput = document.getElementById('codigoInput');
        const reiniciarBtn = document.getElementById('reiniciar');

        if (!botones.length || !miSecuencia) return;

        function actualizarUI() {
            miSecuencia.textContent = seleccion.length ? seleccion.join(' → ') : '-';
            const pct = Math.round((seleccion.length / objetivo.length) * 100);
            progreso.style.width = pct + '%';
        }

        function mostrarMensaje(text, type = 'info') {
            if (!mensaje) return;
            mensaje.className = 'alert mb-3';
            mensaje.classList.add(type === 'error' ? 'alert-danger' : (type === 'success' ? 'alert-success' : 'alert-info'));
            mensaje.textContent = text;
            mensaje.classList.remove('d-none');
        }

        function ocultarMensaje() {
            if (mensaje) mensaje.classList.add('d-none');
        }

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
                    if (seleccion.join('') === objetivo.join('')) {
                        mostrarMensaje('¡Secuencia correcta! Has desbloqueado el código.', 'success');
                        if (codigoDiv) {
                            codigoDiv.classList.remove('d-none');
                            if (codigoInput) codigoInput.value = '4817';
                        }
                        botones.forEach(x => x.disabled = true);
                    } else {
                        mostrarMensaje('Secuencia incorrecta. Reinicia y prueba otra vez.', 'error');
                        botones.forEach(x => x.disabled = true);
                    }
                }
            });
        });

        if (reiniciarBtn) {
            reiniciarBtn.addEventListener('click', () => {
                seleccion = [];
                botones.forEach(x => x.disabled = false);
                if (codigoDiv) codigoDiv.classList.add('d-none');
                if (codigoInput) codigoInput.value = '';
                ocultarMensaje();
                actualizarUI();
            });
        }

        actualizarUI();
    })();

    // --- Minijuego 2: Adivina la palabra ---
    (function () {
        const filas = Array.from(document.querySelectorAll('.cuadricula .fila'));
        const teclas = Array.from(document.querySelectorAll('.tecla'));
        const btnBorrar = document.getElementById('borrar');
        const btnConfirmar = document.getElementById('confirmar');
        const intentosRestantesEl = document.getElementById('intentosRestantes');
        const palabraActualEl = document.getElementById('palabraActual');
        const mensaje = document.getElementById('mensaje');
        const resultadoDiv = document.getElementById('codigoDiv');
        const derrotaDiv = document.getElementById('derrrotaDiv');
        const palabraSecretaEl = document.getElementById('palabraSecreta');
        const codigoInput = document.getElementById('codigoInput');
        const datosMinijuego2 = document.getElementById('datosMinijuego2');

        if (!filas.length) return;

        const secreto = (datosMinijuego2?.dataset.palabra || 'PETER').toUpperCase();
        const codigoMinijuego = datosMinijuego2?.dataset.codigo || '3025';
        const ROWS = 6, COLS = 5;
        let filaIdx = 0;
        let colIdx = 0;
        let grid = Array.from({ length: ROWS }, () => Array(COLS).fill(''));
        let intentosRestantes = ROWS;

        function actualizarPalabraActual() {
            const current = grid[filaIdx].map(c => c || '_').join(' ');
            if (palabraActualEl) palabraActualEl.textContent = current;
            if (intentosRestantesEl) intentosRestantesEl.textContent = String(intentosRestantes);
        }

        function clearMensaje() {
            if (mensaje) mensaje.classList.add('d-none');
        }

        function showMensaje(text, type = 'info') {
            if (!mensaje) return;
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
            if (colIdx < COLS) {
                showMensaje('La palabra debe tener 5 letras.', 'error');
                return;
            }

            const intento = grid[filaIdx].join('');
            const secretoArr = secreto.split('');
            const resultado = Array(COLS).fill('absent');
            const temp = secretoArr.slice();

            for (let i = 0; i < COLS; i++) {
                if (intento[i] === secreto[i]) {
                    resultado[i] = 'correct';
                    temp[i] = null;
                }
            }

            for (let i = 0; i < COLS; i++) {
                if (resultado[i] === 'correct') continue;
                const idx = temp.indexOf(intento[i]);
                if (idx !== -1) {
                    resultado[i] = 'present';
                    temp[idx] = null;
                }
            }

            for (let i = 0; i < COLS; i++) {
                const celda = filas[filaIdx].children[i];
                if (!celda) continue;
                celda.classList.remove('correct', 'present', 'absent');
                celda.classList.add(resultado[i]);
            }

            function setKeyState(tecla, state) {
                if (!tecla) return;
                if (tecla.classList.contains('correct')) return;
                if (tecla.classList.contains('present') && state === 'absent') return;
                tecla.classList.remove('correct', 'present', 'absent');
                tecla.classList.add(state);
            }

            resultado.forEach((r, i) => {
                const letra = intento[i];
                const tecla = teclas.find(t => t.dataset.letra === letra);
                if (!tecla) return;
                setKeyState(tecla, r);
            });

            if (intento === secreto) {
                if (resultadoDiv) resultadoDiv.classList.remove('d-none');
                if (codigoInput) codigoInput.value = codigoMinijuego;
                clearGame();
                return;
            }

            filaIdx++;
            intentosRestantes--;
            colIdx = 0;

            if (filaIdx < ROWS) {
                actualizarPalabraActual();
            }

            if (intentosRestantes <= 0) {
                if (derrotaDiv) derrotaDiv.classList.remove('d-none');
                if (palabraSecretaEl) palabraSecretaEl.textContent = secreto;
                clearGame();
            }
        }

        function clearGame() {
            teclas.forEach(t => t.disabled = true);
            if (btnBorrar) btnBorrar.disabled = true;
            if (btnConfirmar) btnConfirmar.disabled = true;
        }

        teclas.forEach(t => t.addEventListener('click', () => {
            if (resultadoDiv && !resultadoDiv.classList.contains('d-none')) return;
            if (derrotaDiv && !derrotaDiv.classList.contains('d-none')) return;
            ponerLetra((t.dataset.letra || '').toUpperCase());
        }));

        if (btnBorrar) btnBorrar.addEventListener('click', () => borrarLetra());
        if (btnConfirmar) btnConfirmar.addEventListener('click', () => confirmar());

        const reiniciarBtn = document.querySelectorAll('#reiniciar');
        reiniciarBtn.forEach(btn => btn.addEventListener('click', () => location.reload()));

        document.addEventListener('keydown', (e) => {
            if (resultadoDiv && !resultadoDiv.classList.contains('d-none')) return;
            if (derrotaDiv && !derrotaDiv.classList.contains('d-none')) return;
            const k = e.key.toUpperCase();
            if (/^[A-Z]$/.test(k)) {
                ponerLetra(k);
            } else if (e.key === 'Backspace') {
                borrarLetra();
            } else if (e.key === 'Enter') {
                confirmar();
            }
        });

        actualizarPalabraActual();
    })();

    // --- Minijuego 4: Memoria ---
    (function () {
        const cards = Array.from(document.querySelectorAll('.memory-card'));
        const memoryBoard = document.getElementById('memoryBoard');
        const mensaje = document.getElementById('mensaje');
        const codigoDiv = document.getElementById('codigoDiv');
        const codigoInput = document.getElementById('codigoInput');
        const reiniciarBtn = document.getElementById('reiniciarMemoria');
        const codigoMinijuego = 3025;

        if (!cards.length || !memoryBoard) return;

        let flippedCards = [];
        let lockBoard = false;
        let matchedPairs = 0;

        function mezclar(array) {
            for (let i = array.length - 1; i > 0; i--) {
                const j = Math.floor(Math.random() * (i + 1));
                [array[i], array[j]] = [array[j], array[i]];
            }
            return array;
        }

        function mostrarMensaje(text, type = 'info') {
            if (!mensaje) return;
            mensaje.className = 'alert mb-3';
            mensaje.classList.add(type === 'error' ? 'alert-danger' : (type === 'success' ? 'alert-success' : 'alert-info'));
            mensaje.textContent = text;
            mensaje.classList.remove('d-none');
        }

        function ocultarMensaje() {
            if (mensaje) mensaje.classList.add('d-none');
        }

        function resetBoard() {
            const shuffledCards = mezclar([...cards]);
            memoryBoard.innerHTML = '';
            shuffledCards.forEach(card => memoryBoard.appendChild(card));

            cards.forEach(card => {
                card.classList.remove('is-flipped', 'is-matched');
                card.disabled = false;
            });

            flippedCards = [];
            matchedPairs = 0;
            lockBoard = false;
            ocultarMensaje();
            if (codigoDiv) codigoDiv.classList.add('d-none');
            if (codigoInput) codigoInput.value = '';
        }

        function ganarJuego() {
            if (codigoDiv) {
                codigoDiv.classList.remove('d-none');
                if (codigoInput) codigoInput.value = String(codigoMinijuego);
            }
            mostrarMensaje('¡Perfecto! Encontraste todas las parejas.', 'success');
        }

        function evaluarPareja() {
            const [first, second] = flippedCards;

            if (first.dataset.symbol === second.dataset.symbol) {
                first.classList.add('is-matched');
                second.classList.add('is-matched');
                first.disabled = true;
                second.disabled = true;
                matchedPairs++;

                if (matchedPairs === cards.length / 2) {
                    ganarJuego();
                }

                flippedCards = [];
                lockBoard = false;
                return;
            }

            mostrarMensaje('No coinciden. Intenta otra vez.', 'error');
            lockBoard = true;

            setTimeout(() => {
                first.classList.remove('is-flipped');
                second.classList.remove('is-flipped');
                flippedCards = [];
                lockBoard = false;
                ocultarMensaje();
            }, 800);
        }

        cards.forEach(card => {
            card.addEventListener('click', () => {
                if (lockBoard || card.classList.contains('is-flipped') || card.classList.contains('is-matched')) return;

                card.classList.add('is-flipped');
                flippedCards.push(card);

                if (flippedCards.length === 2) {
                    evaluarPareja();
                }
            });
        });

        if (reiniciarBtn) {
            reiniciarBtn.addEventListener('click', () => resetBoard());
        }

        resetBoard();
    })();
});

function darVuelta(carta) {
    carta.classList.toggle('volteada');
}
