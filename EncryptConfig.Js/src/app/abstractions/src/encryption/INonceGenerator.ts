import type { Nonce } from "../primitives";

export interface INonceGenerator {
	newNonce(): Nonce;
}
