#if UNITY_ADDRESSABLES

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityModularScripts.Utilities.Addressable_Utility
{
    /// <summary>
    /// Addressable 자산을 로드하고 관리하는 유틸리티 클래스입니다.
    /// 어드레서블이 설치된 경우에만 활성화됩니다.
    /// </summary>
    public class AddressableAssetManager
    {
        // 인스턴스화된 오브젝트와 그에 대응하는 AsyncOperationHandle을 관리합니다.
        private static readonly Dictionary<GameObject, AsyncOperationHandle<GameObject>> instantiatedAssetHandles = new();

        // 캐싱된 자산의 로드 핸들을 관리합니다.
        private static readonly Dictionary<string, AsyncOperationHandle> assetLoadCache = new();

        /// <summary>
        /// 특정 AssetReference에서 자산을 로드합니다. 
        /// 동일한 자산이 여러 번 로드되지 않도록 캐시를 사용합니다.
        /// </summary>
        public static void LoadAsset<T>(AssetReference assetReference, Action<T> onLoadComplete = null, Action<Exception> onLoadError = null)
        {
            if (assetReference.RuntimeKeyIsValid())
            {
                if (assetLoadCache.TryGetValue(assetReference.RuntimeKey.ToString(), out var cachedHandle))
                {
                    if (cachedHandle.Result is T cachedAsset)
                    {
                        onLoadComplete?.Invoke(cachedAsset);
                        return;
                    }
                }

                var loadOperation = Addressables.LoadAssetAsync<T>(assetReference);
                loadOperation.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        assetLoadCache[assetReference.RuntimeKey.ToString()] = op;
                        onLoadComplete?.Invoke(op.Result);
                    }
                    else
                    {
                        onLoadError?.Invoke(new Exception($"Failed to load asset: {assetReference.RuntimeKey}"));
                    }
                };
            }
            else
            {
                onLoadError?.Invoke(new Exception($"Invalid AssetReference: {assetReference}"));
            }
        }

        /// <summary>
        /// 여러 AssetReference를 일괄적으로 로드합니다.
        /// </summary>
        /// <param name="assetReferences">로드할 AssetReference 리스트</param>
        /// <param name="onAllAssetsLoaded">모든 자산 로드 완료 시 호출되는 콜백</param>
        /// <param name="onLoadError">개별 자산 로드 실패 시 호출되는 콜백</param>
        public static void LoadAssets(
            List<AssetReference> assetReferences,
            Action<Dictionary<AssetReference, object>> onAllAssetsLoaded,
            Action<Exception> onLoadError = null)
        {
            Dictionary<AssetReference, object> loadedAssets = new Dictionary<AssetReference, object>();
            int totalAssets = assetReferences.Count;
            int loadedCount = 0;

            foreach (var assetReference in assetReferences)
            {
                LoadAsset<object>(assetReference, (asset) =>
                {
                    loadedAssets.Add(assetReference, asset);
                    loadedCount++;
                    if (loadedCount == totalAssets)
                    {
                        onAllAssetsLoaded?.Invoke(loadedAssets);
                    }
                }, onLoadError);
            }
        }

        /// <summary>
        /// 특정 AssetReference에서 GameObject를 인스턴스화합니다.
        /// </summary>
        public static bool InstantiateAsset(AssetReference assetReference, Vector3 position, Quaternion rotation, Transform parentTransform = null, Action<GameObject> onInstantiationComplete = null, Action<Exception> onInstantiationError = null)
        {
            if (assetReference.RuntimeKeyIsValid())
            {
                var instantiateOperation = parentTransform != null
                    ? Addressables.InstantiateAsync(assetReference, position, rotation, parentTransform)
                    : Addressables.InstantiateAsync(assetReference, position, rotation);

                instantiateOperation.Completed += (op) =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        instantiatedAssetHandles[op.Result] = instantiateOperation;
                        onInstantiationComplete?.Invoke(op.Result);
                    }
                    else
                    {
                        onInstantiationError?.Invoke(new Exception($"Failed to instantiate asset: {assetReference.RuntimeKey}"));
                    }
                };
                return true;
            }

            onInstantiationError?.Invoke(new Exception($"Invalid AssetReference: {assetReference}"));
            return false;
        }

        /// <summary>
        /// 여러 AssetReference를 일괄적으로 인스턴스화합니다.
        /// </summary>
        /// <param name="assetReferences">인스턴스화할 AssetReference 리스트</param>
        /// <param name="onAllAssetsInstantiated">모든 자산 인스턴스화 완료 시 호출되는 콜백</param>
        /// <param name="onInstantiationError">개별 자산 인스턴스화 실패 시 호출되는 콜백</param>
        public static void InstantiateAssets(
            List<AssetReference> assetReferences,
            Action<Dictionary<AssetReference, GameObject>> onAllAssetsInstantiated,
            Action<Exception> onInstantiationError = null)
        {
            Dictionary<AssetReference, GameObject> instantiatedAssets = new Dictionary<AssetReference, GameObject>();
            int totalAssets = assetReferences.Count;
            int instantiatedCount = 0;

            foreach (var assetReference in assetReferences)
            {
                InstantiateAsset(assetReference, Vector3.zero, Quaternion.identity, null, (instance) =>
                {
                    instantiatedAssets.Add(assetReference, instance);
                    instantiatedCount++;
                    if (instantiatedCount == totalAssets)
                    {
                        onAllAssetsInstantiated?.Invoke(instantiatedAssets);
                    }
                }, onInstantiationError);
            }
        }

        /// <summary>
        /// 특정 GameObject가 Addressable을 통해 인스턴스화된 것인지 확인합니다.
        /// </summary>
        public static bool IsAddressableInstantiated(GameObject gameObject)
        {
            return instantiatedAssetHandles.ContainsKey(gameObject);
        }

        /// <summary>
        /// Addressable을 통해 인스턴스화된 GameObject를 해제하고 자산을 메모리에서 제거합니다.
        /// </summary>
        public static void ReleaseInstantiatedAsset(GameObject instantiatedObject)
        {
            if (instantiatedAssetHandles.TryGetValue(instantiatedObject, out var handle))
            {
                Addressables.ReleaseInstance(instantiatedObject);
                Addressables.Release(handle);
                instantiatedAssetHandles.Remove(instantiatedObject);
            }
            else
            {
                Debug.LogWarning($"Tried to release an object that wasn't instantiated by AddressableAssetManager: {instantiatedObject.name}");
            }
        }

        /// <summary>
        /// 로드된 자산을 캐시에서 해제하고 메모리에서 제거합니다.
        /// </summary>
        public static void ReleaseCachedAsset(string assetKey)
        {
            if (assetLoadCache.TryGetValue(assetKey, out var handle))
            {
                Addressables.Release(handle);
                assetLoadCache.Remove(assetKey);
            }
        }
    }
}

#else

using UnityEngine;

namespace UnityModularScripts.Utilities.Addressable_Utility
{
    /// <summary>
    /// Addressable 시스템이 설치되지 않았을 때 사용되는 대체 클래스.
    /// </summary>
    public class AddressableAssetManager : MonoBehaviour
    {
        private void Start()
        {
            Debug.LogError("Addressable 시스템이 설치되지 않았습니다. Addressable 기능은 비활성화됩니다.");
        }
    }
}

#endif