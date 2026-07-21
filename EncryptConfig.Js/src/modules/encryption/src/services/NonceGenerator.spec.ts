import { NonceGenerator } from "./NonceGenerator";

it("should generate a nonce of the specified length", () => {
	const nonce = new NonceGenerator().newNonce();
	expect(nonce).toHaveLength(12); // 12 bytes = 96 bits
});
