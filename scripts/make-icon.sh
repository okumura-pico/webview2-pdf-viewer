#!/usr/bin/env bash
# src/PdfViewer/Assets/icon.svg から app.ico を生成する。
# 必要なツール: rsvg-convert (librsvg), magick (ImageMagick 7)
set -euo pipefail

assets="$(cd "$(dirname "$0")/../src/PdfViewer/Assets" && pwd)"
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

pngs=()
for size in 16 24 32 48 64 128 256; do
    rsvg-convert -w "$size" -h "$size" "$assets/icon.svg" -o "$work/icon-$size.png"
    pngs+=("$work/icon-$size.png")
done

magick "${pngs[@]}" "$assets/app.ico"
echo "Generated $assets/app.ico"
