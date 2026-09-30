using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace PdfViewer;

/// <summary>
/// 現在のユーザーに対して、PDF を開けるアプリの候補として登録する。
/// 既定のアプリは Windows によって保護されているため、ここでは変更しない（利用者が選ぶ）。
/// </summary>
internal static class FileAssociation
{
    private const string AppName = "PdfViewerWV";
    private const string ProgId = AppName + ".pdf";
    private const string Extension = ".pdf";
    private const string AppKeyPath = @"Software\" + AppName;
    private const string CapabilitiesPath = AppKeyPath + @"\Capabilities";
    private const string ClassesPath = @"Software\Classes";
    private const string RegisteredApplicationsPath = @"Software\RegisteredApplications";

    /// <summary>
    /// 登録する。登録済みで exe の場所も変わっていない場合は何もしない。
    /// </summary>
    public static void Register(string exePath)
    {
        var command = $"\"{exePath}\" \"%1\"";
        var icon = $"\"{exePath}\",0";

        using var classes = Registry.CurrentUser.CreateSubKey(ClassesPath);
        using (var current = classes.OpenSubKey($@"{ProgId}\shell\open\command"))
        {
            if (current?.GetValue(null) as string == command)
            {
                return;
            }
        }

        // PDF を開くための ProgID
        SetValue(classes, ProgId, null, "PDF ドキュメント");
        SetValue(classes, $@"{ProgId}\DefaultIcon", null, icon);
        SetValue(classes, $@"{ProgId}\shell\open\command", null, command);

        // 「プログラムから開く」の候補
        using (var openWith = classes.CreateSubKey($@"{Extension}\OpenWithProgids"))
        {
            openWith.SetValue(ProgId, Array.Empty<byte>(), RegistryValueKind.None);
        }

        // 「既定のアプリ」設定画面への登録
        SetValue(Registry.CurrentUser, CapabilitiesPath, "ApplicationName", AppName);
        SetValue(Registry.CurrentUser, CapabilitiesPath, "ApplicationDescription", "シンプルな PDF ビューア");
        SetValue(Registry.CurrentUser, $@"{CapabilitiesPath}\FileAssociations", Extension, ProgId);
        SetValue(Registry.CurrentUser, RegisteredApplicationsPath, AppName, CapabilitiesPath);

        NotifyAssociationChanged();
    }

    /// <summary>
    /// <see cref="Register"/> で書き込んだ内容を削除する。
    /// </summary>
    public static void Unregister()
    {
        using (var classes = Registry.CurrentUser.OpenSubKey(ClassesPath, writable: true))
        {
            if (classes is not null)
            {
                classes.DeleteSubKeyTree(ProgId, throwOnMissingSubKey: false);
                using var openWith = classes.OpenSubKey($@"{Extension}\OpenWithProgids", writable: true);
                openWith?.DeleteValue(ProgId, throwOnMissingValue: false);
            }
        }

        Registry.CurrentUser.DeleteSubKeyTree(AppKeyPath, throwOnMissingSubKey: false);
        using (var registered = Registry.CurrentUser.OpenSubKey(RegisteredApplicationsPath, writable: true))
        {
            registered?.DeleteValue(AppName, throwOnMissingValue: false);
        }

        NotifyAssociationChanged();
    }

    /// <summary>
    /// 「既定のアプリ」設定画面の、このアプリのページを開く。
    /// </summary>
    public static void OpenDefaultAppsSettings()
    {
        Process.Start(new ProcessStartInfo($"ms-settings:defaultapps?registeredAppUser={AppName}")
        {
            UseShellExecute = true,
        });
    }

    private static void SetValue(RegistryKey root, string subKey, string? name, string value)
    {
        using var key = root.CreateSubKey(subKey);
        key.SetValue(name, value);
    }

    private static void NotifyAssociationChanged()
    {
        const int SHCNE_ASSOCCHANGED = 0x08000000;
        const uint SHCNF_IDLIST = 0x0000;
        SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
    }

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
}
