import crypto from "node:crypto";

const TAG_SIZE_IN_BYTES = 16; // 128 bits
const ALGO_NAME: crypto.CipherGCMTypes = "aes-256-gcm";

const encrypt = (data: {
	plainData: Uint8Array;
	key: Uint8Array;
	nonce: Uint8Array;
}): Uint8Array => {
	const eas = crypto.createCipheriv(ALGO_NAME, data.key, data.nonce, {
		authTagLength: TAG_SIZE_IN_BYTES,
	});
	const cipher = Buffer.concat([eas.update(data.plainData), eas.final()]);
	const tag = eas.getAuthTag();
	return Buffer.concat([cipher, tag]);
};

const decrypt = (data: {
	encryptedData: Uint8Array;
	key: Uint8Array;
	nonce: Uint8Array;
}): Uint8Array => {
	const tag = data.encryptedData.slice(
		data.encryptedData.length - TAG_SIZE_IN_BYTES,
	);
	const encryptedDataWithoutTag = data.encryptedData.slice(
		0,
		data.encryptedData.length - TAG_SIZE_IN_BYTES,
	);
	const eas = crypto.createDecipheriv(ALGO_NAME, data.key, data.nonce, {
		authTagLength: TAG_SIZE_IN_BYTES,
	});
	eas.setAuthTag(tag);
	return Buffer.concat([eas.update(encryptedDataWithoutTag), eas.final()]);
};

export const Eas = {
	encrypt,
	decrypt,
};
