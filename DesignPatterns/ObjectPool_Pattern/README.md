
# UnityModularScripts - Object Pool Pattern 모듈화 스크립트

이 프로젝트는 **Unity**에서 효율적으로 메모리와 성능을 관리할 수 있도록 **Object Pool (오브젝트 풀링)** 디자인 패턴을 기반으로 작성된 모듈화된 스크립트들로 구성되어 있습니다. 오브젝트 풀링을 통해 자주 생성되고 삭제되는 오브젝트들을 재사용하여 성능 향상을 기대할 수 있습니다. 이 스크립트들은 다양한 상황에 유연하게 적용될 수 있도록 설계되었습니다.

---

## 주요 폴더 및 파일 설명

### 1. **ObjectPool_Pattern** 폴더
이 폴더는 **오브젝트 풀링** 패턴을 구현한 클래스들로 구성되어 있으며, 유니티 프로젝트 내에서 쉽게 재사용할 수 있도록 설계되었습니다.

#### **ICustomObjectPool.cs**
`ICustomObjectPool` 인터페이스는 커스텀 오브젝트 풀을 정의합니다.  
풀에서 오브젝트를 가져오거나 반환하는 메서드를 제공합니다.

```csharp
public interface ICustomObjectPool<T>
{
    /// <summary>
    /// 풀에서 오브젝트를 가져오는 메서드.
    /// </summary>
    /// <returns>풀에서 가져온 오브젝트.</returns>
    T GetObject();

    /// <summary>
    /// 오브젝트를 풀에 반환하는 메서드.
    /// </summary>
    /// <param name="obj">반환할 오브젝트.</param>
    void ReturnObject(T obj);
}
```

---

#### **IPoolable.cs**
`IPoolable` 인터페이스는 오브젝트의 생명 주기를 관리하는 인터페이스입니다.  
풀에서 가져오거나 반환될 때 호출되는 이벤트와 메서드를 정의합니다.

```csharp
public interface IPoolable
{
    /// <summary>
    /// 오브젝트가 처음 생성될 때 호출되는 이벤트.
    /// </summary>
    event Action OnCreateEvent;

    /// <summary>
    /// 오브젝트가 풀에서 가져올 때 호출되는 이벤트.
    /// </summary>
    event Action OnGetFromPoolEvent;

    /// <summary>
    /// 오브젝트가 풀로 반환될 때 호출되는 이벤트.
    /// </summary>
    event Action OnReturnToPoolEvent;

    /// <summary>
    /// 오브젝트가 생성될 때 호출되는 메서드.
    /// </summary>
    void OnCreate();

    /// <summary>
    /// 오브젝트가 풀에서 가져올 때 호출되는 메서드.
    /// </summary>
    void OnGetFromPool();

    /// <summary>
    /// 오브젝트가 풀로 반환될 때 호출되는 메서드.
    /// </summary>
    void OnReturnToPool();

    /// <summary>
    /// 오브젝트를 재사용할 때 상태를 초기화하는 메서드.
    /// </summary>
    void Reset();
}
```

---

#### **ObjectPool.cs**
`ObjectPool` 클래스는 제네릭 형태의 오브젝트 풀을 구현하며, 오브젝트의 생명 주기를 관리하고 필요에 따라 오브젝트를 재사용할 수 있도록 설계되었습니다.

```csharp
public class ObjectPool<T> : ICustomObjectPool<T> where T : IPoolable
{
    private readonly Queue<T> poolQueue = new();
    private readonly Func<T> objectFactory;

    /// <summary>
    /// 초기 크기와 팩토리 메서드를 통해 풀을 생성합니다.
    /// </summary>
    public ObjectPool(int initialSize, Func<T> objectFactory)
    {
        this.objectFactory = objectFactory;

        for (var i = 0; i < initialSize; i++)
        {
            T obj = objectFactory();
            obj.OnCreate();  // 초기화
            poolQueue.Enqueue(obj);
        }
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져옵니다. 부족하면 새로 생성합니다.
    /// </summary>
    public T GetObject()
    {
        T obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : objectFactory();
        obj.OnGetFromPool();
        return obj;
    }

    /// <summary>
    /// 오브젝트를 풀에 반환하고 초기화합니다.
    /// </summary>
    public void ReturnObject(T obj)
    {
        obj.OnReturnToPool();
        obj.Reset();
        poolQueue.Enqueue(obj);
    }

    /// <summary>
    /// 모든 풀링된 오브젝트를 파괴하고 풀을 비웁니다.
    /// </summary>
    public void DestroyPool()
    {
        while (poolQueue.Count > 0)
        {
            T obj = poolQueue.Dequeue();
            obj.OnReturnToPool();

            // 오브젝트가 Unity의 GameObject일 경우 비활성화 처리
            var gameObject = obj as GameObject;
            if (gameObject is not null)
            {
                UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
}
```

---

#### **ObjectPoolManager.cs**
`ObjectPoolManager` 클래스는 여러 오브젝트 풀을 관리하는 싱글톤 클래스로, 다양한 풀을 생성하고 관리할 수 있습니다.  
`MonoBehaviour`에 의존하지 않으며, 객체를 효율적으로 관리할 수 있는 도구를 제공합니다.

```csharp
public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    private readonly Dictionary<string, object> poolDictionary = new();
    private const int DefaultPoolSize = 10;

    /// <summary>
    /// 요청한 키에 해당하는 풀에서 오브젝트를 가져옵니다.
    /// </summary>
    public T GetObject<T>(string key, Func<T> objectFactory) where T : IPoolable
    {
        if (!poolDictionary.ContainsKey(key))
        {
            CreatePool(key, DefaultPoolSize, objectFactory);
        }

        return ((ObjectPool<T>)poolDictionary[key]).GetObject();
    }

    /// <summary>
    /// 오브젝트를 풀에 반환합니다.
    /// </summary>
    public void ReturnObject<T>(string key, T obj) where T : IPoolable
    {
        if (poolDictionary.TryGetValue(key, out var pool))
        {
            ((ObjectPool<T>)pool).ReturnObject(obj);
        }
        else
        {
            throw new Exception($"Pool with key '{key}' does not exist.");
        }
    }

    /// <summary>
    /// 새로운 오브젝트 풀을 생성합니다.
    /// </summary>
    public void CreatePool<T>(string key, int initialSize, Func<T> objectFactory) where T : IPoolable
    {
        if (poolDictionary.ContainsKey(key))
        {
            throw new Exception($"Pool with key '{key}' already exists.");
        }

        var newPool = new ObjectPool<T>(initialSize, objectFactory);
        poolDictionary[key] = newPool;
    }

    /// <summary>
    /// 요청한 키에 해당하는 오브젝트 풀을 제거하고 풀 내부의 모든 오브젝트를 파괴합니다.
    /// </summary>
    public void DestroyPool(string key)
    {
        if (poolDictionary.TryGetValue(key, out var pool))
        {
            ((ObjectPool<IPoolable>)pool).DestroyPool();
            poolDictionary.Remove(key);
        }
        else
        {
            throw new Exception($"Pool with key '{key}' does not exist.");
        }
    }
}
```