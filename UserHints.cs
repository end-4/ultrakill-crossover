using System;
using System.IO;
using Notiffy.API;
using UnityEngine.SceneManagement;

namespace Crossover;

public static class UserHints {

    public static void IssueUpdateNoticeIfNecessary() {
        Version lastVersion = new Version(ConfigManager.LastVersion.value);
        Version currVersion = new Version("1.1.0");
        if (currVersion.CompareTo(lastVersion) == 1) {
            ConfigManager.LastVersion.value = Plugin.PluginVersion;
            NotificationSystem.NotifySend("<color=#db1e39>Crossover</color> updated",
                "<b>1.1.0</b>: Added enemy tracking icons. Great for hunting down the last Cyber Grind Schisms or focusing on dangerous enemies. You can enable it in <color=#55c7f6>Options > Plugin Config > Crossover</color>",
                expireTime: 10000, iconFilePath: Path.Combine(Plugin.workingDir, "icon.png"));
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
