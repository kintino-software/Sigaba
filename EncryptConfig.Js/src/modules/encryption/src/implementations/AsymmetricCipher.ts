import type {
	EncryptedData,
	IAsymmetricEncryption,
	PlainData,
	PrivateKey,
	PublicKey,
} from "../../../../app/abstractions";
import { Rsa } from "../services/Rsa";

export class AsymmetricCipher implements IAsymmetricEncryption {
	generateKeys(): { publicKey: PublicKey; privateKey: PrivateKey } {
		const { publicKey, privateKey } = Rsa.generateKeys();
		return {
			publicKey: Buffer.from(publicKey),
			privateKey: Buffer.from(privateKey),
		};
	}

	encrypt(plainData: PlainData, publicKey: PublicKey): EncryptedData {
		return Rsa.encrypt(plainData, publicKey.toHex());
	}

	decrypt(encryptedData: EncryptedData, privateKey: PrivateKey): PlainData {
		return Rsa.decrypt(encryptedData, privateKey.toHex());
	}
}
