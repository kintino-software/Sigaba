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

Currently, this tool only works on .json files.

## Installation

To install Sigaba, you can use the following command:

```bash
dotnet tool install --global Sigaba
```

## Usage

1. Initialize Sigaba in your project or solution directory:

    ```bash
    sigaba init
    ```

    It will create a ```sigaba.json```, a ```public.key``` and a ```private.key``` file in the current directory.

    **IMPORTANT: Do not commit ```private.key``` and ```public.key``` files to your version control system! And mind with whom you share them!**

    The public key can be shared with users that can update the secret values. Although those users can change the value and re-encrypt them, they cannot see the plain values.

    The private key should be shared only with parties that must decrypt and have access to plain values. It's generally good practice to allow only an automated system to access the private key through a secure mechanism. For example, in Azure Devops, you can upload the private key as a secure file and use it during a deployment pipeline.

    The ```sigaba.json``` marks the directory root from where the tool will search for files to encrypt/decript (see configuration below).

2. To encrypt your files, use the following command:

    ```bash
    sigaba encrypt
    ```

    The tool will look for the files and fields according to the configuration file and encrypt them.

3. To decrypt the encrypted files, use:

    ```bash
    sigaba decrypt
    ```

## Configuration

To configure the tool, open and edit _sigaba.json_ file.
You will see a content like:

```json
{
    "version":1,
    "fieldRegex":"^.*_secret$",
    "include":["**/*.secrets.json"],
    "exclude":["**node_modules/**","**/bin/**","**/obj/**"]
}
```

| Name       | Type         |                                                                                                                                       |
| ---------- | ------------ | ------------------------------------------------------------------------------------------------------------------------------------- |
| version    | number       | **Do not changet it!** Keeps track of the configuration file version.                                                                 |
| fieldRegex | string       | The regex pattern used to select the fields to be encrypted/decrypted. The pattern is matched against the field name, not its value.  |
| include    | string array | A glob pattern to filter which file will be encrypted/decrypted.                                                                      |
| exclude    | string array | A glob pattern to filter which file will NOT be encrypted/decrypted.                                                                  |

In the example above, the tool will process all files that ends with _\.secrets.json_ (like appsettings.secrets.json), but will not process files inside a _node_modules_ or _bin_ or _obj_ folder.
Then, on each of the filtered files, the tool will look for any field name that ends with _\_secret_ and encrypt it's value.

## Features

1. Encrypts any kind of value: strings, numbers, booleans, arrays and even nulls (but it does not work with nested objects - see below).
2. Preserves comments and formatting after decryption and encryption.
3. Allows users with the public key to update the secret values and re-encrypt them.
4. Scans file deep into the folder structure, so you will have a single configuration file for multiple projects.

## Limitations

### Single configuration per project or solution

Currently, the tool cannot handle multiple (nested) ```sigaba.json``` files in the same folder hierarchy. It means that, or you have only one configuration in the solution root folder, or multiple configurations, each one in a different project folders.

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

    and never

    ```json
    {
        "parent_secret": "ENC(AAAA...)"
    }
    ```

    That's to avoid confusion on which level of the document would be encrypted first and the hassle to handle nested encrypted values.
