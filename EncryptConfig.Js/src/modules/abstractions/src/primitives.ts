export type Bytes = Uint8Array;

export type EncryptedData = { bytes: Bytes };

export type PlainData = { bytes: Bytes };

export type Key = { data: PlainData };

export type PublicKey = { data: PlainData };

export type PrivateKey = { data: PlainData };

export type Nonce = { data: PlainData };