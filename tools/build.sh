#!/bin/bash
# Builds the Odin port. Usage: tools/build.sh [web]   (default: web). Output goes to build/ (git-ignored).
# ODIN_FLAGS adds compiler flags (tools/ship.sh passes -o:size).
set -euo pipefail
cd "$(dirname "$0")/.."
what="${1:-web}"
OPTS="-collection:bq=odin -vet-shadowing -vet-unused"

build_web() {
	mkdir -p build/web
	odin build odin/platform/web -target:js_wasm32 -out:build/web/platform.wasm $OPTS ${ODIN_FLAGS:-}
	cp odin/platform/web/page/index.html odin/platform/web/page/platform.js odin/platform/web/page/keys.js odin/platform/web/page/storage.js odin/platform/web/page/sounds.js odin/platform/web/page/music.js build/web/
	cp "$(odin root)/core/sys/wasm/js/odin.js" build/web/odin.js
	rm -rf build/web/audio; mkdir -p build/web/audio/sfx
	cp src/BagelQuest/Content/Audio/Sfx/*.wav build/web/audio/sfx/
	echo "web build: build/web (serve with tools/serve.sh)"
}
case "$what" in
	web) build_web ;;
	*) echo "usage: $0 [web]"; exit 2 ;;
esac
