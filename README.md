# PDF Viewer for Windows

WebView2を使用したシンプルなPDFビューアです。
PDFファイルダブルクリックで使用することを想定しています。

## PDF ファイルへの関連付け

起動すると、PDF を開けるアプリの候補として自動で登録されます（現在のユーザーのみ、管理者権限は不要）。
その後、次のどちらかで既定のアプリに設定してください。

- PDF ファイルを右クリック →「プログラムから開く」→「別のアプリを選択」→ PdfViewerWV を選び「常に使う」
- exe を `--register` を付けて実行し、開いた「既定のアプリ」設定画面で `.pdf` に PdfViewerWV を選ぶ

exe を移動した場合は、移動先で一度起動すると登録が更新されます。
削除する前に exe を `--unregister` を付けて実行すると、登録を解除できます。

## 配布用ビルド

```sh
dotnet publish src/PdfViewer/PdfViewer.csproj -p:PublishProfile=win-x64
```

`src/PdfViewer/bin/publish/win-x64/PdfViewerWV.exe` が単一の exe として出力されます。
.NET ランタイムは同梱されるため、配布先へのインストールは不要です（WebView2 ランタイムは必要です）。
