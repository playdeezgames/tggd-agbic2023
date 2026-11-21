#!/usr/bin/env bash
# Builds the shipping web build and copies it to docs/ for GitHub Pages (served from main, /docs):
# https://playdeezgames.github.io/tggd-agbic2023/
# docs/ is generated and wiped on each run; the port's notes and VB reference screens live in design/. Never commits or pushes.
set -euo pipefail
cd "$(dirname "$0")/.."
rm -rf build/web
ODIN_FLAGS="-o:size" tools/build.sh web
rm -rf docs
mkdir docs
cp -r build/web/. docs/
touch docs/.nojekyll
echo "docs/ ready: $(du -sh docs | cut -f1)"
