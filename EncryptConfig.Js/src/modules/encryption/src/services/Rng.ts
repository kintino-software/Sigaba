import crypto from "node:crypto";

const createRandomBytes = (length: number): Uint8Array => {
	if (length <= 0) {
		throw new Error("Length must be greater than 0");
	}
	return crypto.randomBytes(length);
};

export const Rng = { createRandomBytes };
