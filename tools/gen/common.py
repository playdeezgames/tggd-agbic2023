"""Shared helpers for the generators (stdlib only)."""
import json, os, re

ROOT = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "..")
CONTENT = os.path.join(ROOT, "src", "BagelQuest", "Content")

# AOS.UI BoilerplateHue, in hue-index order
HUE_NAMES = ["Black", "Blue", "Green", "Cyan", "Red", "Purple", "Brown", "Light_Gray", "Dark_Gray", "Light_Blue",
             "Light_Green", "Orange", "Pink", "Tan", "Yellow", "White"]

def load(name):
    with open(os.path.join(CONTENT, name), encoding="utf-8-sig") as f:
        return json.load(f)

def read_text(name):
    with open(os.path.join(CONTENT, name), encoding="utf-8-sig") as f:
        return f.read()

def snake(name):
    """CamelCase to Ada_Case: RiverNESW -> River_NESW, LearnHatchedMaking -> Learn_Hatched_Making."""
    return re.sub(r"(?<=[a-z0-9])(?=[A-Z])", "_", name)

class Strict:
    """A JSON object read through a schema: every property must be taken, or finish() fails.
    This is what makes the generator the complete list of how the port reads the original data."""
    def __init__(self, data, where):
        assert isinstance(data, dict), f"{where}: expected an object"
        self.data, self.where, self.taken = data, where, set()
    def take(self, key):
        if key not in self.data:
            raise SystemExit(f"{self.where}: missing property {key!r}")
        self.taken.add(key)
        return self.data[key]
    def take_empty(self, key, what="empty"):
        v = self.take(key)
        if v not in ({}, [], "", None):
            raise SystemExit(f"{self.where}.{key}: expected {what}, got {v!r}")
    def finish(self):
        extra = set(self.data) - self.taken
        if extra:
            raise SystemExit(f"{self.where}: unknown properties {sorted(extra)} (add them to the schema or to IGNORED)")
