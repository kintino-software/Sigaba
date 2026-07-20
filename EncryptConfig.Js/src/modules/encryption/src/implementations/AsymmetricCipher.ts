import { type EncryptedData, type IAsymmetricEncryption, type PlainData, type PrivateKey, type PublicKey } from "@abstractions";


export class AsymmetricCipher implements IAsymmetricEncryption{
    generateKeys(): [publicKey: PublicKey, privateKey: PrivateKey] {
        throw new Error("Method not implemented.");
    }
    
    encrypt(plainData: PlainData, publicKey: PublicKey): Uint8Array {
        throw new Error("Method not implemented.");
    }
    
    decrypt(encryptedData: EncryptedData, privateKey: PrivateKey): Uint8Array {
        throw new Error("Method not implemented.");
    }

}
