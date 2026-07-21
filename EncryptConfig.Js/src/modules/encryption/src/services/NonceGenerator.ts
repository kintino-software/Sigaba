import type { INonceGenerator, Nonce } from "@abstractions";
import { Rng } from "./Rng";

const nonceSizeInBytes = 12; // 96 bits

export class NonceGenerator implements INonceGenerator {
	newNonce(): Nonce {
		return Rng.createRandomBytes(nonceSizeInBytes); // 96 bits
	}
}
