import type { EncryptedData, Nonce, PlainData, PlainKey } from "../primitives";

export interface ISymmetricEncryption {
	encrypt(plainData: PlainData, key: PlainKey, nonce: Nonce): EncryptedData;
	decrypt(encryptedData: EncryptedData, key: PlainKey, nonce: Nonce): PlainData;
}
