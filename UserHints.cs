using System;
using System.IO;
using Notiffy.API;
using ThornClient.System;
using UnityEngine.SceneManagement;

namespace Crossover;

public static class UserHints {
    public static void IssueUpdateNoticeIfNecessary() {
        var verString = CrossoverConfig.LastVersion.Value ?? "999.999.999";
        Version lastVersion = new Version(verString);
        Version currVersion = new Version("2.0.0");
        if (currVersion.CompareTo(lastVersion) == 1) {
            bool isUpdate = verString == "0.0.0";
            string updateAction = isUpdate ? "updated" : "installed";
            string bodyPrefix = isUpdate
                ? "<b>2.0.0</b>: Configuration is now in Thorn"
                : "Configuration is available in Thorn";
            string configNaviAdvice = $"Press {ThornModule.Instance!.OpenClickGUI.Value.ToString(true)} then search \"crossover\"";
            NotificationSystem.NotifySend($"<color=#db1e39>Crossover</color> {updateAction}",
                $"{bodyPrefix}. {configNaviAdvice}",
                expireTime: 10000, iconFilePath: Path.Combine(Plugin.workingDir, "icon.png"));
            CrossoverConfig.LastVersion.Value = Plugin.PluginVersion;
        }

        SceneManager.sceneLoaded -= IssueUpdateNoticeIfNecessary;
    }

    public static void IssueUpdateNoticeIfNecessary(Scene _, LoadSceneMode __) {
        IssueUpdateNoticeIfNecessary();
    }

    public static void Initialize() { }

    static UserHints() {
        SceneManager.sceneLoaded += IssueUpdateNoticeIfNecessary;
    }
}
