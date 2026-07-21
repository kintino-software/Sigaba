import { CipherPack } from "./CipherPack";

it("should pack", () => {
	const cipherPack = new CipherPack(
		new Uint8Array([1, 2, 3, 4, 5]),
		new Uint8Array([6, 7, 8, 9, 10]),
	);
	const pack = CipherPack.pack(cipherPack);
	expect(pack).toMatch(/^ENC\(.+\)$/);
	expect(pack.length).toBeGreaterThan(0);
});

it("should pack and unpack consistently", () => {
	const original = new CipherPack(
		new Uint8Array([1, 2, 3, 4, 5]),
		new Uint8Array([6, 7, 8, 9, 10]),
	);
	const pack = CipherPack.pack(original);
	const unpacked = CipherPack.unpack(pack);
	expect(unpacked).toEqual(original);
});
