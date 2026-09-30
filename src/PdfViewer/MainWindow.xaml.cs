using System.IO;
using System.Windows;
using Microsoft.Web.WebView2.Core;

namespace PdfViewer;

public partial class MainWindow : Window
{
    private readonly string _pdfPath;

    public MainWindow(string pdfPath)
    {
        InitializeComponent();

        _pdfPath = pdfPath;
        Title = $"{Path.GetFileName(pdfPath)} - PDF Viewer";
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // 既定の保存先は exe と同じフォルダーのため、書き込み可能な場所を指定する
            var userDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PdfViewer", "WebView2");
            var environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder);
            await WebView.EnsureCoreWebView2Async(environment);
        }
        catch (WebView2RuntimeNotFoundException)
        {
            MessageBox.Show(this,
                "WebView2 ランタイムが見つかりません。\nhttps://developer.microsoft.com/microsoft-edge/webview2/ からインストールしてください。",
                "PDF Viewer", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
            return;
        }

        WebView.CoreWebView2.DocumentTitleChanged += (_, _) =>
        {
            var title = WebView.CoreWebView2.DocumentTitle;
            if (!string.IsNullOrEmpty(title))
            {
                Title = $"{title} - PDF Viewer";
            }
        };
        WebView.Source = new Uri(_pdfPath);
    }
}
