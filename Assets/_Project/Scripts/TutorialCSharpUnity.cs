// =============================================================================
// TUTORIAL COMPLETO DE C# PARA UNITY
// Guía de estudio para examen - Conceptos fundamentales
// =============================================================================
// Este archivo contiene TODOS los conceptos esenciales de C# en Unity
// Úsalo como referencia para prepararte para crear un juego desde cero
// =============================================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

// =============================================================================
// SECCIÓN 1: FUNDAMENTOS DE C#
// =============================================================================

/*
 * VARIABLES Y TIPOS DE DATOS
 * ---------------------------
 * Los tipos básicos que usarás constantemente:
 */

public class FundamentosCS : MonoBehaviour
{
    // TIPOS PRIMITIVOS
    private int puntuacion = 0;              // Números enteros
    private float velocidad = 5.5f;          // Números decimales (nota la 'f' al final)
    private bool estaVivo = true;            // Verdadero o falso
    private string nombreJugador = "Player"; // Texto

    // ARRAYS - Tamaño fijo
    private int[] numeros = new int[5];      // Array de 5 enteros
    private string[] nombres = { "Ana", "Luis", "María" }; // Array inicializado

    // LISTAS - Tamaño dinámico (mucho más flexible)
    private List<GameObject> enemigos = new List<GameObject>();

    // PROPIEDADES - Forma moderna de exponer variables con control
    public int Puntuacion
    {
        get { return puntuacion; }
        set { puntuacion = value; }
    }

    // Propiedad con auto-implementación (más común)
    public int Vidas { get; private set; } = 3; // Lectura pública, escritura privada


    // ENUMS - Define un conjunto de constantes nombradas
    public enum EstadoJuego
    {
        MenuPrincipal,
        Jugando,
        Pausado,
        GameOver
    }

    private EstadoJuego estadoActual = EstadoJuego.MenuPrincipal;

    // MÉTODOS - Bloques de código reutilizables

    // Método simple sin retorno
    private void AumentarPuntuacion(int cantidad)
    {
        puntuacion += cantidad;
        Debug.Log("Puntuación: " + puntuacion);
    }

    // Método con retorno
    private bool TieneVidasRestantes()
    {
        return Vidas > 0;
    }

    // Método con múltiples parámetros
    private void CrearEnemigo(string tipo, Vector3 posicion, float vida)
    {
        Debug.Log($"Creando enemigo {tipo} en {posicion} con {vida} de vida");
    }

    // MODIFICADORES DE ACCESO
    public int publico;        // Accesible desde cualquier script
    private int privado;       // Solo accesible dentro de esta clase
    protected int protegido;   // Accesible en esta clase y clases derivadas
    [SerializeField] private int serializado; // Privado pero visible en Inspector
}

// =============================================================================
// SECCIÓN 2: MONOBEHAVIOUR LIFECYCLE
// =============================================================================
// El ciclo de vida de un componente Unity - CRÍTICO entender el orden

public class LifecycleTutorial : MonoBehaviour
{
    /*
     * AWAKE() - Se ejecuta PRIMERO, antes que cualquier Start()
     * ----------------------------------------------------------
     * Úsalo para:
     * - Inicializar referencias a otros componentes del MISMO GameObject
     * - Inicializar variables que otros scripts necesitarán en su Start()
     * - Configurar el objeto antes de que cualquier otra cosa ocurra
     */
    private void Awake()
    {
        Debug.Log("1. Awake - Primera inicialización");

        // Ejemplo: obtener componentes del mismo GameObject
        // Rigidbody rb = GetComponent<Rigidbody>();

        // Ejemplo: configurar un Singleton
        // if (Instance == null) Instance = this;
    }

    /*
     * ONENABLE() - Se ejecuta cuando el objeto se activa
     * ---------------------------------------------------
     * Úsalo para:
     * - Suscribirte a eventos
     * - Registrar listeners
     */
    private void OnEnable()
    {
        Debug.Log("2. OnEnable - El objeto está activo");
        // Ejemplo: Suscribirse a eventos
        // GameManager.OnGameOver += ManejarGameOver;
    }

    /*
     * START() - Se ejecuta DESPUÉS de todos los Awake(), antes del primer frame
     * --------------------------------------------------------------------------
     * Úsalo para:
     * - Inicializar variables que dependen de otros objetos ya inicializados
     * - Buscar referencias a otros GameObjects (Find, FindObjectOfType)
     * - Setup inicial del juego
     */
    private void Start()
    {
        Debug.Log("3. Start - Inicialización tardía");

        // Ejemplo: buscar otros objetos (costoso, evita hacerlo en Update)
        // GameObject jugador = GameObject.FindWithTag("Player");
        // GameManager manager = FindObjectOfType<GameManager>();
    }

    /*
     * UPDATE() - Se ejecuta CADA FRAME
     * ---------------------------------
     * Úsalo para:
     * - Input del jugador
     * - Movimiento que no sea física
     * - Lógica de juego frame-by-frame
     * - Temporizadores
     *
     * CUIDADO: Se ejecuta ~60 veces por segundo, no hagas operaciones costosas aquí
     */
    private void Update()
    {
        // Ejemplo: Movimiento simple
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * movimientoHorizontal * Time.deltaTime);

        // Ejemplo: Temporizador
        // tiempoRestante -= Time.deltaTime;

        // Time.deltaTime es el tiempo desde el último frame (importante para movimiento suave)
    }

    /*
     * FIXEDUPDATE() - Se ejecuta a intervalos fijos (default: 50 veces/segundo)
     * --------------------------------------------------------------------------
     * Úsalo para:
     * - TODO lo relacionado con física (Rigidbody)
     * - Movimiento con fuerzas
     */
    private void FixedUpdate()
    {
        // Ejemplo: Aplicar fuerza a un Rigidbody
        // rb.AddForce(Vector3.forward * fuerza);
    }

    /*
     * LATEUPDATE() - Se ejecuta DESPUÉS de todos los Update()
     * --------------------------------------------------------
     * Úsalo para:
     * - Cámaras que siguen al jugador (camera follow)
     * - Cualquier cosa que necesite ejecutarse después del movimiento
     */
    private void LateUpdate()
    {
        // Ejemplo: Cámara siguiendo al jugador
        // transform.position = jugador.position + offset;
    }

    /*
     * ONDISABLE() - Se ejecuta cuando el objeto se desactiva
     * -------------------------------------------------------
     * Úsalo para:
     * - Desuscribirte de eventos (IMPORTANTE para evitar errores)
     */
    private void OnDisable()
    {
        Debug.Log("4. OnDisable - El objeto se desactivó");
        // Ejemplo: Desuscribirse de eventos
        // GameManager.OnGameOver -= ManejarGameOver;
    }

    /*
     * ONDESTROY() - Se ejecuta cuando el objeto es destruido
     * -------------------------------------------------------
     * Úsalo para:
     * - Cleanup final
     * - Liberar recursos
     */
    private void OnDestroy()
    {
        Debug.Log("5. OnDestroy - El objeto fue destruido");
        // Ejemplo: Guardar datos antes de destruirse
        // PlayerPrefs.SetInt("Score", puntuacion);
    }
}

// =============================================================================
// SECCIÓN 3: INPUT SYSTEM
// =============================================================================

public class InputTutorial : MonoBehaviour
{
    /*
     * MÉTODO 1: INPUT CLÁSICO (funciona siempre, más simple)
     * -------------------------------------------------------
     */
    private void UpdateInputClasico()
    {
        // TECLAS INDIVIDUALES
        if (Input.GetKeyDown(KeyCode.Space)) // Una vez al presionar
        {
            Debug.Log("Espaciadora presionada");
            // Saltar();
        }

        if (Input.GetKey(KeyCode.W)) // Mientras se mantiene presionada
        {
            Debug.Log("W está siendo presionada");
            // MoverAdelante();
        }

        if (Input.GetKeyUp(KeyCode.Space)) // Una vez al soltar
        {
            Debug.Log("Espaciadora soltada");
        }

        // EJES (configurados en Project Settings > Input Manager)
        float horizontal = Input.GetAxis("Horizontal"); // -1 a 1 (suavizado)
        float vertical = Input.GetAxis("Vertical");     // -1 a 1 (suavizado)

        // Movimiento básico
        Vector3 movimiento = new Vector3(horizontal, 0, vertical);
        transform.Translate(movimiento * 5f * Time.deltaTime);

        // GetAxisRaw devuelve valores sin suavizado (-1, 0, o 1)
        float horizontalRaw = Input.GetAxisRaw("Horizontal");

        // MOUSE
        if (Input.GetMouseButtonDown(0)) // 0 = izquierdo, 1 = derecho, 2 = medio
        {
            Debug.Log("Click izquierdo");
        }

        Vector3 posicionMouse = Input.mousePosition; // Posición en pantalla
    }

    /*
     * MÉTODO 2: NEW INPUT SYSTEM (más moderno, requiere paquete)
     * -----------------------------------------------------------
     * Como se usa en InputController.cs del proyecto
     */
    private GameInputs gameInputs; // Clase auto-generada del .inputactions

    private void Awake()
    {
        // Inicializar el Input System
        gameInputs = new GameInputs();
    }

    private void OnEnable()
    {
        // Habilitar el action map
        gameInputs.Enable();

        // Suscribirse a eventos de input (PATRÓN RECOMENDADO)
        gameInputs.Gameplay.Move.performed += OnMovePerformed;
        gameInputs.Gameplay.Jump.performed += OnJumpPerformed;
    }

    private void OnDisable()
    {
        // CRÍTICO: Desuscribirse para evitar errores
        gameInputs.Gameplay.Move.performed -= OnMovePerformed;
        gameInputs.Gameplay.Jump.performed -= OnJumpPerformed;

        gameInputs.Disable();
    }

    private void OnMovePerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Vector2 movimiento = context.ReadValue<Vector2>();
        Debug.Log($"Movimiento: {movimiento}");

        // Usar el input
        // transform.Translate(new Vector3(movimiento.x, 0, movimiento.y) * velocidad * Time.deltaTime);
    }

    private void OnJumpPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        Debug.Log("¡Salto!");
        // rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
    }
}

// =============================================================================
// SECCIÓN 4: MANIPULACIÓN DE GAMEOBJECTS
// =============================================================================

public class GameObjectTutorial : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private Transform puntoSpawn;

    private GameObject instanciaCreada;

    private void Start()
    {
        EjemplosTransform();
        EjemplosInstanciar();
        EjemplosBuscarObjetos();
    }

    /*
     * TRANSFORM - Controla posición, rotación y escala
     * -------------------------------------------------
     */
    private void EjemplosTransform()
    {
        // POSICIÓN
        transform.position = new Vector3(0, 5, 10);        // Posición mundial
        transform.localPosition = new Vector3(1, 0, 0);    // Posición relativa al padre

        // Mover gradualmente
        transform.Translate(Vector3.forward * 5 * Time.deltaTime); // Mover hacia adelante

        // ROTACIÓN
        transform.rotation = Quaternion.identity;          // Sin rotación (0,0,0)
        transform.eulerAngles = new Vector3(0, 90, 0);     // Rotar 90 grados en Y

        // Rotar gradualmente
        transform.Rotate(Vector3.up * 50 * Time.deltaTime); // Rotar sobre eje Y

        // Mirar hacia un objetivo
        // transform.LookAt(objetivo.position);

        // ESCALA
        transform.localScale = new Vector3(2, 2, 2);       // Doble de tamaño
        transform.localScale = Vector3.one * 0.5f;         // Mitad de tamaño

        // JERARQUÍA
        // transform.parent = otroObjeto.transform;        // Establecer padre
        // transform.SetParent(otroObjeto.transform);      // Alternativa
        // transform.DetachChildren();                     // Quitar todos los hijos

        // Acceder a hijos
        Transform primerHijo = transform.GetChild(0);
        int cantidadHijos = transform.childCount;
    }

    /*
     * INSTANTIATE - Crear objetos en runtime
     * ---------------------------------------
     */
    private void EjemplosInstanciar()
    {
        // Crear objeto simple
        GameObject nuevoObjeto = Instantiate(prefabEnemigo);

        // Crear con posición y rotación
        GameObject enemigo = Instantiate(
            prefabEnemigo,
            new Vector3(0, 0, 5),
            Quaternion.identity
        );

        // Crear como hijo de otro objeto
        GameObject hijo = Instantiate(prefabEnemigo, puntoSpawn);

        // Guardar referencia para destruir después
        instanciaCreada = enemigo;
    }

    /*
     * DESTROY - Eliminar objetos
     * ---------------------------
     */
    private void EjemplosDestruir()
    {
        // Destruir inmediatamente
        Destroy(instanciaCreada);

        // Destruir después de X segundos
        Destroy(instanciaCreada, 3.0f);

        // Destruir solo el componente (no el GameObject completo)
        // Destroy(GetComponent<Rigidbody>());

        // NO destruir entre escenas
        DontDestroyOnLoad(gameObject);
    }

    /*
     * BUSCAR OBJETOS - Encontrar referencias
     * ---------------------------------------
     * CUIDADO: Estas operaciones son COSTOSAS, úsalas solo en Start/Awake
     */
    private void EjemplosBuscarObjetos()
    {
        // Por nombre (evita si es posible)
        GameObject jugador = GameObject.Find("Player");

        // Por tag (más eficiente)
        GameObject enemigo = GameObject.FindGameObjectWithTag("Enemy");
        GameObject[] todosEnemigos = GameObject.FindGameObjectsWithTag("Enemy");

        // Buscar componente en la escena
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        AudioManager[] todosManagers = FindObjectsOfType<AudioManager>();

        // GETCOMPONENT - Obtener componentes del MISMO GameObject
        Rigidbody rb = GetComponent<Rigidbody>();
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        // GetComponent en hijos
        SpriteRenderer spriteHijo = GetComponentInChildren<SpriteRenderer>();

        // GetComponent en padre
        Canvas canvasPadre = GetComponentInParent<Canvas>();
    }

    /*
     * ACTIVAR/DESACTIVAR OBJETOS
     * ---------------------------
     */
    private void EjemplosActivacion()
    {
        // Activar/desactivar
        gameObject.SetActive(false); // Desactivar este GameObject
        gameObject.SetActive(true);  // Activar

        // Verificar si está activo
        bool estaActivo = gameObject.activeSelf;        // Activo en sí mismo
        bool estaActivoEnHierarquia = gameObject.activeInHierarchy; // Activo si padres también lo están
    }
}

// =============================================================================
// SECCIÓN 5: UI CON TEXTMESHPRO
// =============================================================================

public class UITutorial : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoVidas;
    [SerializeField] private Button botonJugar;
    [SerializeField] private Button botonSalir;
    [SerializeField] private Slider sliderVolumen;
    [SerializeField] private Image imagenVida;

    private int puntuacion = 0;
    private int vidas = 3;

    private void Start()
    {
        // CONFIGURAR BOTONES - Asignar métodos a onClick
        botonJugar.onClick.AddListener(OnBotonJugarClick);
        botonSalir.onClick.AddListener(OnBotonSalirClick);

        // CONFIGURAR SLIDER
        sliderVolumen.onValueChanged.AddListener(OnVolumenCambiado);

        // Inicializar UI
        ActualizarUI();
    }

    /*
     * ACTUALIZAR TEXTO
     * ----------------
     */
    private void ActualizarUI()
    {
        // Actualizar texto simple
        textoPuntuacion.text = "Puntuación: " + puntuacion;

        // Con interpolación de strings (más moderno)
        textoVidas.text = $"Vidas: {vidas}";

        // Formatear números
        float tiempo = 123.456f;
        textoVidas.text = $"Tiempo: {tiempo:F2}"; // 2 decimales: "123.46"

        // Cambiar color del texto
        textoPuntuacion.color = Color.yellow;
        textoPuntuacion.color = new Color(1f, 0.5f, 0f); // RGB (naranja)

        // Cambiar tamaño
        textoPuntuacion.fontSize = 48;
    }

    /*
     * MANEJO DE BOTONES
     * -----------------
     */
    private void OnBotonJugarClick()
    {
        Debug.Log("Botón Jugar presionado");
        SceneManager.LoadScene("Game");
    }

    private void OnBotonSalirClick()
    {
        Debug.Log("Botón Salir presionado");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Parar el juego en el editor
        #endif
    }

    /*
     * SLIDERS Y CONTROLES
     * -------------------
     */
    private void OnVolumenCambiado(float valor)
    {
        Debug.Log($"Volumen cambiado a: {valor}");
        // AudioListener.volume = valor;
    }

    /*
     * IMÁGENES
     * --------
     */
    private void EjemplosImagenes()
    {
        // Cambiar color (útil para feedback visual)
        imagenVida.color = Color.red;

        // Cambiar transparencia
        Color colorActual = imagenVida.color;
        colorActual.a = 0.5f; // 50% transparente
        imagenVida.color = colorActual;

        // Cargar sprite dinámicamente
        // Sprite nuevoSprite = Resources.Load<Sprite>("Sprites/corazon");
        // imagenVida.sprite = nuevoSprite;

        // Activar/desactivar imagen
        imagenVida.enabled = false;
    }

    /*
     * EJEMPLO PRÁCTICO: Sistema de vidas con iconos
     * ----------------------------------------------
     */
    [SerializeField] private GameObject prefabCorazon;
    [SerializeField] private Transform contenedorVidas;
    private List<GameObject> iconosVidas = new List<GameObject>();

    private void CrearIconosVidas(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            GameObject corazon = Instantiate(prefabCorazon, contenedorVidas);
            iconosVidas.Add(corazon);
        }
    }

    private void PerderVida()
    {
        if (iconosVidas.Count > 0)
        {
            GameObject ultimoCorazon = iconosVidas[iconosVidas.Count - 1];
            iconosVidas.RemoveAt(iconosVidas.Count - 1);
            Destroy(ultimoCorazon);
        }
    }
}

// =============================================================================
// SECCIÓN 6: COROUTINES
// =============================================================================
// Las Coroutines permiten pausar ejecución y esperar - ESENCIAL para delays

public class CoroutineTutorial : MonoBehaviour
{
    private bool estaAnimando = false;

    private void Start()
    {
        // INICIAR una Coroutine
        StartCoroutine(EjemploBasico());

        // Guardar referencia para detenerla después
        Coroutine miCoroutine = StartCoroutine(ContadorInfinito());

        // Detener una Coroutine específica
        // StopCoroutine(miCoroutine);

        // Detener TODAS las Coroutines de este MonoBehaviour
        // StopAllCoroutines();
    }

    /*
     * COROUTINE BÁSICA - Esperar tiempo
     * ----------------------------------
     */
    private IEnumerator EjemploBasico()
    {
        Debug.Log("Inicio");

        // Esperar 2 segundos
        yield return new WaitForSeconds(2f);

        Debug.Log("Han pasado 2 segundos");

        // Esperar 1 frame
        yield return null;

        Debug.Log("Ha pasado 1 frame");

        // Esperar hasta el final del frame (después de renderizar)
        yield return new WaitForEndOfFrame();

        Debug.Log("Final del frame");
    }

    /*
     * COROUTINE CON LOOP
     * ------------------
     */
    private IEnumerator ContadorInfinito()
    {
        int contador = 0;

        while (true) // Loop infinito (controlado)
        {
            Debug.Log($"Contador: {contador}");
            contador++;

            yield return new WaitForSeconds(1f); // Esperar 1 segundo entre iteraciones
        }
    }

    /*
     * COROUTINE CON CONDICIÓN
     * -----------------------
     */
    private IEnumerator EsperarHastaCondicion()
    {
        Debug.Log("Esperando hasta que termine la animación...");

        // Esperar hasta que se cumpla una condición
        yield return new WaitUntil(() => estaAnimando == false);

        Debug.Log("¡La animación terminó!");

        // También existe WaitWhile (opuesto)
        // yield return new WaitWhile(() => estaAnimando == true);
    }

    /*
     * EJEMPLO PRÁCTICO: Temporizador de cuenta regresiva
     * ---------------------------------------------------
     */
    [SerializeField] private TextMeshProUGUI textoTemporizador;

    private IEnumerator CuentaRegresiva(int segundos)
    {
        for (int i = segundos; i > 0; i--)
        {
            if (textoTemporizador != null)
            {
                textoTemporizador.text = i.ToString();
            }

            Debug.Log($"Cuenta regresiva: {i}");
            yield return new WaitForSeconds(1f);
        }

        if (textoTemporizador != null)
        {
            textoTemporizador.text = "¡GO!";
        }

        Debug.Log("¡Tiempo agotado!");
    }

    /*
     * EJEMPLO PRÁCTICO: Fade de imagen
     * ---------------------------------
     */
    [SerializeField] private Image imagenFade;

    private IEnumerator FadeIn(float duracion)
    {
        float tiempoTranscurrido = 0f;
        Color color = imagenFade.color;
        color.a = 0f; // Empezar transparente

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, tiempoTranscurrido / duracion);
            imagenFade.color = color;

            yield return null; // Esperar 1 frame
        }

        // Asegurar que termine en 1
        color.a = 1f;
        imagenFade.color = color;
    }

    /*
     * EJEMPLO PRÁCTICO: Spawn periódico de enemigos
     * ----------------------------------------------
     */
    [SerializeField] private GameObject prefabEnemigo;
    [SerializeField] private Transform puntoSpawn;

    private IEnumerator SpawnEnemigosRepetido(float intervalo, int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            // Crear enemigo
            Vector3 posicionAleatoria = puntoSpawn.position + new Vector3(
                UnityEngine.Random.Range(-5f, 5f),
                0,
                UnityEngine.Random.Range(-5f, 5f)
            );

            Instantiate(prefabEnemigo, posicionAleatoria, Quaternion.identity);

            Debug.Log($"Enemigo {i + 1}/{cantidad} spawneado");

            // Esperar antes del siguiente
            yield return new WaitForSeconds(intervalo);
        }

        Debug.Log("Spawn completado");
    }

    /*
     * EJEMPLO PRÁCTICO: Game Over con delay (como en InputController.cs)
     * -------------------------------------------------------------------
     */
    private IEnumerator MostrarGameOverConDelay(float delay)
    {
        Debug.Log("Juego terminado, esperando antes de mostrar pantalla...");

        yield return new WaitForSeconds(delay);

        // Cambiar a escena de Game Over
        SceneManager.LoadScene("GameOver");
    }
}

// =============================================================================
// SECCIÓN 7: EVENTOS Y DELEGATES
// =============================================================================
// Sistema de comunicación entre scripts sin acoplamiento directo

public class EventosTutorial : MonoBehaviour
{
    /*
     * PATRÓN 1: SYSTEM.ACTION (más común y simple)
     * --------------------------------------------
     */

    // Evento simple sin parámetros
    public static event Action OnGameStart;

    // Evento con 1 parámetro
    public static event Action<int> OnScoreChanged; // Pasa la nueva puntuación

    // Evento con múltiples parámetros
    public static event Action<int, string> OnPlayerDamaged; // Pasa daño y tipo

    // Evento con múltiples tipos
    public static event Action<Vector3, GameObject> OnEnemySpawned;

    private int puntuacion = 0;

    private void Start()
    {
        // DISPARAR (invoke) un evento
        OnGameStart?.Invoke(); // El '?' es seguro - solo invoca si hay suscriptores

        // Disparar evento con parámetros
        puntuacion = 100;
        OnScoreChanged?.Invoke(puntuacion);

        OnPlayerDamaged?.Invoke(25, "Fire");
    }

    /*
     * SUSCRIBIRSE Y DESUSCRIBIRSE (PATRÓN CRÍTICO)
     * ---------------------------------------------
     */
    private void OnEnable()
    {
        // SUSCRIBIRSE a eventos (registrar listener)
        OnGameStart += ManejarInicioJuego;
        OnScoreChanged += ManejarCambioPuntuacion;
        OnPlayerDamaged += ManejarDañoJugador;
    }

    private void OnDisable()
    {
        // DESUSCRIBIRSE (CRÍTICO para evitar errores de null reference)
        OnGameStart -= ManejarInicioJuego;
        OnScoreChanged -= ManejarCambioPuntuacion;
        OnPlayerDamaged -= ManejarDañoJugador;
    }

    // Métodos que responden a eventos
    private void ManejarInicioJuego()
    {
        Debug.Log("¡El juego ha comenzado!");
    }

    private void ManejarCambioPuntuacion(int nuevaPuntuacion)
    {
        Debug.Log($"Nueva puntuación: {nuevaPuntuacion}");
        // Actualizar UI, guardar en PlayerPrefs, etc.
    }

    private void ManejarDañoJugador(int daño, string tipo)
    {
        Debug.Log($"Jugador recibió {daño} de daño de tipo {tipo}");
    }

    /*
     * EJEMPLO PRÁCTICO: Sistema de puntuación con eventos
     * ----------------------------------------------------
     */
}

// Clase que GENERA eventos
public class ScoreManager : MonoBehaviour
{
    public static event Action<int> OnScoreUpdated;
    public static event Action OnHighScoreBeaten;

    private int puntuacionActual = 0;
    private int mejorPuntuacion = 0;

    public void AñadirPuntos(int cantidad)
    {
        puntuacionActual += cantidad;

        // Notificar a todos los suscriptores
        OnScoreUpdated?.Invoke(puntuacionActual);

        // Verificar record
        if (puntuacionActual > mejorPuntuacion)
        {
            mejorPuntuacion = puntuacionActual;
            OnHighScoreBeaten?.Invoke();
        }
    }
}

// Clase que ESCUCHA eventos
public class UIScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textoPuntuacion;

    private void OnEnable()
    {
        ScoreManager.OnScoreUpdated += ActualizarDisplay;
        ScoreManager.OnHighScoreBeaten += MostrarNuevoRecord;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreUpdated -= ActualizarDisplay;
        ScoreManager.OnHighScoreBeaten -= MostrarNuevoRecord;
    }

    private void ActualizarDisplay(int nuevaPuntuacion)
    {
        textoPuntuacion.text = $"Puntuación: {nuevaPuntuacion}";
    }

    private void MostrarNuevoRecord()
    {
        Debug.Log("¡NUEVO RÉCORD!");
        // Mostrar animación, sonido, etc.
    }
}

// =============================================================================
// SECCIÓN 8: SINGLETON PATTERN
// =============================================================================
// Patrón para tener UNA SOLA INSTANCIA de una clase accesible globalmente

public class SingletonTutorial : MonoBehaviour
{
    /*
     * IMPLEMENTACIÓN BÁSICA DE SINGLETON
     * -----------------------------------
     */

    // Referencia estática a la única instancia
    public static SingletonTutorial Instance { get; private set; }

    private void Awake()
    {
        // Si ya existe una instancia y no soy yo, destruirme
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Soy la primera instancia
        Instance = this;

        // NO destruir al cambiar de escena
        DontDestroyOnLoad(gameObject);
    }

    // Ahora cualquier script puede acceder a este singleton:
    // SingletonTutorial.Instance.MiMetodo();
}

/*
 * EJEMPLO COMPLETO: GameManager Singleton
 * ----------------------------------------
 */
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Eventos del juego
    public static event Action OnGameStarted;
    public static event Action OnGamePaused;
    public static event Action OnGameOver;

    // Estado del juego
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState EstadoActual { get; private set; } = GameState.MainMenu;

    // Estadísticas
    public int Puntuacion { get; private set; } = 0;
    public int Vidas { get; private set; } = 3;
    public float TiempoJugado { get; private set; } = 0f;

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // Contar tiempo solo si está jugando
        if (EstadoActual == GameState.Playing)
        {
            TiempoJugado += Time.deltaTime;
        }
    }

    // Métodos públicos para controlar el juego
    public void IniciarJuego()
    {
        EstadoActual = GameState.Playing;
        Puntuacion = 0;
        Vidas = 3;
        TiempoJugado = 0f;

        OnGameStarted?.Invoke();
        Debug.Log("Juego iniciado");
    }

    public void PausarJuego()
    {
        EstadoActual = GameState.Paused;
        Time.timeScale = 0f; // Pausar el tiempo del juego

        OnGamePaused?.Invoke();
        Debug.Log("Juego pausado");
    }

    public void ReanudarJuego()
    {
        EstadoActual = GameState.Playing;
        Time.timeScale = 1f; // Restaurar tiempo normal
        Debug.Log("Juego reanudado");
    }

    public void AñadirPuntos(int cantidad)
    {
        Puntuacion += cantidad;
        Debug.Log($"Puntuación: {Puntuacion}");
    }

    public void PerderVida()
    {
        Vidas--;
        Debug.Log($"Vidas restantes: {Vidas}");

        if (Vidas <= 0)
        {
            TerminarJuego();
        }
    }

    public void TerminarJuego()
    {
        EstadoActual = GameState.GameOver;
        Time.timeScale = 1f; // Restaurar timeScale

        OnGameOver?.Invoke();
        Debug.Log($"Game Over - Puntuación final: {Puntuacion}");

        // Guardar mejor puntuación
        int mejorPuntuacion = PlayerPrefs.GetInt("BestScore", 0);
        if (Puntuacion > mejorPuntuacion)
        {
            PlayerPrefs.SetInt("BestScore", Puntuacion);
            PlayerPrefs.Save();
        }
    }
}

// Ejemplo de uso desde otro script:
public class EjemploUsoGameManager : MonoBehaviour
{
    private void Start()
    {
        // Acceder al GameManager desde cualquier lugar
        GameManager.Instance.IniciarJuego();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.PerderVida();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            GameManager.Instance.AñadirPuntos(10);
            Destroy(other.gameObject);
        }
    }
}

// =============================================================================
// SECCIÓN 9: EJEMPLO COMPLETO - JUEGO DE RECOLECCIÓN DE MONEDAS
// =============================================================================
// Este ejemplo integra TODOS los conceptos anteriores en un juego funcional

/*
 * JUEGO: RECOLECTOR DE MONEDAS
 * -----------------------------
 * - Jugador se mueve con WASD
 * - Monedas aparecen aleatoriamente cada 2 segundos
 * - Recoger monedas aumenta puntuación
 * - Tiempo límite de 60 segundos
 * - Game Over cuando se acaba el tiempo
 * - Sistema de eventos para comunicación entre componentes
 */

// ============= GAME MANAGER =============
public class CoinGameManager : MonoBehaviour
{
    // Singleton
    public static CoinGameManager Instance { get; private set; }

    // Eventos
    public static event Action<int> OnCoinCollected;
    public static event Action<float> OnTimeUpdated;
    public static event Action<int> OnGameOver;

    // Estado del juego
    public int Puntuacion { get; private set; } = 0;
    public float TiempoRestante { get; private set; } = 60f;
    public bool JuegoActivo { get; private set; } = false;

    [Header("Configuración")]
    [SerializeField] private float tiempoLimite = 60f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        IniciarJuego();
    }

    private void Update()
    {
        if (!JuegoActivo) return;

        // Actualizar temporizador
        TiempoRestante -= Time.deltaTime;
        OnTimeUpdated?.Invoke(TiempoRestante);

        // Verificar fin del juego
        if (TiempoRestante <= 0f)
        {
            TiempoRestante = 0f;
            TerminarJuego();
        }
    }

    public void IniciarJuego()
    {
        Puntuacion = 0;
        TiempoRestante = tiempoLimite;
        JuegoActivo = true;

        Debug.Log("¡Juego iniciado! Recoge todas las monedas que puedas en 60 segundos");
    }

    public void RecolectarMoneda(int valor)
    {
        if (!JuegoActivo) return;

        Puntuacion += valor;
        OnCoinCollected?.Invoke(Puntuacion);

        Debug.Log($"¡Moneda recolectada! Puntuación: {Puntuacion}");
    }

    private void TerminarJuego()
    {
        JuegoActivo = false;
        OnGameOver?.Invoke(Puntuacion);

        Debug.Log($"¡JUEGO TERMINADO! Puntuación final: {Puntuacion}");

        // Guardar mejor puntuación
        int mejorPuntuacion = PlayerPrefs.GetInt("CoinGameBestScore", 0);
        if (Puntuacion > mejorPuntuacion)
        {
            PlayerPrefs.SetInt("CoinGameBestScore", Puntuacion);
            Debug.Log("¡NUEVO RÉCORD!");
        }
    }

    public void ReiniciarJuego()
    {
        IniciarJuego();
    }
}

// ============= PLAYER CONTROLLER =============
public class CoinPlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float velocidadRotacion = 720f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sonidoRecoleccion;

    private Vector3 movimiento;

    private void Update()
    {
        if (!CoinGameManager.Instance.JuegoActivo) return;

        // Capturar input (Input clásico)
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D o Flechas
        float vertical = Input.GetAxisRaw("Vertical");     // W/S o Flechas

        // Crear vector de movimiento
        movimiento = new Vector3(horizontal, 0f, vertical).normalized;

        // Mover el jugador
        if (movimiento.magnitude > 0.1f)
        {
            transform.Translate(movimiento * velocidad * Time.deltaTime, Space.World);

            // Rotar hacia la dirección de movimiento
            Quaternion rotacionObjetivo = Quaternion.LookRotation(movimiento);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }
    }

    // Detectar colisión con monedas
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            // Obtener el componente Coin
            Coin moneda = other.GetComponent<Coin>();
            if (moneda != null)
            {
                moneda.Recolectar();

                // Reproducir sonido
                if (audioSource != null && sonidoRecoleccion != null)
                {
                    audioSource.PlayOneShot(sonidoRecoleccion);
                }
            }
        }
    }
}

// ============= COIN (Moneda) =============
public class Coin : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int valor = 10;
    [SerializeField] private float velocidadRotacion = 90f;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem particulasRecoleccion;

    private bool recolectada = false;

    private void Update()
    {
        // Rotar la moneda para que se vea llamativa
        transform.Rotate(Vector3.up, velocidadRotacion * Time.deltaTime);
    }

    public void Recolectar()
    {
        if (recolectada) return;
        recolectada = true;

        // Notificar al GameManager
        CoinGameManager.Instance.RecolectarMoneda(valor);

        // Efecto de partículas
        if (particulasRecoleccion != null)
        {
            Instantiate(particulasRecoleccion, transform.position, Quaternion.identity);
        }

        // Destruir la moneda
        Destroy(gameObject);
    }
}

// ============= COIN SPAWNER =============
public class CoinSpawner : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GameObject prefabMoneda;

    [Header("Configuración")]
    [SerializeField] private float intervaloSpawn = 2f;
    [SerializeField] private int maxMonedasSimultaneas = 5;
    [SerializeField] private Vector2 rangoX = new Vector2(-8f, 8f);
    [SerializeField] private Vector2 rangoZ = new Vector2(-8f, 8f);
    [SerializeField] private float alturaSpawn = 0.5f;

    private List<GameObject> monedasActivas = new List<GameObject>();
    private Coroutine spawnCoroutine;

    private void OnEnable()
    {
        // Suscribirse a eventos
        CoinGameManager.OnGameOver += DetenerSpawn;
    }

    private void OnDisable()
    {
        // Desuscribirse
        CoinGameManager.OnGameOver -= DetenerSpawn;
    }

    private void Start()
    {
        // Iniciar spawn de monedas
        spawnCoroutine = StartCoroutine(SpawnMonedasContinuo());
    }

    private IEnumerator SpawnMonedasContinuo()
    {
        while (true)
        {
            // Limpiar referencias nulas (monedas recolectadas)
            monedasActivas.RemoveAll(m => m == null);

            // Spawnear si no hemos llegado al límite
            if (monedasActivas.Count < maxMonedasSimultaneas)
            {
                SpawnearMoneda();
            }

            // Esperar antes del siguiente spawn
            yield return new WaitForSeconds(intervaloSpawn);
        }
    }

    private void SpawnearMoneda()
    {
        // Posición aleatoria
        Vector3 posicion = new Vector3(
            UnityEngine.Random.Range(rangoX.x, rangoX.y),
            alturaSpawn,
            UnityEngine.Random.Range(rangoZ.x, rangoZ.y)
        );

        // Crear moneda
        GameObject moneda = Instantiate(prefabMoneda, posicion, Quaternion.identity);
        monedasActivas.Add(moneda);

        Debug.Log($"Moneda spawneada en {posicion}");
    }

    private void DetenerSpawn(int puntuacionFinal)
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        Debug.Log("Spawn de monedas detenido");
    }
}

// ============= UI MANAGER =============
public class CoinGameUIManager : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private TextMeshProUGUI textoPuntuacion;
    [SerializeField] private TextMeshProUGUI textoTiempo;
    [SerializeField] private GameObject panelGameOver;
    [SerializeField] private TextMeshProUGUI textoPuntuacionFinal;
    [SerializeField] private TextMeshProUGUI textoMejorPuntuacion;
    [SerializeField] private Button botonReiniciar;

    private void OnEnable()
    {
        // Suscribirse a eventos del GameManager
        CoinGameManager.OnCoinCollected += ActualizarPuntuacion;
        CoinGameManager.OnTimeUpdated += ActualizarTiempo;
        CoinGameManager.OnGameOver += MostrarGameOver;
    }

    private void OnDisable()
    {
        // Desuscribirse
        CoinGameManager.OnCoinCollected -= ActualizarPuntuacion;
        CoinGameManager.OnTimeUpdated -= ActualizarTiempo;
        CoinGameManager.OnGameOver -= MostrarGameOver;
    }

    private void Start()
    {
        // Configurar botón
        if (botonReiniciar != null)
        {
            botonReiniciar.onClick.AddListener(OnBotonReiniciarClick);
        }

        // Ocultar panel de Game Over
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        // Inicializar UI
        ActualizarPuntuacion(0);
        ActualizarTiempo(60f);
    }

    private void ActualizarPuntuacion(int nuevaPuntuacion)
    {
        if (textoPuntuacion != null)
        {
            textoPuntuacion.text = $"Puntuación: {nuevaPuntuacion}";
        }
    }

    private void ActualizarTiempo(float tiempoRestante)
    {
        if (textoTiempo != null)
        {
            // Formatear como MM:SS
            int minutos = Mathf.FloorToInt(tiempoRestante / 60f);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60f);
            textoTiempo.text = $"Tiempo: {minutos:00}:{segundos:00}";

            // Cambiar color si queda poco tiempo
            if (tiempoRestante <= 10f)
            {
                textoTiempo.color = Color.red;
            }
            else
            {
                textoTiempo.color = Color.white;
            }
        }
    }

    private void MostrarGameOver(int puntuacionFinal)
    {
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text = $"Puntuación: {puntuacionFinal}";
        }

        if (textoMejorPuntuacion != null)
        {
            int mejorPuntuacion = PlayerPrefs.GetInt("CoinGameBestScore", 0);
            textoMejorPuntuacion.text = $"Mejor Puntuación: {mejorPuntuacion}";
        }
    }

    private void OnBotonReiniciarClick()
    {
        // Ocultar panel
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(false);
        }

        // Reiniciar juego
        CoinGameManager.Instance.ReiniciarJuego();
    }
}

// =============================================================================
// SECCIÓN 10: CHECKLIST PARA EXAMEN
// =============================================================================

/*
 * ✅ CHECKLIST - CONCEPTOS ESENCIALES PARA CREAR UN JUEGO
 * ========================================================
 *
 * 1. CREAR SCRIPT BÁSICO
 *    □ Crear clase que hereda de MonoBehaviour
 *    □ Usar namespace System.Collections para IEnumerator
 *    □ Añadir using statements necesarios (UnityEngine, UnityEngine.UI, TMPro)
 *
 * 2. CICLO DE VIDA (LIFECYCLE)
 *    □ Awake() - Inicialización temprana, referencias, Singleton setup
 *    □ Start() - Inicialización que depende de otros objetos
 *    □ Update() - Input, lógica frame-by-frame
 *    □ FixedUpdate() - Física
 *    □ OnEnable/OnDisable() - Suscribir/desuscribir eventos
 *
 * 3. INPUT DEL JUGADOR
 *    □ Input.GetKey() / GetKeyDown() / GetKeyUp() para teclas
 *    □ Input.GetAxis("Horizontal") / GetAxis("Vertical") para movimiento
 *    □ Input.GetMouseButtonDown() para clicks
 *    □ Usar Time.deltaTime para movimiento independiente del framerate
 *
 * 4. MOVIMIENTO Y TRANSFORM
 *    □ transform.position para mover objetos
 *    □ transform.Translate() para movimiento relativo
 *    □ transform.rotation / transform.eulerAngles para rotar
 *    □ transform.Rotate() para rotación gradual
 *    □ Vector3 para posiciones y direcciones
 *
 * 5. INSTANCIAR Y DESTRUIR OBJETOS
 *    □ Instantiate(prefab, posicion, rotacion) para crear objetos
 *    □ Destroy(objeto) para eliminar
 *    □ Destroy(objeto, tiempo) para eliminar con delay
 *    □ GameObject.Find() / FindWithTag() para buscar
 *    □ GetComponent<T>() para obtener componentes
 *
 * 6. COLISIONES Y TRIGGERS
 *    □ OnCollisionEnter() para colisiones físicas (Collider NO trigger)
 *    □ OnTriggerEnter() para triggers (Collider con "Is Trigger" activado)
 *    □ other.CompareTag("TagName") para identificar objetos
 *    □ other.GetComponent<T>() para acceder a componentes del objeto colisionado
 *
 * 7. UI (INTERFAZ DE USUARIO)
 *    □ TextMeshProUGUI para texto (mejor que Text)
 *    □ text.text = "..." para cambiar texto
 *    □ Button.onClick.AddListener(Metodo) para botones
 *    □ [SerializeField] para referencias en Inspector
 *
 * 8. COROUTINES (DELAYS Y ANIMACIONES)
 *    □ IEnumerator para definir una Coroutine
 *    □ yield return new WaitForSeconds(tiempo) para esperar
 *    □ StartCoroutine(MiCoroutine()) para iniciar
 *    □ StopCoroutine() para detener
 *
 * 9. EVENTOS (COMUNICACIÓN ENTRE SCRIPTS)
 *    □ public static event Action<T> OnEvento; para declarar
 *    □ OnEvento?.Invoke(parametros) para disparar
 *    □ OnEvento += Metodo en OnEnable() para suscribir
 *    □ OnEvento -= Metodo en OnDisable() para desuscribir
 *
 * 10. SINGLETON (MANAGER GLOBAL)
 *     □ public static MiClase Instance { get; private set; }
 *     □ Instance = this en Awake()
 *     □ Verificar si Instance ya existe (destruir duplicados)
 *     □ DontDestroyOnLoad() para persistir entre escenas
 *
 * 11. SCENE MANAGEMENT
 *     □ using UnityEngine.SceneManagement;
 *     □ SceneManager.LoadScene("NombreEscena") para cambiar escena
 *     □ Application.Quit() para salir del juego
 *
 * 12. PLAYERPREFS (GUARDAR DATOS)
 *     □ PlayerPrefs.SetInt("Clave", valor) para guardar entero
 *     □ PlayerPrefs.GetInt("Clave", valorDefault) para leer
 *     □ SetFloat(), GetFloat() para decimales
 *     □ SetString(), GetString() para texto
 *     □ PlayerPrefs.Save() para forzar guardado
 *
 * 13. DEBUG
 *     □ Debug.Log("Mensaje") para imprimir en consola
 *     □ Debug.LogWarning() para advertencias
 *     □ Debug.LogError() para errores
 *     □ Usar $ antes de string para interpolación: $"Valor: {variable}"
 *
 * 14. AUDIO
 *     □ AudioSource para reproducir sonidos
 *     □ audioSource.Play() para reproducir
 *     □ audioSource.PlayOneShot(clip) para efectos de sonido
 *     □ AudioListener.volume para volumen global
 *
 * 15. RANDOM
 *     □ UnityEngine.Random.Range(min, max) para números aleatorios
 *     □ Random.Range(0f, 1f) para float entre 0 y 1
 *     □ Random.Range(0, 10) para int entre 0 y 9 (max excluido en int)
 *
 * ESTRUCTURA TÍPICA DE UN JUEGO SIMPLE:
 * ======================================
 *
 * 1. GameManager (Singleton)
 *    - Controla el estado del juego
 *    - Puntuación, vidas, tiempo
 *    - Eventos de game over, victoria
 *
 * 2. PlayerController
 *    - Captura input
 *    - Mueve al jugador
 *    - Detecta colisiones
 *
 * 3. Enemy / Coin / Pickup
 *    - Lógica individual de cada objeto
 *    - OnTriggerEnter para recolección
 *
 * 4. Spawner
 *    - Crea objetos periódicamente
 *    - Usa Coroutines para timing
 *
 * 5. UIManager
 *    - Actualiza texto de puntuación, tiempo
 *    - Responde a eventos del GameManager
 *    - Maneja botones
 *
 * TIPS FINALES:
 * =============
 * - Siempre verificar null antes de usar referencias ([SerializeField] puede estar vacío)
 * - Usar [Header("Nombre")] para organizar variables en Inspector
 * - Comentar tu código para recordar qué hace cada parte
 * - Probar frecuentemente en el editor
 * - Los tags deben estar configurados en Unity (Edit > Project Settings > Tags)
 * - Los prefabs deben estar asignados en el Inspector
 * - Las escenas deben estar en Build Settings para poder cargarlas
 *
 * ¡BUENA SUERTE EN TU EXAMEN!
 */

// =============================================================================
// FIN DEL TUTORIAL
// =============================================================================
// Este archivo contiene TODO lo necesario para crear un juego básico en Unity
// Léelo completo, practica los ejemplos, y modifícalos para entender mejor
// El juego de ejemplo (Sección 9) es funcional y puede servir como base
// =============================================================================
