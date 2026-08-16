# Sigaba

Sigaba is a command-line tool that encrypts/decrypts determined fields in files that contain sensitive information, such as passwords and connection strings. It is mainly intended to be used on configuration files.

Once encrypted, the file will still be human-readable, but the sensitive fields will be replaced with encrypted values. 

So, before encryption you would have a file with the following content:

```json
{
    "private_field_secret": "some secret data",
    "public_field": "public value"
}
```

and after the encryption:

```json
{
    "private_field_secret": "ENC(XAKfHKC...)",
    "public_field": "public value"
}
```

**IMPORTANT:** For now, this tool only works on ```json``` files.

## TLDR;

### Initialize:
Run:

```bash
dotnet tool install --global Sigaba
sigaba init
> Enter a password to encrypt the private key: ********
> Enter the password again to confirm: ********
```

Edit ```sigaba.json``` file to select which files and fields will be encrypted/decrypted.

Move the private key (```<user profile folder>\.sigaba\private.key```) to a secure location.

### Encrypt:

```bash
sigaba encrypt
```

### Decrypt:
Put the private key file (```private.key```) in:
- the current working directory, or 
- in a folder defined in the environment variable ```SIGABA_PRIVATE_KEY_DIR```, or
- in the ```.sigaba``` folder in the user profile, in a subfolder matching the ```projectId``` in the ```sigaba.json``` file.

Then run: 

```bash
sigaba decrypt -p <password>
```

## Features

1. Encrypts any kind of values (strings, numbers, booleans, arrays and even nulls) except entire objects (see more below).
2. Preserves comments and formatting after decryption and encryption.
3. Allows users to update the secret values and re-encrypt them, but not decrypt them.
4. Scans file deep into the folder structure, so you will have a single configuration file for multiple projects or subfolders.

## Installation

Install as a dotnet tool:

```bash
dotnet tool install --global Sigaba
```
or with tool manifests:
```bash
dotnet new tool-manifest # if you don't have a manifest file yet
dotnet tool install Sigaba
```

## Usage

### Initialization

Initialize Sigaba in your project or solution directory, running on terminal:

```bash
sigaba init
```
or
```bash
sigaba init --non-interactive -password <password>
```

In this step you will set the password that will be necessary to decrypt the files.

Also, the tool will create 2 files: ```sigaba.json``` and ```private.key```:

- **sigaba.json**:
  - placed in the current working directory, i.e. the folder where you run the command.
  - contains the tool configuration (see more below).
  - required to perform encryption/decryption, so the tool won't work without it.
  - sets the topmost folder where the tool will search for files to encrypt/decrypt.
  - this file should be kept in the source control, so that all team members will have the same configuration. 
- **private.key**:
  - placed in the user profile folder for safety, i.e. ```%USERPROFILE%\.sigaba\private.key``` on Windows or ```~/.sigaba/private.key``` on Linux and MacOS.
  - it is needed to decrypt all files. In another words, the file that will transform encrypted content to plain content.
  - The private key content is encrypted with the provided password, so it is useless without it.
  - once created, move it to a secure location and delete it from the original location.
  - It's generally good practice to allow only an automated system to access the private key through a secure mechanism. For example, in Azure Devops, you can upload the private key as a secure file and use it during a deployment pipeline.
  - **Do not check it into source control**!

### Encryption

To encrypt your files, use the following command:

  ```bash
  sigaba encrypt
  ```

The tool will look for the files and fields according to the configuration file and encrypt/re-encrypt them.

If any field has changed and it's not encrypted yet, the tool will encrypt it. If any field is already encrypted, the tool will leave it as is.

### Decryption


To decrypt the encrypted files, use:

```bash
sigaba decrypt -p <password>
```

The tool will look for the files and fields according to the configuration file and decrypt them.

#### Prerequisites for decryption

For decryption, the tool will need:
1. the password you defined during initialization. You pass it to the command line as shown above. This password is used to decrypt the private key file.
1. the private key file ```private.key``` that was created during initialization.

The tool will search for the file ```private.key``` in the following locations, in order:
1. A directory path defined in the environment variable ```SIGABA_PRIVATE_KEY_DIR```
2. The current working directory, i.e. the folder where you run the command.
3. The ```.sigaba``` folder in the user profile, in a subfolder matching the ```projectId``` in the ```sigaba.json``` file. 

Keep in mind those locations above when you are planning to use the tool in a CI/CD pipeline, so that the private key is available for decryption.

## Configuration

To configure the tool, open and edit ```sigaba.json``` file and edit any field on the ```configuration``` section.

**Do not modify the fields outside the ```configuration``` section** or the tool will not work.

The section will contain the following fields:

| Name       | Type         |   |
| ---------- | ------------ | - |
| fieldRegex | string       | The regex pattern used to select the fields to be encrypted/decrypted.<br/> The pattern is matched against the field name, not its value. |
| include    | string array | A glob pattern to filter which file will be encrypted/decrypted.|
| exclude    | string array | A glob pattern to filter which file will NOT be encrypted/decrypted.|

### Example

With the following configuration:

```json
{
  "configuration": {
    "fieldRegex": "^.*_secret$",
    "include": [ "**/*.secrets.json" ],
    "exclude": [ "**node_modules/**", "**/bin/**", "**/obj/**" ]
  },

  "meta": {
    "version": 1,
    "projectId": "b5f2a6d1b4124bc38012c2a70c575646",
    "publicKey": "ATBZMBMGByqGSM49Ag==",
  }
  
}
```

the tool will work according to these rules:

- the tool will process all files that ends with *.secrets.json* (like *appsettings.secrets.json*, *myconfig.secrets.json*, etc.).
- the tool will skip any file inside a *node_modules* or *bin* or *obj* folder.
- on each of the filtered files, the tool will look for any field name that ends with *_secret* and encrypt/decrypt it's value.

## Limitations

### Single configuration per project or solution

Currently, the tool cannot handle nested ```sigaba.json``` files in the same folder hierarchy. It means that:

- or you have only one configuration in the project root folder,
- or multiple configurations, each one in a different project subfolder.

### JSON files

Although the tool can reach any field in the document hiearchy, it cannot encrypt entire json objects. So if you have:

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

  That's to avoid confusion on which level of the document would be encrypted first and the hassle to handle nested encrypted values.
