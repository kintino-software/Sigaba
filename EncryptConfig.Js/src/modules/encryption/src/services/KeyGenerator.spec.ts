import { KeyGenerator } from "./KeyGenerator";

it("should generate a key of the specified length", () => {
	const keyLength = 32;
	const key = new KeyGenerator().generateKey();
	expect(key).toHaveLength(keyLength);
});
