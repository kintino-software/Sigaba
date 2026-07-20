import type { PlainData, PrivateKey, PublicKey, EncryptedData, Nonce, Key } from "./primitives";

export interface IAsymmetricEncryption {
    generateKeys(): [publicKey: PublicKey, privateKey: PrivateKey];
    
    encrypt(plainData: PlainData, publicKey: PublicKey): Uint8Array;
    
    decrypt(encryptedData: EncryptedData, privateKey: PrivateKey): Uint8Array;
}

export interface ISymmetricEncryption {
    encrypt(plainData: PlainData, key: Key, nonce: Nonce): EncryptedData;
    decrypt(encryptedData: EncryptedData, key: Key, nonce: Nonce): PlainData;
}

export interface IKeyGenerator {
    generateKey(): Key;
}

export interface INonceGenerator {
    newNonce(): Nonce;
}

