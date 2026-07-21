import type { EncryptedKey, PrivateKey, PublicKey } from "../primitives";
import type { IFieldFilter } from "./IFieldFilter";
import type { IFileFilter } from "./IFileFilter";

export type IContext = {
	privateKey: PrivateKey;
	publicKey: PublicKey;
	fieldFilter: IFieldFilter;
	fileFilter: IFileFilter;
	key: EncryptedKey;
};
