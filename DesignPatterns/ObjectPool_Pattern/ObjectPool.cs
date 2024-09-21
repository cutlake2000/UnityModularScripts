using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityModularScripts.DesignPatterns.ObjectPool_Pattern
{
    /// <summary>
    /// 제네릭 오브젝트 풀 클래스. 오브젝트를 생성하고 관리하며, 객체의 생명 주기를 처리합니다.
    /// </summary>
    /// <typeparam name="T">풀링할 객체 타입 (IPoolable을 구현해야 함)</typeparam>
    public class ObjectPool<T> : ICustomObjectPool<T> where T : IPoolable
    {
        /// <summary>
        /// 풀 내부에서 관리되는 큐. 오브젝트를 보관합니다.
        /// </summary>
        private readonly Queue<T> poolQueue = new();

        /// <summary>
        /// 오브젝트를 생성하는 팩토리 메서드. 오브젝트가 부족할 경우 새로 생성합니다.
        /// </summary>
        private readonly Func<T> objectFactory;

        /// <summary>
        /// 지정된 초기 크기로 오브젝트 풀을 생성합니다.
        /// </summary>
        /// <param name="initialSize">초기 풀 크기</param>
        /// <param name="objectFactory">오브젝트 팩토리 함수</param>
        public ObjectPool(int initialSize, Func<T> objectFactory)
        {
            this.objectFactory = objectFactory;

            // 초기 풀 크기만큼 오브젝트를 생성하여 큐에 추가합니다.
            for (var i = 0; i < initialSize; i++)
            {
                T obj = objectFactory();
                obj.OnCreate();  // 객체 생성 시 초기화
                poolQueue.Enqueue(obj);
            }
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다. 
        /// 오브젝트가 부족하면 팩토리를 통해 새로운 오브젝트를 생성합니다.
        /// </summary>
        /// <returns>풀에서 가져온 오브젝트</returns>
        public T GetObject()
        {
            T obj = poolQueue.Count > 0 ? poolQueue.Dequeue() : objectFactory();
            obj.OnGetFromPool();
            return obj;
        }

        /// <summary>
        /// 오브젝트를 풀로 반환합니다.
        /// 반환된 오브젝트는 재사용할 수 있도록 초기화됩니다.
        /// </summary>
        /// <param name="obj">반환할 오브젝트</param>
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
}