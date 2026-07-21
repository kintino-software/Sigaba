import type { IFileCipher, PlainKey } from "@abstractions";

export class FileCipher implements IFileCipher {
	encryptFile(filePath: string, key: PlainKey): Promise<void> {
		throw new Error("Method not implemented.");
	}
	decryptFile(filePath: string, key: PlainKey): Promise<void> {
		throw new Error("Method not implemented.");
	}
}
