#!/bin/bash
# Re-records the reference screens from the original VB UI into docs/reference/vb/ by replaying docs/reference/scenes.txt.
# Needs the dotnet SDK. The frames are committed; run this only when scenes.txt changes.
set -euo pipefail
root="$(cd "$(dirname "$0")/.." && pwd)"
dotnet build "$root/tools/vb-oracle/Oracle.csproj" -o "$root/build/oracle" -v q --nologo >/dev/null
rm -f "$root"/docs/reference/vb/*.txt
cd "$root/src/BagelQuest"
dotnet "$root/build/oracle/Oracle.dll" "$root/docs/reference/scenes.txt" "$root/docs/reference/vb"
