#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace UnityModularScripts.Utilities.Addressable_Utility
{
    [InitializeOnLoad]
    public static class AddressablesChecker
    {
        static AddressablesChecker()
        {
            // 패키지 관리자의 패키지 리스트를 가져와서 Addressables 패키지가 설치되었는지 확인
            if (IsAddressablesInstalled())
            {
                AddDefineSymbol("UNITY_ADDRESSABLES");
            }
            else
            {
                RemoveDefineSymbol("UNITY_ADDRESSABLES");
            }
        }

        // Addressables 패키지가 설치되었는지 확인하는 함수
        private static bool IsAddressablesInstalled()
        {
            return Type.GetType("UnityEngine.AddressableAssets.Addressables, Unity.Addressables") != null;
        }

        // Scripting Define Symbols에 심볼을 추가하는 함수
        private static void AddDefineSymbol(string symbol)
        {
            var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            
            if (!scriptingDefineSymbols.Contains(symbol))
            {
                scriptingDefineSymbols += ";" + symbol;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingDefineSymbols);
                Debug.Log($"Added {symbol} to Scripting Define Symbols.");
            }
        }

        // Scripting Define Symbols에서 심볼을 제거하는 함수
        private static void RemoveDefineSymbol(string symbol)
        {
            var scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            
            if (scriptingDefineSymbols.Contains(symbol))
            {
                scriptingDefineSymbols = scriptingDefineSymbols.Replace(symbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingDefineSymbols);
                Debug.Log($"Removed {symbol} from Scripting Define Symbols.");
            }
        }
    }
}
#endif