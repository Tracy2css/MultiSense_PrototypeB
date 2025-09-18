using System;
using System.Linq;
using UnityEditor;

namespace VRUIP
{
    public class VRUIPMenu
    {
        // CONSTS -------------------------------------------------------------
        private const string OculusSettingPath = "VRUIP/Target Framework/Oculus Integration";
        private const string MetaSDKSettingPath = "VRUIP/Target Framework/Meta SDK";
        private const string XRITKSettingPath = "VRUIP/Target Framework/XR Interaction Toolkit";
        private const string UnityEditorSettingPath = "VRUIP/Target Framework/Unity Editor";
        
        public const string SettingPrefKey = "vruip_tf";

        // DEFINE SYMBOLS
        private const string DefineOculus = "OCULUS_INTEGRATION";
        private const string DefineXRITK = "XR_ITK";
        private const string DefineMetaSDK = "META_SDK";

        // EDITOR PREFS -------------------------------------------------------------
        private static VRUIPManager.TargetFramework TargetFramework
        {
            get => EditorUtil.StringToTargetFramework(EditorPrefs.GetString(SettingPrefKey));
            set => EditorPrefs.SetString(SettingPrefKey, EditorUtil.TargetFrameworkToString(value));
        }

            // MENU FUNCTIONS -------------------------------------------------------------
        [MenuItem(OculusSettingPath)]
        private static void SetOculus()
        {
            TargetFramework = VRUIPManager.TargetFramework.OculusIntegration;
            SetupDefines();
        }
        
        [MenuItem(MetaSDKSettingPath)]
        private static void SetMetaSDK()
        {
            TargetFramework = VRUIPManager.TargetFramework.MetaSDK;
            SetupDefines();
        }
        
        [MenuItem(XRITKSettingPath)]
        private static void SetXRITK()
        {
            TargetFramework = VRUIPManager.TargetFramework.XRInteractionToolkit;
            SetupDefines();
        }
    
        [MenuItem(UnityEditorSettingPath)]
        private static void SetUnityEditor()
        {
            TargetFramework = VRUIPManager.TargetFramework.UnityEditorTesting;
            SetupDefines();
        }

        // VALIDATION FUNCTIONS -------------------------------------------------------------
        [MenuItem(OculusSettingPath, true)]
        private static bool ValidateOculus()
        {
            Menu.SetChecked(OculusSettingPath, TargetFramework == VRUIPManager.TargetFramework.OculusIntegration);
            return true;
        }
        
        [MenuItem(MetaSDKSettingPath, true)]
        private static bool ValidateMetaSDK()
        {
            Menu.SetChecked(MetaSDKSettingPath, TargetFramework == VRUIPManager.TargetFramework.MetaSDK);
            return true;
        }
        
        [MenuItem(XRITKSettingPath, true)]
        private static bool ValidateXRITK()
        {
            Menu.SetChecked(XRITKSettingPath, TargetFramework == VRUIPManager.TargetFramework.XRInteractionToolkit);
            return true;
        }
        
        [MenuItem(UnityEditorSettingPath, true)]
        private static bool ValidateUnityEditor()
        {
            Menu.SetChecked(UnityEditorSettingPath, TargetFramework == VRUIPManager.TargetFramework.UnityEditorTesting);
            return true;
        }
        
        // OTHER FUNCTIONS -------------------------------------------------------------
        private static void SetupDefines()
        {
            var targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (TargetFramework == VRUIPManager.TargetFramework.OculusIntegration)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out var defines);
                if (defines.Contains(DefineOculus)) return;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, new[] { DefineOculus });
            }
            else if (TargetFramework == VRUIPManager.TargetFramework.MetaSDK)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out var defines);
                if (defines.Contains(DefineMetaSDK)) return;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, new[] { DefineMetaSDK });
            }
            else if (TargetFramework == VRUIPManager.TargetFramework.XRInteractionToolkit)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out var defines);
                if (defines.Contains(DefineXRITK)) return;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, new[] { DefineXRITK });
            }
            else if (TargetFramework == VRUIPManager.TargetFramework.UnityEditorTesting)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out var defines);
                if (defines.Length == 0) return;
                if (defines.Contains(DefineOculus) || defines.Contains(DefineXRITK) || defines.Contains(DefineMetaSDK))
                {
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, Array.Empty<string>());
                }
            }
        }
    }
}
