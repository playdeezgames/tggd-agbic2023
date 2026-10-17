// Browser side of the platform interface: draw one RGBA frame, turn keys and taps into commands.
(async function () {
	const FRAME_W = 384, FRAME_H = 216;
	const COMMAND = { up: 1, down: 2, left: 3, right: 4, confirm: 5, cancel: 6 };
	const debugLog = new URLSearchParams(location.search).has("log"); // ?log=1 prints every input sent to the game
	const logInput = (...a) => { if (debugLog) console.log("[input]", ...a); };

	const seedParam = new URLSearchParams(location.search).get("seed");
	const fixedSeed = seedParam !== null && /^\d{1,9}$/.test(seedParam) ? Number(seedParam) : null;
	let fixedSeedHalf = 0;

	const mem = new odin.WasmMemoryInterface();
	const platformImports = {
		// ?seed=N fixes the random numbers (bug reports, tests): the high word is 0 and the low word is N
		js_entropy_u32() {
			if (fixedSeed !== null) { fixedSeedHalf ^= 1; return fixedSeedHalf ? 0 : fixedSeed; }
			return crypto.getRandomValues(new Uint32Array(1))[0] | 0;
		},
		js_log(p, n) { console.log(mem.loadString(p, n)); },
		// export: offer the save as a file download
		js_download_text(fp, fn, tp, tn) {
			const blob = new Blob([mem.loadString(tp, tn)], { type: "application/json" });
			const a = document.createElement("a");
			a.href = URL.createObjectURL(blob); a.download = mem.loadString(fp, fn);
			document.body.appendChild(a); a.click(); a.remove();
			setTimeout(() => URL.revokeObjectURL(a.href), 1000);
		},
		// import: a real file input in a small panel, so the click that opens it is the player's own (browsers insist)
		js_request_file() {
			if (document.getElementById("filepanel")) return;
			const panel = document.createElement("div");
			panel.id = "filepanel";
			panel.style.cssText = "position:fixed;left:50%;top:40%;transform:translate(-50%,-50%);background:#222;color:#eee;padding:16px;border:2px solid #ff9233;font:16px monospace;text-align:center;z-index:10";
			panel.innerHTML = "<div>Choose a Bagel Quest save file</div><br>";
			const input = document.createElement("input"); input.type = "file"; input.accept = ".json,application/json";
			const cancel = document.createElement("button"); cancel.textContent = "Cancel"; cancel.style.marginLeft = "8px";
			const close = () => { panel.remove(); canvas.focus(); };
			input.onchange = async () => {
				const f = input.files[0]; close();
				if (!f) return;
				const bytes = new Uint8Array(await f.arrayBuffer());
				const ptr = exports.platform_alloc(bytes.length);
				new Uint8Array(mem.memory.buffer, ptr, bytes.length).set(bytes);
				exports.platform_file(bytes.length);
			};
			cancel.onclick = close;
			panel.append(input, cancel); document.body.appendChild(panel);
		},
	};
	await odin.runWasm("platform.wasm", null, { ...storageImports(mem), platform: platformImports }, mem);
	const exports = mem.exports;

	// ---- layout: the largest whole multiple of the frame that fits, else a smooth fit; centred on black ----------
	const canvas = document.getElementById("screen"), ctx = canvas.getContext("2d");
	let laidOutFor = "";
	function layout() {
		const w = innerWidth, h = innerHeight;
		laidOutFor = w + "x" + h;
		if (w === 0 || h === 0) return;
		let scale = Math.min(w / FRAME_W, h / FRAME_H);
		if (scale >= 1) scale = Math.floor(scale);
		const cw = Math.floor(FRAME_W * scale), ch = Math.floor(FRAME_H * scale);
		canvas.style.width = cw + "px"; canvas.style.height = ch + "px";
		canvas.style.left = Math.floor((w - cw) / 2) + "px"; canvas.style.top = Math.floor((h - ch) / 2) + "px";
	}
	addEventListener("resize", layout); layout();

	// ---- audio. Browsers keep sound off until the player does something, so everything starts on the first key, click or touch.
	let actx = null, sfxBuffers = [], synth = null, unlocked = false, wantedMusic = 0;
	async function loadSfx() {
		for (let i = 1; i < SFX_FILES.length; i++) {
			try {
				const data = await (await fetch("audio/sfx/" + SFX_FILES[i])).arrayBuffer();
				sfxBuffers[i] = await actx.decodeAudioData(data);
			} catch (e) { console.warn("sound", SFX_FILES[i], "failed:", e); }
		}
	}
	// the music is synthesized (music.js): switching theme restarts the loop, as the original restarted its song
	function setMusic(id) {
		wantedMusic = id;
		if (!unlocked || !synth) return;
		synth.setVolume(exports.platform_mux_volume());
		synth.play(id);
	}
	function unlock() {
		if (unlocked) return;
		unlocked = true;
		try {
			actx = new (window.AudioContext || window.webkitAudioContext)();
			actx.resume();
			synth = makeMusic(actx);
			loadSfx();
		} catch (e) { console.warn("no Web Audio:", e); }
		setMusic(wantedMusic);
	}
	for (const ev of ["keydown", "pointerdown", "touchstart"]) addEventListener(ev, unlock, { capture: true });
	function playSfx(id, volume) {
		const buf = sfxBuffers[id];
		if (!actx || !buf || volume <= 0) return;
		const src = actx.createBufferSource(), gain = actx.createGain();
		src.buffer = buf; gain.gain.value = volume;
		src.connect(gain); gain.connect(actx.destination); src.start();
	}

	// ---- input: keys, gamepad and touch buttons all feed one "is this command held" table, and a held command repeats the way the
	// original's Host does: it fires at once, again after 1.0 s, then every 0.3 s. KEYS_BY_KEY / KEYS_BY_CODE come from keys.js
	// (generated from the original keys.json); the logical key (e.key) is read before the physical code, because remote desktops can
	// send wrong codes.
	const NAMES = ["up", "down", "left", "right", "confirm", "cancel"];
	const held = Object.fromEntries(NAMES.map((n) => [n, new Set()])); // who holds each command
	const tapped = new Set();   // commands pressed and released between two frames still count once
	const nextFire = {};        // command -> time of its next repeat
	const REPEAT_FIRST = 1000, REPEAT_NEXT = 300;
	const nameOf = (e) => KEYS_BY_KEY[e.key] || KEYS_BY_CODE[e.code];
	addEventListener("keydown", (e) => {
		const name = nameOf(e);
		if (!name) return;
		e.preventDefault();
		if (!held[name].has(e.code || e.key)) tapped.add(name);
		held[name].add(e.code || e.key);
		logInput("key", e.key, e.code, "->", name);
	});
	addEventListener("keyup", (e) => {
		const name = nameOf(e);
		if (name) held[name].delete(e.code || e.key);
	});
	addEventListener("blur", () => { for (const n of NAMES) held[n].clear(); });
	addEventListener("contextmenu", (e) => e.preventDefault());

	// gamepad: the d-pad and the left stick move, A confirms, B cancels (standard mapping only)
	function pollGamepads() {
		const pads = navigator.getGamepads ? navigator.getGamepads() : [];
		for (const n of NAMES) held[n].delete("pad");
		for (const p of pads) {
			if (!p || p.mapping !== "standard") continue;
			const b = (i) => p.buttons[i] && p.buttons[i].pressed;
			const on = { up: b(12) || p.axes[1] < -0.5, down: b(13) || p.axes[1] > 0.5, left: b(14) || p.axes[0] < -0.5,
				right: b(15) || p.axes[0] > 0.5, confirm: b(0), cancel: b(1) };
			for (const n of NAMES) if (on[n]) held[n].add("pad");
			if (Object.values(on).some(Boolean)) unlock();
		}
	}

	// touch: on-screen buttons, shown on a touch screen (or after the first touch)
	const pad = document.createElement("div");
	pad.id = "touchpad";
	pad.innerHTML = '<button data-c="up" style="left:64px;bottom:128px">&#9650;</button><button data-c="left" style="left:0;bottom:64px">&#9664;</button>' +
		'<button data-c="right" style="left:128px;bottom:64px">&#9654;</button><button data-c="down" style="left:64px;bottom:0">&#9660;</button>' +
		'<button data-c="cancel" style="right:80px;bottom:16px">B</button><button data-c="confirm" style="right:0;bottom:80px">A</button>';
	document.body.appendChild(pad);
	let padShown = false;
	function showPad() { if (!padShown) { padShown = true; pad.style.display = "block"; } }
	if (matchMedia("(pointer: coarse)").matches) showPad();
	addEventListener("touchstart", showPad, { once: true, passive: true });
	for (const btn of pad.querySelectorAll("button")) {
		const c = btn.dataset.c;
		btn.addEventListener("pointerdown", (e) => {
			e.preventDefault();
			try { btn.setPointerCapture(e.pointerId); } catch (_) {}
			if (!held[c].has("touch:" + e.pointerId)) tapped.add(c);
			held[c].add("touch:" + e.pointerId);
		});
		const release = (e) => held[c].delete("touch:" + e.pointerId);
		btn.addEventListener("pointerup", release); btn.addEventListener("pointercancel", release);
	}

	function sendCommands(now) {
		for (const n of NAMES) {
			const isHeld = held[n].size > 0;
			let fire = false;
			if (isHeld) {
				if (!(n in nextFire)) { fire = true; nextFire[n] = now + REPEAT_FIRST; }
				else if (now > nextFire[n]) { fire = true; nextFire[n] = now + REPEAT_NEXT; }
			} else {
				delete nextFire[n];
				if (tapped.has(n)) fire = true;
			}
			if (fire) { exports.platform_command(COMMAND[n]); logInput("command", n); }
		}
		tapped.clear();
	}

	// ---- full screen: the Options menu's toggle (a key press counts as the player's click)
	let lastFullscreen = false;
	function syncFullscreen() {
		const want = exports.platform_fullscreen();
		if (want === lastFullscreen) return;
		lastFullscreen = want;
		try {
			if (want && !document.fullscreenElement) document.documentElement.requestFullscreen();
			else if (!want && document.fullscreenElement) document.exitFullscreen();
		} catch (_) {}
	}

	// ---- the frame loop ---------------------------------------------------------------------------------
	let prev = performance.now(), lastVolume = -1;
	function frame(now) {
		if (laidOutFor !== innerWidth + "x" + innerHeight) layout(); // a pane that was hidden at load reports 0 by 0
		pollGamepads();
		sendCommands(now);
		exports.platform_frame(Math.min((now - prev) / 1000, 0.25), Date.now()); prev = now;
		const ptr = exports.platform_frame_ptr();
		if (ptr && exports.platform_frame_changed()) {
			ctx.putImageData(new ImageData(new Uint8ClampedArray(mem.memory.buffer, ptr, FRAME_W * FRAME_H * 4), FRAME_W, FRAME_H), 0, 0);
		}
		const sfxVolume = exports.platform_sfx_volume();
		for (let i = 0, n = exports.platform_sfx_count(); i < n; i++) playSfx(exports.platform_sfx(i), sfxVolume);
		const id = exports.platform_mux();
		if (id !== wantedMusic) setMusic(id);
		const mv = exports.platform_mux_volume();
		if (synth && mv !== lastVolume) { synth.setVolume(mv); lastVolume = mv; }
		syncFullscreen();
		requestAnimationFrame(frame);
	}
	window.__platform = { exports, mem, held, get unlocked() { return unlocked; }, get music() { return synth; } };
	requestAnimationFrame(frame);
})();
