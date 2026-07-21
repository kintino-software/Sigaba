import { expect, it } from "bun:test";
import type { PlainData } from "@abstractions";
import { KeyGenerator } from "../services/KeyGenerator";
import { NonceGenerator } from "../services/NonceGenerator";
import { SymmetricCipher } from "./SymmetricCipher";

const key = new KeyGenerator().generateKey();
const nonce = new NonceGenerator().newNonce();

it("should encrypt and decrypt data correctly", () => {
	const symmetricCipher = new SymmetricCipher();
	const originalData: PlainData = Buffer.from("Hello, World!");
	const encryptedData = symmetricCipher.encrypt(originalData, key, nonce);
	const decryptedData = symmetricCipher.decrypt(encryptedData, key, nonce);
	expect(decryptedData).toEqual(originalData);
});
