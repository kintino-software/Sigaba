import type { Key } from "../primitives";

export interface IKeyGenerator {
	generateKey(): Key;
}
