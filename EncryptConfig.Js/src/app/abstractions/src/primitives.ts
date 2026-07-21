export type Bytes = Uint8Array<ArrayBufferLike>;

export type EncryptedData = Bytes;

export type PlainData = Bytes;

export type PlainKey = PlainData;

export type EncryptedKey = EncryptedData;

export type PublicKey = PlainData;

export type PrivateKey = PlainData;

export type Nonce = PlainData;
