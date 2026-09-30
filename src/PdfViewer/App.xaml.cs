using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace PdfViewer;

public partial class App : Application
{
    private void OnStartup(object sender, StartupEventArgs e)
    {
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

    private static string? SelectPdfFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "PDF ファイル (*.pdf)|*.pdf|すべてのファイル (*.*)|*.*",
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
