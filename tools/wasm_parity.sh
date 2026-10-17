#!/bin/bash
# Plays the parity script through build/web/platform.wasm in headless Chrome and writes build/parity_wasm.txt.
# (There is no node here; Chrome is the JavaScript engine that runs the shipped wasm anyway.)
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p build
cp tools/wasm_parity.html build/web/parity.html
PORT=$((20000 + RANDOM % 20000))
python3 -m http.server "$PORT" -d build/web >/dev/null 2>&1 &
SERVER=$!
trap 'kill $SERVER 2>/dev/null; rm -f build/web/parity.html' EXIT
sleep 1
dom=$(timeout 150 google-chrome --headless=new --no-sandbox --disable-gpu --virtual-time-budget=100000 --dump-dom "http://localhost:$PORT/parity.html" 2>/dev/null || true)
line=$(echo "$dom" | grep -o 'RESULT frames=[0-9]* state=[0-9]* steps=[0-9]*' || true)
if [ -z "$line" ]; then echo "wasm parity run failed:"; echo "$dom" | grep -o '<pre id="result">[^<]*' | head -3; exit 1; fi
echo "${line#RESULT }" > build/parity_wasm.txt
