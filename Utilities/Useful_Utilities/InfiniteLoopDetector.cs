using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;

namespace UnityModulpts.Utilities.Useful_Utilities
{
    public static class InfiniteLoopDetector
    {
        private const int DefaultDetectionThreshold = 100000;
        private static string _prevPoint = "";
        private static int _detectionCount;

        /// <summary>
        /// 디텍션 임계값을 에디터에서 조정할 수 있도록 UnityEditor에 메뉴 추가
        /// </summary>
#if UNITY_EDITOR
        private static int DetectionThreshold
        {
            get => EditorPrefs.GetInt("InfiniteLoopDetectionThreshold", DefaultDetectionThreshold);
            set => EditorPrefs.SetInt("InfiniteLoopDetectionThreshold", value);
        }
#endif

        [Conditional("UNITY_EDITOR")]
        public static void Run(
            [CallerMemberName] string mn = "",
            [CallerFilePath] string fp = "",
            [CallerLineNumber] int ln = 0
        )
        {
            var currentPoint = $"{fp}:{ln}, {mn}()";

            if (_prevPoint == currentPoint)
                _detectionCount++;
            else
                _detectionCount = 0;

            if (_detectionCount > DetectionThreshold)
            {
                var stackTrace = new StackTrace();
                throw new Exception(
                    $"Infinite Loop Detected: \n{currentPoint}\n\n" +
                    $"Stack Trace: {stackTrace}\n"
                );
            }

            _prevPoint = currentPoint;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void Init()
        {
            EditorApplication.update += () =>
            {
                // 매 프레임마다 디텍션 카운트 리셋
                _detectionCount = 0;
            };
        }

        // UnityEditor에 임계값을 변경할 수 있는 메뉴 추가
        [MenuItem("Tools/Set Infinite Loop Detection Threshold")]
        private static void SetThreshold()
        {
            string input = EditorUtility.DisplayDialogComplex(
                "Infinite Loop Detection Threshold",
                "현재 임계값: " + DetectionThreshold,
                "기본값 복원",
                "취소",
                "적용"
            ).ToString();

            DetectionThreshold = int.Parse(input);
        }
#endif
    }
}
