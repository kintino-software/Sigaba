import type { EncryptedData, Nonce } from "@abstractions";

// slim type to serialize CipherPack to json and back
// wraps values in base64 to avoid issues with binary data in json
type PackObj = {
	data: string; // base64 encoded
	nonce: string; // base64 encoded
};

const ENCRYPTION_PREFIX = "ENC(";
const ENCRYPTION_SUFFIX = ")";

export class CipherPack {
	constructor(
		public readonly data: EncryptedData,
		public readonly nonce: Nonce,
	) {}

	static pack(cipherPack: CipherPack): string {
		const obj: PackObj = {
			data: cipherPack.data.toBase64(),
			nonce: cipherPack.nonce.toBase64(),
		}; // 1. cipherPack to obj
		const json = JSON.stringify(obj); // 2. obj to json
		const buffer = Buffer.from(json, "utf-8"); // 3. json to buffer
		const base64 = buffer.toString("base64"); // 4. buffer to base64
		return CipherPack.wrap(base64); // 5. wrap base64
	}

	static unpack(pack: string): CipherPack {
		if (!CipherPack.isCipherPackString(pack)) {
			throw new Error("Invalid pack format");
		}
		const base64 = CipherPack.unwrap(pack); // 5. unwrap base64
		const buffer = Buffer.from(base64, "base64"); // 4. base64 to buffer
		const json = buffer.toString("utf-8"); // 3. buffer to json
		const obj = JSON.parse(json); // 2. json to obj
		if (!CipherPack.isCipherPackObj(obj)) {
			throw new Error("Invalid pack format");
		}
		return new CipherPack(
			// 1. obj to cipherPack
			Buffer.from(obj.data, "base64"),
			Buffer.from(obj.nonce, "base64"),
		);
	}

	private static wrap(str: string): string {
		return `${ENCRYPTION_PREFIX}${str}${ENCRYPTION_SUFFIX}`;
	}

	private static unwrap(str: string): string {
		return str.slice(ENCRYPTION_PREFIX.length, -ENCRYPTION_SUFFIX.length);
	}

	private static isCipherPackString(value: unknown): value is string {
		return (
			typeof value === "string" &&
			value.startsWith(ENCRYPTION_PREFIX) &&
			value.endsWith(ENCRYPTION_SUFFIX)
		);
	}

	private static isCipherPackObj(value: unknown): value is PackObj {
		return (
			typeof value === "object" &&
			value !== null &&
			"data" in value &&
			"nonce" in value &&
			typeof (value as PackObj).data === "string" &&
			typeof (value as PackObj).nonce === "string"
		);
	}
}
