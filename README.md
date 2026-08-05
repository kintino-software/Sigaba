# Sigaba

Sigaba is a command-line tool that encrypts/decrypts selected fields in files that contain sensitive information, such as passwords and connection strings. It is mainly intended to be used on configuration files.

Once encrypted, the file will still be human-readable, but the sensitive fields will be replaced with encrypted values. 

So, before encryption you would have:

```json
{
    "apikey_secret": "plain-api-key",
    "connectionString_secret": "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;",
    "public_field": "public value"
    
}
```

and after the encryption:

```json
{
    "apikey_secret": "ENC(XAKfHKC...)",
    "connectionString_secret": "ENC(bU4hcFt...)",
    "public_field": "public value"
}
```

The tool uses asymmetric encryption, which means that you can share a public key with your team members, while keeping the private key secure.

**IMPORTANT:** For now, this tool only works on ```json``` files.

## Features

1. Encrypts any kind of value: strings, numbers, booleans, arrays and even nulls (but it does not work with nested objects - see below).
2. Preserves comments and formatting after decryption and encryption.
3. Allows users with the public key to update the secret values and re-encrypt them, but not decrypt them.
4. Scans file deep into the folder structure, so you will have a single configuration file for multiple projects or folders.

## Installation

To install Sigaba, you can use the following command:

```bash
dotnet tool install --global Sigaba
```

## Usage

### Initialization

Initialize Sigaba in your project or solution directory, run on terminal:

```bash
sigaba init
```

It will create 3 files in the working directory: ```sigaba.json```, ```public.key``` and ```private.key```:

- **sigaba.json**:
  - contains the tool configuration (see more below).
  - required to perform encryption/decryption, so the tool won't work without it.
  - sets the starting point where the tool will search for files to encrypt/decrypt.
- **public.key**:
  - the public key needed to encrypt or re-encrypt the sensitive content.
  - has to be in the same folder as ```sigaba.json``` file.
  - you cannot decrypt content with this file, only encrypt or re-encrypt.
  - generally is a good idea to NOT commit this file and share it only with people you want to.
- **private.key**:
  - the private key needed to decrypt all files. In another words, the file that will transform encrypted content to plain content.
  - has to be in the same folder as ```sigaba.json``` file.
  - **DO NOT commit** this file, as it can expose all your sensitive content.
  - It's generally good practice to allow only an automated system to access the private key through a secure mechanism. For example, in Azure Devops, you can upload the private key as a secure file and use it during a deployment pipeline.

### Encryption

To encrypt your files, use the following command:

  ```bash
  sigaba encrypt
  ```

The tool will look for the files and fields according to the configuration file and encrypt/re-encrypt them.

### Decryption

To decrypt the encrypted files, use:

```bash
sigaba decrypt
```

The tool will look for the files and fields according to the configuration file and decrypt them.

## Configuration

To configure the tool, open and edit ```sigaba.json``` file.

The file will contain the following fields:

| Name       | Type         |                                                                                                                                       |
| ---------- | ------------ | ------------------------------------------------------------------------------------------------------------------------------------- |
| version    | number       | **Do not changet it!** Keeps track of the configuration file version.                                                                 |
| fieldRegex | string       | The regex pattern used to select the fields to be encrypted/decrypted. The pattern is matched against the field name, not its value.  |
| include    | string array | A glob pattern to filter which file will be encrypted/decrypted.                                                                      |
| exclude    | string array | A glob pattern to filter which file will NOT be encrypted/decrypted.                                                                  |

### Example

With the following configuration:

```json
{
    "version": 1,
    
    "fieldRegex": "^.*_secret$",
    
    "include": [ 
      "**/*.secrets.json" 
    ],
    
    "exclude":[
      "**node_modules/**",
      "**/bin/**",
      "**/obj/**"
    ]
}
```

the tool will work according to these rules:

- the tool will process all files that ends with *.secrets.json* (like *appsettings.secrets.json*, *myconfig.secrets.json*, etc.).
- the tool will skip any file inside a *node_modules* or *bin* or *obj* folder.
- on each of the filtered files, the tool will look for any field name that ends with *_secret* and encrypt/decrypt it's value.

## Limitations

### Single configuration per project or solution

Currently, the tool cannot handle multiple (nested) ```sigaba.json``` files in the same folder hierarchy. It means that:

- or you have only one configuration in the solution root folder,
- or multiple configurations, each one in a different project folder.

### JSON files

1. Although the tool can reach any field in the document hiearchy, it cannot encrypt entire json objects. So if you have:

    ```json
    {
        "parent_secret": { 
            "child_secret": "sensitive data" 
        }
    }
    ```

    will become:

    ```json
    {
        "parent_secret": { 
            "child_secret": "ENC(AAAA...)" 
        }
    }
    ```

    and not:

    ```json
    {
        "parent_secret": "ENC(AAAA...)"
    }
    ```

    That's to avoid confusion on which level of the document would be encrypted first and the hassle to handle nested encrypted values.
