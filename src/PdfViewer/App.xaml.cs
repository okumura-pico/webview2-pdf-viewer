using System.IO;
using System.Security;
using System.Windows;
using Microsoft.Win32;

namespace PdfViewer;

public partial class App : Application
{
    private void OnStartup(object sender, StartupEventArgs e)
    {
        var exePath = Environment.ProcessPath;

        switch (e.Args)
        {
            case ["--register"] when exePath is not null:
                RunAssociationCommand(() =>
                {
                    FileAssociation.Register(exePath);
                    FileAssociation.OpenDefaultAppsSettings();
                });
                return;
            case ["--unregister"]:
                RunAssociationCommand(() =>
                {
                    FileAssociation.Unregister();
                    MessageBox.Show("PDF ファイルとの関連付けを解除しました。", "PDF Viewer",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                });
                return;
        }

        if (exePath is not null)
        {
            try
            {
                FileAssociation.Register(exePath);
            }
            catch (Exception ex) when (ex is UnauthorizedAccessException or SecurityException or IOException)
            {
                // 登録できなくても PDF の表示には影響しないため無視する
            }
        }

        var path = e.Args.Length > 0 ? e.Args[0] : SelectPdfFile();
        if (path is null)
        {
            Shutdown();
            return;
        }

        if (!File.Exists(path))
        {
            MessageBox.Show($"ファイルが見つかりません。\n{path}", "PDF Viewer",
                MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
            return;
        }

        new MainWindow(Path.GetFullPath(path)).Show();
    }

    private void RunAssociationCommand(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"関連付けの処理に失敗しました。\n{ex.Message}", "PDF Viewer",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
        Shutdown();
    }

    private static string? SelectPdfFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "PDF ファイル (*.pdf)|*.pdf|すべてのファイル (*.*)|*.*",
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
