package game

// The game's own generator (xoshiro256** seeded by splitmix64), so a seed gives the same world and the same rolls on every
// target and tests can seed it. The original used System.Random, which cannot be seeded, so its rolls cannot be replayed
// (PORT.md decision 5); the dice grammar of the data ("1d6/6") is kept.

Rng :: struct { s: [4]u64 }

rng_seed :: proc(r: ^Rng, seed: u64) {
	x := seed
	for i in 0 ..< 4 {
		x += 0x9E3779B97F4A7C15
		z := x
		z = (z ~ (z >> 30)) * 0xBF58476D1CE4E5B9
		z = (z ~ (z >> 27)) * 0x94D049BB133111EB
		r.s[i] = z ~ (z >> 31)
	}
}

rotl :: proc "contextless" (x: u64, k: uint) -> u64 { return (x << k) | (x >> (64 - k)) }

rng_next :: proc(r: ^Rng) -> u64 {
	result := rotl(r.s[1] * 5, 7) * 9
	t := r.s[1] << 17
	r.s[2] ~= r.s[0]
	r.s[3] ~= r.s[1]
	r.s[1] ~= r.s[2]
	r.s[0] ~= r.s[3]
	r.s[2] ~= t
	r.s[3] = rotl(r.s[3], 45)
	return result
}

// A uniform integer in [lo, hi] (inclusive), without modulo bias. Requires lo <= hi.
rng_range :: proc(r: ^Rng, lo, hi: int) -> int {
	span := u64(hi - lo) + 1
	limit := (max(u64) / span) * span // draws at or above this would favour the low values
	for {
		x := rng_next(r)
		if x < limit { return lo + int(x % span) }
	}
}

// Sum of `count` dice of `size` sides.
roll_xdy :: proc(r: ^Rng, count, size: int) -> int {
	total := 0
	for _ in 0 ..< count { total += rng_range(r, 1, size) }
	return total
}

// The data's "NdS/D" form: the dice total is integer-divided by D (so "1d6/6" is 1 only on a 6, "6d6/6" is the total over 6).
roll_dice :: proc(r: ^Rng, count, size: int, divisor := 1) -> int { return roll_xdy(r, count, size) / divisor }
