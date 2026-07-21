import type {
	EncryptedData,
	ISymmetricEncryption,
	Nonce,
	PlainData,
	PlainKey,
} from "@abstractions";
import { Eas } from "../services/Eas";

export class SymmetricCipher implements ISymmetricEncryption {
	encrypt(plainData: PlainData, key: PlainKey, nonce: Nonce): EncryptedData {
		return Eas.encrypt({
			plainData: plainData,
			key: key,
			nonce: nonce,
		});
	}
	decrypt(
		encryptedData: EncryptedData,
		key: PlainKey,
		nonce: Nonce,
	): PlainData {
		return Eas.decrypt({
			encryptedData: encryptedData,
			key: key,
			nonce: nonce,
		});
	}
}
