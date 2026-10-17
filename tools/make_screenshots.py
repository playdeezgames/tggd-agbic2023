#!/usr/bin/env python3
"""Writes assets/screenshots/ from frames of the original game recorded by tools/oracle.sh (docs/reference/vb), which the port
reproduces pixel for pixel (odin/tests/screens_test.odin). Each is the 384 by 216 frame at 3x, plain nearest-neighbour (1152 by 648).
Usage: tools/make_screenshots.py   (needs Pillow)"""
import os
from PIL import Image
ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..")
PAL = [(0,0,0),(42,75,215),(29,105,20),(41,208,208),(173,35,35),(129,38,192),(129,74,25),(160,160,160),(87,87,87),(157,175,255),
       (129,197,122),(255,146,51),(255,205,243),(233,222,187),(255,238,51),(255,255,255)]
SHOTS = [("01-town", "town_center"), ("02-inn", "room_inn"), ("03-wilderness", "wild_items"), ("04-actions", "actions_full"),
         ("05-fight", "combat_three"), ("06-innkeeper", "gorachan_talk")]
out = os.path.join(ROOT, "assets", "screenshots")
os.makedirs(out, exist_ok=True)
for name, shot in SHOTS:
    rows = open(os.path.join(ROOT, "docs", "reference", "vb", shot + ".txt")).read().split("\n")[:216]
    im = Image.new("RGB", (384, 216))
    im.putdata([PAL[int(c, 16)] for r in rows for c in r])
    im.resize((1152, 648), Image.NEAREST).save(os.path.join(out, name + ".png"))
    print("wrote", name)
