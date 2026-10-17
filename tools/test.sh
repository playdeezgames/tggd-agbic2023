#!/bin/bash
# Runs every test of the Odin port; exits non-zero on any failure. Usage: tools/test.sh
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p build
fail=0
step() { echo; echo "== $1"; }

step "generated files are up to date (fonts and palette, content tables, key map)"
gen=(odin/game/assets_data.odin odin/game/content_data.odin odin/platform/web/page/keys.js odin/platform/web/page/sounds.js)
for f in "${gen[@]}"; do cp "$f" "build/$(basename "$f").before"; done
python3 tools/gen/gen_assets.py >/dev/null
python3 tools/gen/gen_content.py >/dev/null
for f in "${gen[@]}"; do
	cmp -s "$f" "build/$(basename "$f").before" && echo "ok: $f" || { echo "FAIL: $f changed when regenerated; commit the result"; fail=1; }
done

step "native suite (odin test)"
odin test odin/tests -collection:bq=odin -o:speed -out:build/tests_native -define:ODIN_TEST_THREADS=1 2>&1 | tee build/native_tests.log | grep -E "FAIL|passed|failed|Finished|error|Error" || true
grep -qE "All tests were successful|successful" build/native_tests.log || fail=1

step "web build (with the vet flags, so 32-bit and shadowing mistakes show)"
tools/build.sh web >/dev/null && echo "ok" || { echo "FAIL: the web platform does not build"; fail=1; }

step "wasm plays exactly like native (the same scripted game, 4,000 steps, in headless Chrome)"
if tools/wasm_parity.sh; then
	if cmp -s build/parity_native.txt build/parity_wasm.txt; then echo "ok: $(cat build/parity_native.txt)"; else echo "FAIL: native and wasm digests differ"; cat build/parity_native.txt build/parity_wasm.txt; fail=1; fi
else fail=1; fi

echo
if [ "$fail" = 0 ]; then echo "ALL TESTS PASSED"; else echo "SOME TESTS FAILED"; exit 1; fi
