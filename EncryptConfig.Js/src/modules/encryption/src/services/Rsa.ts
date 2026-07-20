import * as crypto from "node:crypto";

const keySizeInBits = 3072;

const generateKeys = () => {
	const { publicKey, privateKey } = crypto.generateKeyPairSync("rsa", {
		modulusLength: keySizeInBits,
		publicKeyEncoding: {
			type: "spki",
			format: "pem",
		},
		privateKeyEncoding: {
			type: "pkcs8",
			format: "pem",
		},
	});
	return { publicKey, privateKey };
};

const encrypt = (plainData: Uint8Array, publicKey: string): Uint8Array => {
	const encryptedData = crypto.publicEncrypt(publicKey, plainData);
	return encryptedData;
};

const decrypt = (encryptedData: Uint8Array, privateKey: string): Uint8Array => {
	const decryptedData = crypto.privateDecrypt(privateKey, encryptedData);
	return decryptedData;
};

export const RSA = {
	generateKeys,
	encrypt,
	decrypt,
};
