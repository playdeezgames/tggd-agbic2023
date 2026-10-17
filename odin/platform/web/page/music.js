// The game's music, synthesized with Web Audio (no audio files): a square-wave lead over a triangle-wave bass, one short loop per
// theme. The original's recorded music is not shipped (user decision, phase 7); these themes are placeholders written for this
// port and are meant to be easy to replace: edit THEMES, or swap makeMusic() for something else with the same three methods.
// Theme ids follow the core's Mux_Id: 1 main, 2 combat, 3 victory, 4 death. A step is an eighth note; numbers are MIDI notes, 0 a rest.
const THEMES = [
	null,
	{ // main: C major, easy walking pace
		bpm: 112,
		lead: [72,0,76,0,79,0,76,0, 77,0,81,0,79,0,76,0, 74,0,77,0,81,0,77,0, 76,0,72,0,67,0,0,0],
		bass: [48,0,48,0,55,0,55,0, 53,0,53,0,55,0,55,0, 50,0,50,0,57,0,57,0, 55,0,55,0,48,0,0,0],
	},
	{ // combat: A minor, driving
		bpm: 168,
		lead: [69,72,76,72,69,72,76,72, 67,71,74,71,67,71,74,71, 65,69,72,69,65,69,72,69, 64,68,71,68,64,68,71,0],
		bass: [45,45,0,45,45,0,52,0, 43,43,0,43,43,0,50,0, 41,41,0,41,41,0,48,0, 40,40,0,40,40,0,47,0],
	},
	{ // victory: a bright fanfare
		bpm: 120,
		lead: [72,0,72,72,76,0,79,0, 84,0,0,0,79,0,84,0, 83,0,79,0,76,0,79,0, 84,0,0,0,0,0,0,0],
		bass: [48,0,0,0,55,0,0,0, 53,0,0,0,55,0,0,0, 55,0,0,0,52,0,0,0, 48,0,0,0,0,0,0,0],
	},
	{ // death: D minor, slow and low
		bpm: 66,
		lead: [62,0,0,0,60,0,0,0, 57,0,0,0,53,0,0,0, 55,0,0,0,57,0,0,0, 50,0,0,0,0,0,0,0],
		bass: [38,0,0,0,0,0,0,0, 36,0,0,0,0,0,0,0, 31,0,0,0,0,0,0,0, 38,0,0,0,0,0,0,0],
	},
];

// makeMusic(actx) -> { play(id), stop(), setVolume(v) }
function makeMusic(actx) {
	const master = actx.createGain();
	master.gain.value = 0.3;
	master.connect(actx.destination);
	let timer = null, theme = null, step = 0, nextTime = 0;
	const midi = (n) => 440 * Math.pow(2, (n - 69) / 12);
	function note(n, when, len, type, level) {
		const osc = actx.createOscillator(), g = actx.createGain();
		osc.type = type; osc.frequency.value = midi(n);
		g.gain.setValueAtTime(0.0001, when);
		g.gain.linearRampToValueAtTime(level, when + 0.01);
		g.gain.exponentialRampToValueAtTime(0.0001, when + len * 0.95);
		osc.connect(g); g.connect(master);
		osc.start(when); osc.stop(when + len);
	}
	function tick() {
		if (!theme) return;
		const len = 60 / theme.bpm / 2;
		while (nextTime < actx.currentTime + 0.25) {
			const i = step % theme.lead.length;
			if (theme.lead[i]) note(theme.lead[i], nextTime, len, "square", 0.12);
			if (theme.bass[i]) note(theme.bass[i], nextTime, len * 1.6, "triangle", 0.2);
			nextTime += len; step++;
		}
	}
	return {
		play(id) {
			this.stop();
			theme = THEMES[id] || null;
			if (!theme) return;
			step = 0; nextTime = actx.currentTime + 0.05;
			tick(); timer = setInterval(tick, 60);
		},
		stop() { if (timer) clearInterval(timer); timer = null; theme = null; },
		setVolume(v) { master.gain.value = v; },
		get playing() { return theme !== null; },
	};
}
