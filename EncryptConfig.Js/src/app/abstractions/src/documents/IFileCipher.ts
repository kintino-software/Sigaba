import type { PlainKey } from "../primitives";

export interface IFileCipher {
	encryptFile(filePath: string, key: PlainKey): Promise<void>;
	decryptFile(filePath: string, key: PlainKey): Promise<void>;
}
