using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

public class DemoSettingsProvider : SettingProviderBase<DemoSettings>
{
    public DemoSettingsProvider(string path, SettingsScope scope) : base(path, scope) { }

    [SettingsProvider]
    public static SettingsProvider GetSettingsProvider() => CreateProviderForProjectSettings();
}
#endif