namespace UnityModularScripts.DesignPatterns.ObjectPool_Pattern
{
    /// <summary>
    /// 커스텀 오브젝트 풀 인터페이스. 풀링된 객체를 가져오거나 반환하는 메서드를 제공합니다.
    /// </summary>
    /// <typeparam name="T">풀링할 객체 타입</typeparam>
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
}