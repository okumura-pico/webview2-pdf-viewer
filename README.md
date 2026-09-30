# PDF Viewer for Windows

WebView2を使用したシンプルなPDFビューアです。
PDFファイルダブルクリックで使用することを想定しています。

## 配布用ビルド

```sh
dotnet publish src/PdfViewer/PdfViewer.csproj -p:PublishProfile=win-x64
```

`src/PdfViewer/bin/publish/win-x64/PdfViewer.exe` が単一の exe として出力されます。
.NET ランタイムは同梱されるため、配布先へのインストールは不要です（WebView2 ランタイムは必要です）。
