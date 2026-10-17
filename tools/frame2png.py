#!/usr/bin/env python3
"""Turns frame text files (216 lines of 384 hue digits) into one PNG, scaled 2x, frames side by side.
Usage: tools/frame2png.py out.png frame1.txt [frame2.txt ...]. With the screens test: build/screens_failed/NAME.txt is the port's
frame and docs/reference/vb/NAME.txt the original's."""
import sys
from PIL import Image
PAL = [(0,0,0),(42,75,215),(29,105,20),(41,208,208),(173,35,35),(129,38,192),(129,74,25),(160,160,160),(87,87,87),(157,175,255),
       (129,197,122),(255,146,51),(255,205,243),(233,222,187),(255,238,51),(255,255,255)]
def load(path):
    rows = open(path).read().split("\n")[:216]
    im = Image.new("RGB", (384, 216))
    im.putdata([PAL[int(c, 16)] for r in rows for c in r])
    return im.resize((768, 432), Image.NEAREST)
frames = [load(p) for p in sys.argv[2:]]
sheet = Image.new("RGB", (768 * len(frames) + 8 * (len(frames) - 1), 432), (255, 0, 255))
for i, f in enumerate(frames): sheet.paste(f, (i * 776, 0))
sheet.save(sys.argv[1])
