import type { IKeyGenerator } from "@abstractions";
import { Rng } from "./Rng";

const keySizeInBytes = 32; // 256 bits

export class KeyGenerator implements IKeyGenerator {
	generateKey() {
		return Rng.createRandomBytes(keySizeInBytes);
	}
}
