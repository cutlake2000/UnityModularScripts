using System;

namespace UnityModularScripts.DesignPatterns.ObjectPool_Pattern
{
    /// <summary>
    /// 풀링된 오브젝트가 구현해야 할 인터페이스.
    /// 오브젝트의 생명주기를 관리합니다.
    /// </summary>
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
}