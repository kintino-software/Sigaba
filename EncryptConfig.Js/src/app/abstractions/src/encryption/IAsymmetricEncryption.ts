import type {
	EncryptedData,
	PlainData,
	PrivateKey,
	PublicKey,
} from "../primitives";

export interface IAsymmetricEncryption {
	generateKeys(): { publicKey: PublicKey; privateKey: PrivateKey };

	encrypt(plainData: PlainData, publicKey: PublicKey): EncryptedData;

	decrypt(encryptedData: EncryptedData, privateKey: PrivateKey): PlainData;
}
