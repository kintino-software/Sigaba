import { expect, it } from "bun:test";
import { Rng } from "./Rng";

it("Should return filled byte array", () => {
	const length = 16;

	const randomBytes = Rng.createRandomBytes(length);

	expect(randomBytes.length).toBe(length);
	expect(randomBytes.every((byte) => byte >= 0 && byte <= 255)).toBe(true);
});

it("Should throw error for non-positive length", () => {
	const length = 0;

	expect(() => Rng.createRandomBytes(length)).toThrowError(
		"Length must be greater than 0",
	);
});
