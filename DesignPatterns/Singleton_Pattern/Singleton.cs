namespace UnityModularScripts.DesignPatterns.Singleton_Pattern
{
    /// <summary>
    /// 제네릭 싱글톤 클래스. MonoBehaviour에 의존하지 않으며 모든 클래스에서 싱글톤으로 사용될 수 있습니다.
    /// </summary>
    /// <typeparam name="T">싱글톤을 적용할 클래스 타입</typeparam>
    public class Singleton<T> where T : class, new()
    {
        private static readonly object _lock = new object();
        private static T _instance;

        /// <summary>
        /// 싱글톤 인스턴스를 반환합니다. 인스턴스가 없으면 새로 생성됩니다.
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }

        /// <summary>
        /// 인스턴스를 제거할 때 사용할 수 있습니다.
        /// </summary>
        public static void ClearInstance()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }
    }
}