# Documentación de Lógica y Guía del Tutorial - AmazonCrisis2D

Este documento resume la lógica de juego actual basada en los scripts del proyecto, detalla el sistema de multiplicadores y dificultad por nivel, y explica cómo se calculan las recompensas de Help Points y Puntos de Confianza.

---

## 1. Personajes y Roles Principales

- **Doctor MORI (Jugador)**: Es un veterinario situado en la selva amazónica. Su misión es encontrar animales heridos o enfermos en el mapa, capturarlos de manera segura y llevarlos a su clínica para curarlos mediante tratamientos periódicos.
- **Animales Salvajes**: Se desplazan por la selva. Tienen barras de salud visibles que se desgastan con el tiempo. Sufren de tres afecciones principales:
  1. Deshidratación
  2. Fracturas
  3. Infección

---

## 2. Lógica del Sistema de Juego (Game Loop)

El ciclo de juego se divide en tres fases principales: **Detección y Búsqueda**, **Captura** y **Tratamiento Clínico**.

```mermaid
graph TD
    A[Animal en la Selva] -->|Salud disminuye con el tiempo| B(Estado del Animal)
    B -->|Estrés Alto >= 70| C[Movimiento Rápido / Rango Detección Amplio]
    B -->|Estrés Bajo < 30| D[Movimiento Lento / Autoregeneración]
    A -->|Médico se acerca y captura| E{¿Captura exitosa?}
    E -->|Fallo: 40% si Estrés >= 70| F[Animal escapa +15 Estrés]
    E -->|Éxito| G[Pausa + Mostrar Panel del Animal]
    G -->|Enviar a Clínica| H[Ingreso en Clínica]
    H -->|Esperar temporizador y aplicar tratamientos| I{Tratamientos = 0}
    I -->|Sí| J[Animal Curado: +Help Points y +Confianza]
```

### A. Mecánica de Salud ([HealthAnimals](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Animals/HealthAnimals.cs))
- La salud máxima es de **100 puntos** y disminuye de forma continua por segundo basándose en un temporizador de muerte (`timeToDie`, por defecto 300 segundos o 5 minutos).
- **Desgaste por Estrés**: La velocidad con la que disminuye la salud (`healthDecayPerSecond`) se multiplica según el nivel de estrés del animal:
  - **Estrés Alto (>= 70)**: Se multiplica por **1.8x** (el animal pierde vida casi el doble de rápido).
  - **Estrés Bajo (< 30)**: Se multiplica por **0.3x** (el desgaste se ralentiza significativamente) y, además, si está en libertad, el animal recupera **1.5 HP/segundo**.
- **Muerte**: Si la salud llega a 0, el animal muere, lo cual activa una penalización de confianza al jugador.

### B. Mecánica de Estrés ([AnimalStress](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Animals/AnimalStress.cs))
- El estrés va de **0 a 100 puntos** (comienza en **50** de forma predeterminada).
- **Incremento Natural**: Aumenta automáticamente con el tiempo a un ritmo de `stressIncreaseRate` (**3 puntos por segundo**).
- **Efectos de los Niveles de Estrés**:
  - **Estrés Alto (>= 70)**: 
    - El animal corre **1.6x** más rápido y salta **1.4x** más alto.
    - Su rango de detección del jugador se amplía **1.8x** ([AnimalSensor](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Animals/AnimalSensor.cs)), lo que hace que detecte al doctor desde lejos y huya rápidamente.
  - **Estrés Bajo (< 30)**: 
    - El animal se calma: camina y corre más lento (**0.7x**) y su rango de alerta baja a **0.5x** (fácil de acercarse).
    - Se activa la regeneración de salud de **1.5 HP/segundo**.
  - **Rango Medio (30 a 70)**: Movimiento y comportamiento estándar del animal.

### C. Mecánica de Captura ([CatchPlayer](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Player/CatchPlayer.cs))
- Al estar cerca del animal, el jugador puede intentar capturarlo pulsando la tecla `R` o el botón en pantalla.
- **Probabilidad de Fallo por Estrés**: 
  - Si el estrés es **Alto (>= 70)**, hay una probabilidad de fallo del **40%** (`highStressFailChance = 0.4f`).
  - Si la captura falla, el animal realiza un salto de huida, pasa al estado `RunAway` y su estrés aumenta inmediatamente en **+15 puntos** (`stressOnFailCapture`).
  - Si la captura tiene éxito, el juego se pausa y se abre el panel informativo ([AnimalPanelUI](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Animals/AnimalPanelUI.cs)), que detalla la enfermedad, tratamientos requeridos, recompensas y la probabilidad de reducción de tratamientos basada en el estrés.

### D. Tratamiento Clínico ([ClinicManager](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Clinic/ClinicManager.cs) y [ClinicPanelUI](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Clinic/ClinicPanelUI.cs))
- El jugador envía al animal capturado a la clínica (capacidad máxima: **3 animales**).
- **Influencia del Estrés al Ingresar**:
  - Si el animal ingresa con **Estrés Bajo (< 30)**:
    - **Probabilidad configurable** (70% por defecto) de que el total de tratamientos requeridos se reduzca a **solo 1**.
    - La recompensa en **Help Points** se multiplica por `lowStressHelpPointsMultiplier` (1.5x por defecto = 50% más).
    - La recompensa en **Puntos de Confianza** se multiplica por `lowStressTrustPointsMultiplier` (1.5x por defecto = 50% más).
- **Proceso de Curación**:
  - Cada enfermedad define un tiempo de espera entre tratamientos (`minutesBetweenTreatments`) y una cantidad total de tratamientos (`treatmentCount`).
  - El temporizador de curación corre en segundo plano. Cuando llega a 0, el jugador interactúa con el panel de la clínica para aplicar un tratamiento.
  - Al completar todos los tratamientos, el animal se recupera por completo, se retira de la clínica y otorga al jugador sus **Help Points** y **Puntos de Confianza**.

### E. Confianza y Vidas del Jugador ([TrustManager](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/UI/HUD/TrustManager.cs) y [HealthManager](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/UI/HUD/HealthManager.cs))
- El Doctor cuenta con una barra de confianza de **0 a 100 puntos** (comienza en **100**).
- Cada vez que un animal muere, la confianza disminuye en **2 puntos** (`trustLossPerMonkey`).
- Si la confianza llega a **0**:
  - Se pierde **1 Vida** (resta de `HealthManager.health`).
  - La confianza se restablece a **100**.
  - Si las vidas llegan a 0, ocurre un **Game Over**.

### F. Habilidad de Doble Salto del Personaje ([DoctorMovement](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Player/DoctorMovement.cs))
Para mejorar la exploración del mapa 2D y la captura de animales en ramas altas, el Doctor Mori cuenta con la habilidad de **Doble Salto**:
- El jugador puede presionar el botón de salto en el aire para realizar un segundo salto consecutivo.
- **Control de Caídas**: Si el jugador cae de un borde sin saltar, conserva el segundo salto (salto en el aire), perdiendo el primero.
- **Física Limpia**: En el momento de realizar el doble salto, la velocidad vertical se anula antes de aplicar la fuerza hacia arriba, asegurando un impulso limpio, consistente y satisfactorio independientemente de la velocidad de caída previa.

---

## 3. Sistema de Multiplicadores y Dificultad por Nivel ([LevelDifficultyConfig](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/LevelDifficultyConfig.cs))

Cada nivel del juego posee un GameObject con el componente `LevelDifficultyConfig` que define los parámetros de dificultad y los multiplicadores de recompensa. Esto permite que los diseñadores ajusten la experiencia nivel por nivel desde el Inspector de Unity sin tocar código.

### Cómo Funcionan los Multiplicadores

Todos los multiplicadores siguen la misma lógica: **valorBase × multiplicador**. El multiplicador se configura en `LevelDifficultyConfig` y se aplica en tiempo de ejecución.

| Multiplicador | Campo en Código | Defecto | Efecto |
|---|---|---|---|
| Velocidad de pérdida de salud | `healthDecayMultiplier` | 1.0 | 1.0 = el animal tarda 5 min en morir. 2.0 = tarda la mitad (2.5 min). |
| Velocidad de incremento de estrés | `stressMultiplier` | 1.0 | 1.0 = ritmo normal. 2.0 = el estrés sube al doble de velocidad. |
| Pérdida de confianza por muerte | `trustLossMultiplier` | 1.0 | 1.0 = -2 pts por muerte. 2.0 = -4 pts por muerte. |
| Help Points por estrés bajo | `lowStressHelpPointsMultiplier` | 1.5 | 1.5 = +50% sobre la base. 2.0 = +100% (el doble). |
| Puntos de Confianza por estrés bajo | `lowStressTrustPointsMultiplier` | 1.5 | 1.5 = +50% sobre la base. 2.0 = +100% (el doble). |
| Probabilidad de reducir tratamientos | `lowStressTreatmentsChance` | 0.7 | Probabilidad (0.0 a 1.0) de que los tratamientos se reduzcan a 1. |
| Umbral de estrés bajo | `lowStressThreshold` | 30 | Valor de estrés máximo para considerar "estrés bajo". |

### Ejemplos de Cálculo de Recompensas

Una enfermedad define valores base en `DiseaseData` (ScriptableObject). Cuando el animal ingresa a la clínica con **estrés bajo**, estos valores se multiplican:

> **Ejemplo 1 — Help Points:**
> - Base en enfermedad: `helpPointsReward = 2`
> - Multiplicador del nivel: `lowStressHelpPointsMultiplier = 1.5`
> - Cálculo: **2 × 1.5 = 3** → El jugador recibe 3 Help Points.
> - Si el multiplicador fuera 2.0: **2 × 2.0 = 4** → El jugador recibe 4 Help Points.

> **Ejemplo 2 — Puntos de Confianza:**
> - Base en enfermedad: `trustPoints = 3`
> - Multiplicador del nivel: `lowStressTrustPointsMultiplier = 1.5`
> - Cálculo: **3 × 1.5 = 4.5 → redondeado a 5** → El jugador recibe 5 pts de confianza.
> - Si el multiplicador fuera 2.0: **3 × 2.0 = 6** → El jugador recibe 6 pts de confianza.

> **Ejemplo 3 — Sin estrés bajo (estrés normal o alto):**
> - No se aplica ningún multiplicador. Se entregan los valores base exactos de la enfermedad.
> - Help Points = valor base, Puntos de Confianza = valor base, tratamientos = valor base.

### Factores Adicionales que Afectan la Dificultad

Además de los multiplicadores, `LevelDifficultyConfig` controla por nivel:
- **`spawnInterval`**: Tiempo entre cada aparición de animal (menor = más presión).
- **`maxMonkeys`**: Máximo de animales simultáneos en el mapa.
- **`stressRateOnSilence/Normal/Nervous/Panic`**: Qué tan fuerte afecta cada emoción del jugador al estrés del animal.

### Diferencia entre Multiplicadores de Pérdida y Ganancia

- **Pérdida** (salud, estrés, confianza por muerte): un multiplicador **mayor a 1.0** hace el juego **más difícil** (el animal muere más rápido, pierdes más confianza).
- **Ganancia** (Help Points, Confianza por curación): un multiplicador **mayor a 1.0** hace el juego **más fácil** (mayor recompensa por curar con estrés bajo). Los diseñadores pueden reducir estos multiplicadores en niveles avanzados para aumentar la dificultad.

---

## 4. Integración de Emociones por FFT (Análisis de Voz) - ¡IMPLEMENTADO!

El sistema de audio ([AudioAnalyzer](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/AudioSystem/FFT/AudioAnalyzer.cs)) procesa el micrófono del jugador en tiempo real y detecta cuatro estados emocionales según el nivel de volumen (RMS) y la agresión espectral:

1. **SILENCE (Silencio)**: Sin entrada de voz o volumen por debajo del umbral mínimo.
2. **NORMAL (Voz Normal)**: Hablar de manera tranquila, pausada y dulce.
3. **NERVOUS (Voz Nerviosa)**: Hablar con cierta agitación o volumen intermedio-alto.
4. **PANIC (Pánico)**: Gritos, sonidos agudos o voz muy fuerte/agresiva.

### Mecánica de Modulación de Estrés por Voz (Implementada en [AnimalStress](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Animals/AnimalStress.cs))
Cuando el jugador está en el rango de detección del animal (`IsPlayerNear` es verdadero en [AnimalSensor](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/Animals/AnimalSensor.cs)), la emoción de su voz influye directamente en el estrés del animal de la siguiente manera:

- **NORMAL (Voz suave y calmada)**: **Disminuye el estrés activamente** (`-6 puntos/segundo`). Esto permite bajar el estrés del animal a menos de 30 para facilitar su captura, ralentizar su desgaste de vida, regenerarlo y obtener bonificaciones en la clínica.
- **SILENCE (Silencio)**: Mantiene el incremento natural del estrés (`+3 puntos/segundo`) causado por la incomodidad de su enfermedad.
- **NERVOUS (Voz nerviosa)**: Incrementa el estrés de forma acelerada (`+8 puntos/segundo`).
- **PANIC (Gritos o pánico)**: Dispara el estrés de manera agresiva (`+10 puntos/segundo`), asustando al animal rápidamente y haciendo que huya y sea difícil de curar.

---

## 5. Transición de Nivel en el Tutorial ([TutorialExitTrigger](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Gameplay/TutorialExitTrigger.cs))

Para finalizar el tutorial de forma interactiva cuando el jugador alcance el objetivo de Help Points:
- Un objeto de portal/salida en la escena de Tutorial permanece inactivo y oculto visualmente al inicio del nivel.
- **Activación por Puntos**: En cuanto los Help Points llegan a **100** (a través de [HelpPointsManager](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/UI/HUD/HelpPointsManager.cs)), el portal se vuelve visible y activa su colisionador automáticamente.
- **Transición**: Al acercarse a él y presionar la tecla **"E"**, el script carga la escena del siguiente nivel (`Level1`).

---

## 6. Sistema de Selección de Niveles en el Menú

El flujo de niveles del juego está conectado en el menú principal a través del [LevelManager](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Progression/LevelManager.cs) y [MenuEvents](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Core/MenuEvents.cs).

### Orden de Progresión
El juego sigue la siguiente secuencia de escenas:
1. **Tutorial** (Escena: `Tutorial`)
2. **Nivel 1** (Escena: `Level1`)
3. **Nivel 2** (Escena: `Level2`)
4. **Nivel 4** (Escena: `Level3` - Nivel 3 en archivos, 4 según flujo del jugador)
5. **Final** (Escena: `Final`)

### Acceso Directo Temporal
Para pruebas y desarrollo rápido, se ha modificado el sistema para desactivar el bloqueo de niveles:
- Todos los niveles están configurados como desbloqueados (`unlocked = true`) en [LevelManager](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Progression/LevelManager.cs) por defecto.
- El script [MenuEvents](file:///c:/UnityProjects/AmazonCrisis2D/Assets/Scripts/Core/MenuEvents.cs) contiene un mapeo robusto que asocia los índices de los botones a sus respectivos nombres de escena en Unity para asegurar que se carguen correctamente al hacer clic en el menú.

---

## 7. Guía para Diseñar el Tutorial Interactivo

Para enseñar esta lógica al jugador, proponemos estructurar el tutorial en los siguientes pasos narrados por el Doctor Mori:

1. **Paso 1: Identificar y Acercarse**
   - *Diálogo*: "¡Hola! Soy el Doctor Mori. Hay un monito deshidratado allí adelante. Su barra verde muestra su salud; si se agota, perderemos su confianza. ¡Vamos a acercarnos despacio!"
   - *Objetivo*: Caminar hacia el animal.
2. **Paso 2: Explicar el Estrés y la Voz (Introducción al Micrófono)**
   - *Diálogo*: "¡Mira! Su barra de estrés (barra amarilla sobre él) está subiendo. Si llega a ser muy alta, correrá rápido y será difícil de atrapar. Háblale bonito al micrófono con un tono calmado para tranquilizarlo."
   - *Objetivo*: El jugador debe hablar de manera tranquila en su micrófono (se detecta estado `NORMAL`). El estrés del animal baja a menos de 30.
   - *Feedback Visual*: La barra de estrés brilla en verde, el animal ralentiza su paso y el sprite del HUD del doctor sonríe.
3. **Paso 3: La Captura**
   - *Diálogo*: "¡Excelente! Ahora que está calmado, su estrés es bajo. No hay riesgo de que escape al intentar atraparlo. ¡Presiona 'R' para capturarlo!"
   - *Objetivo*: Presionar `R` cerca del animal.
4. **Paso 4: Ingreso Clínico y Tratamiento**
   - *Diálogo*: "¡Lo tenemos! Al haberlo capturado calmado, obtuvimos un beneficio: ¡solo necesitará un tratamiento rápido en nuestra clínica! Llevémoslo allí."
   - *Objetivo*: Ir a la clínica, abrir el panel con `E` y presionar el botón para aplicar el tratamiento cuando el temporizador esté listo para curarlo.
5. **Paso 5: Completar el Tutorial**
   - *Diálogo*: "¡Buen trabajo! Al curarlo, recuperamos la confianza de la selva y ganamos Puntos de Ayuda. Hemos alcanzado los 100 Help Points. ¡Mira, se ha abierto el portal de salida! Acércate e interactúa con él (presionando 'E') para iniciar tu aventura."
   - *Objetivo*: Dirigirse al portal aparecido e interactuar con 'E' para cargar la escena `Level1`.

