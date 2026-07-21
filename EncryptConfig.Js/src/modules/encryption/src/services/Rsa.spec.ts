import { expect, it } from "bun:test";
import { Rsa } from "./Rsa";

it("should generate RSA keys", () => {
	const { publicKey, privateKey } = Rsa.generateKeys();
	expect(publicKey).toBeDefined();
	expect(privateKey).toBeDefined();
});

it("should encrypt and decrypt data using RSA", () => {
	const { publicKey, privateKey } = Rsa.generateKeys();
	const plainData = new Uint8Array([1, 2, 3, 4, 5]);
	const encryptedData = Rsa.encrypt(plainData, publicKey);
	const decryptedData = Rsa.decrypt(encryptedData, privateKey);
	expect(decryptedData).toEqual(plainData);
});

it("should throw an error when decrypting with the wrong private key", () => {
	const { publicKey } = Rsa.generateKeys();
	const { privateKey: wrongPrivateKey } = Rsa.generateKeys();
	const plainData = new Uint8Array([1, 2, 3, 4, 5]);
	const encryptedData = Rsa.encrypt(plainData, publicKey);
	expect(() => Rsa.decrypt(encryptedData, wrongPrivateKey)).toThrow();
});
