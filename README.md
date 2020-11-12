# Practica 3 - Delegados, Eventos
## Javier Duque Melguizo

## Apartado 1
Agregar dos objetos en la escena: A y B. Cuando el jugador colisiona con un objeto de tipo B, el objeto A volcará en consola un mensaje. Cuando toca el objeto A se incrementará un contador en el objeto B.

**Al tocar la moneda, el coche policia suelta un mensaje por consola:**
![img/1.a.gif](img/1.a.gif)

Esto se consigue con el siguiente codigo:

El codigo del jugador que será el responsable de detectar cuando se choca y disparar el evento "OnPlayerGameObjectCollision" en consecuencia
```c#
public delegate void fOnPlayerGameObjectCollision(PlayerHandler handler, GameObject other); //Importante para definir el tipo de puntero a función que el evento registrará.
public class PlayerHandler : MonoBehaviour
{
	//Todo esto de manejar instancias es para que solo ocurra con una instancia de jugador principal y no una teorica implementación con muchos jugadores
    private static Dictionary<int, PlayerHandler> playerDataBase = new Dictionary<int, PlayerHandler>();
    public static PlayerHandler GetPlayerInstance(int id)
    {
        PlayerHandler player;
        if (PlayerHandler.playerDataBase.TryGetValue(id, out player))
            return player;
        else
            return null;
    }

    private static int mainPlayerId = -1;
    public static int MainPlayerId { get => mainPlayerId; }

    private PlayerHandler() { }
	/// Lo anterior es para tener controlado las posibles instancias del jugador. El codigo responsable de hacer lo que la actividad solicita empieza aqui:
	public event fOnPlayerGameObjectCollision eventOnPlayerGameObjectCollision;
	private void OnTriggerEnter(Collider other)
    {
        eventOnPlayerGameObjectCollision(this,other.gameObject);
    }
}
```

El codigo para que la moneda se "registre" al evento del jugador, lo reciba correctamente y entonces ejecute otro evento ("onCoinCollision") al que estara registrado el coche policia para que diga algo por consola en consecuencia

```c#
public delegate void fOnCoinDestroy(CoinHandler handler);
public class CoinHandler : MonoBehaviour
{
	public static event fOnCoinDestroy eventOnCoinDestroy;
	void Start()
    {
        PlayerHandler.GetPlayerInstance(PlayerHandler.MainPlayerId).eventOnPlayerGameObjectCollision += OnPlayerGameObjectCollision;
    }
	
	 void OnPlayerGameObjectCollision(PlayerHandler handler, GameObject other)
    {
        if (other.GetInstanceID() == this.gameObject.GetInstanceID())
            CoinHandler.eventOnCoinCollision(this); //Disparo otro evento. En este caso para que el objeto tipo B(el coche policia), que estará registrado a él, diga algo cuando la consecuencia "Jugador toca objeto tipo Moneda" ocurra
    }
}
```

Y aqui como el coche policia se registra al evento de la moneda para soltar por consola "¡¡¡Alto policia!!!":

```c#
public class InterceptorHandler : MonoBehaviour
{
	void Start()
    {
        CoinHandler.eventOnCoinCollision += onCoinCollision; //Para que si el jugador choca con la moneda,la moneda dispare el evento y el coche policia al estar registrado recibe esa colisión jugador-moneda
    }
	
	 void onCoinCollision(CoinHandler handler)
    {
        Debug.Log("¡¡¡Alto policia!!!");
    }
```

**Al tocar el coche policia, la moneda decrementa un contador (en este caso, su valor)**
![img/1.b.gif](img/1.b.gif)

Lo anterior se consigue, añadiendo al codigo anterior, lo siguiente:

Al script que maneja el coche policia se le agrega tambien un evento para que, como el coche policia se registra a un evento de la moneda, la moneda se registre al evento del coche policia y esta pueda detectar cuando se ha producido la colisión jugador-coche policia
```c#
public delegate void fOnInterceptorCollision(InterceptorHandler handler); // NEW
public class InterceptorHandler : MonoBehaviour
{
	void Start()
    {
        CoinHandler.eventOnCoinCollision += onCoinCollision; //Para que si el jugador choca con la moneda,la moneda dispare el evento y el coche policia al estar registrado recibe esa colisión jugador-moneda
		PlayerHandler.GetPlayerInstance(PlayerHandler.MainPlayerId).eventOnPlayerGameObjectCollision += OnPlayerGameObjectCollision; //NEW, para propagar el evento 
    }
	
	 void onCoinCollision(CoinHandler handler)
    {
        Debug.Log("¡¡¡Alto policia!!!");
    }
	
	void OnPlayerGameObjectCollision(PlayerHandler handler, GameObject other) // NEW
    {
        if(other.GetInstanceID() == this.gameObject.GetInstanceID())
            InterceptorHandler.eventOnInterceptorCollision(this); //La moneda recogerá este disparador
    }
```


Aqui el codigo de la moneda para decrecer su propiedad "value"
```c#
public delegate void fOnCoinDestroy(CoinHandler handler);
public class CoinHandler : MonoBehaviour
{
	public static event fOnCoinDestroy eventOnCoinDestroy;
	[ReadOnly] //Atributo custom para que la propiedad se pueda visualizar en el editor de Unity pero no se pueda modificar
    public int value = 100;
	void Start()
    {
        PlayerHandler.GetPlayerInstance(PlayerHandler.MainPlayerId).eventOnPlayerGameObjectCollision += OnPlayerGameObjectCollision;
		InterceptorHandler.eventOnInterceptorCollision += onInterceptorCollision; // NEW. Esencial para detectar cuando se produce la colisión jugador-coche policia
    }
	
	 void OnPlayerGameObjectCollision(PlayerHandler handler, GameObject other)
    {
        if (other.GetInstanceID() == this.gameObject.GetInstanceID())
            CoinHandler.eventOnCoinCollision(this); //Disparo otro evento. En este caso para que el objeto tipo B(el coche policia), que estará registrado a él, diga algo cuando la consecuencia "Jugador toca objeto tipo Moneda" ocurra
    }
	
	void onInterceptorCollision(InterceptorHandler handler) //NEW
    {
        this.value -= 10;
    }
}
```


## Apartado 2

Implementar un controlador de escena usando el patrón delegado que gestione las siguientes acciones:

**Si el jugador dispara, los objetos de tipo A que estén a una distancia media serán desplazados y los que estén a una distancia pequeña serán destruidos. Los umbrales que marcan los límites se deben configurar en el inspector.**
![img/2.a.gif](img/2.a.gif)

El codigo para lograr lo anterior es el siguiente:
```c#
public class PlayerHandler : MonoBehaviour
{
	[Range(0.0f, 20.0f)]
    public float distanciaMedia = 5.0f;
    [Range(0.0f, 20.0f)]
    public float distanciaCerca = 3.0f;
	
	void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Space)) //Disparo
        {
            Collider[] colliders = Physics.OverlapSphere(this.transform.position, distanciaCerca);
            foreach (Collider collider in colliders)
                if (collider.attachedRigidbody != null)
                    if (collider.gameObject.GetInstanceID() != TerrainHandler.Instance.gameObject.GetInstanceID())
                        GameObject.Destroy(collider.gameObject);

            colliders = Physics.OverlapSphere(this.transform.position, distanciaMedia);
            foreach (Collider collider in colliders)
                if (collider.attachedRigidbody != null)
                    collider.attachedRigidbody.AddExplosionForce(1.0f, this.transform.position, 10.0f, 3.0f, ForceMode.VelocityChange);
                   
        }

    }
}
```

**Si el jugador choca con objetos de tipo B, todos los de ese tipo sufrirán alguna transformación o algún cambio en su apariencia y decrementarán el poder del jugador.**
![img/2.b.gif](img/2.b.gif)

El codigo para lo anterior es el siguiente:


En el jugador nos registramos al evento "onInterceptorCollision" del coche policia y decrementamos su poder
```c#
public class PlayerHandler : MonoBehaviour
{
 void onInterceptorCollision(InterceptorHandler handler)
    {
        if(this.distanciaCerca > 10)
            this.distanciaCerca -= 10;
        if (this.distanciaMedia > 10)
            this.distanciaMedia -= 5;
    }
}
```

En el coche policia, en el evento "onPlayerCollision", si el objeto con el que se choca el jugador es el mismo, le aplicamos un cambio de color en el MeshRenderer
```c#
public class InterceptorHandler : MonoBehaviour
{
	void OnPlayerGameObjectCollision(PlayerHandler handler, GameObject other)
    {
        if(other.GetInstanceID() == this.gameObject.GetInstanceID())
        {
            InterceptorHandler.eventOnInterceptorCollision(this);
            Color random = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            this.GetComponent<MeshRenderer>().material.color = random;
        }
    }
}
```

## Apartado 3 
**En la escena habrá ubicados un tipo de objetos que al ser recolectados por el jugador harán que ciertos obstáculos se desplacen desbloqueando algún espacio.**
![img/3.a.gif](img/3.a.gif)
Codigo responsable:

El jugador al pulsar E aumenta su poder y recoge la moneda (realmente la elimina)
```c#
public class PlayerHandler : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var coin = other.gameObject.GetComponent<CoinHandler>();
        if (coin != null && Input.GetKey(KeyCode.E))
        {
            this.distanciaCerca += (this.distanciaCerca * (coin.value/2))/100;
            this.distanciaMedia += (this.distanciaCerca * coin.value)/100;
            GameObject.Destroy(other.gameObject);
            return;
        }
    }
}
```

La moneda estará a la escucha del momento en que sea eliminado, y cuando lo sea, disparara un evento para avisar a todos los objetos suscrito a ella
```c#
public delegate void fOnCoinDestroy(CoinHandler handler);
public class CoinHandler : MonoBehaviour
{
	public static event fOnCoinDestroy eventOnCoinDestroy;
    private void OnDestroy()
    {
        CoinHandler.eventOnCoinDestroy(this);
    }
}
```

Y aqui el obstaculo que se suscribira al evento "onCoinDestroy" y se desbloqueará cuando la moneda se destruya
```c#
public class BuildingHandler : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject door;
    bool open = false;
    void Start()
    {
        CoinHandler.eventOnCoinDestroy += onCoinDestroy;
        door = this.gameObject.GetComponentInChildren<DoorHandler>().gameObject;
    }

    void onCoinDestroy(CoinHandler handler)
    {
        this.open = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            door.transform.rotation = Quaternion.Euler(door.transform.rotation.eulerAngles.x - 1.0f,0 , 0);
            if (Mathf.Cos(door.transform.rotation.eulerAngles.x) == 0) open = false;
        }
           
    }
}
```


## Apartado 4
**Incorporar un elemento que sirva para encender o apagar un foco utilizando el teclado.**
![img/4.a.gif](img/4.a.gif)

El codigo responsable de ello es el siguiente:

El codigo del jugador responsable de, al estar cerca y pulsar e, encender la linterna
```c#
public class PlayerHandler : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var flashlight = other.gameObject.GetComponent<FlashLightHandler>();
        if (flashlight != null && Input.GetKey(KeyCode.E))
        {
            if (!flashlight.on)
            {
                flashlight.spotlight.intensity = 3;
                flashlight.on = true;
            }
            else
            {
                flashlight.spotlight.intensity = 0;
                flashlight.on = false;
            }
            return;
        }
    }
}
```

