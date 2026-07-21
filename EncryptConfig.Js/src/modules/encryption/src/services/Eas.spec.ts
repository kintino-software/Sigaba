import crypto from "node:crypto";
import { Eas } from "./Eas";

it("should encrypt and decrypt data using EAS", () => {
	const plainData = new Uint8Array([1, 2, 3, 4, 5]);
	const key = crypto.randomBytes(32);
	const nonce = crypto.randomBytes(12);
	const encryptedData = Eas.encrypt({ plainData, key, nonce });
	const decryptedData = Eas.decrypt({ encryptedData, key, nonce });
	expect(decryptedData).toEqual(plainData);
});
