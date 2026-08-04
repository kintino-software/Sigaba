# Sigaba

## Description
Sigaba is a command-line tool that encrypts/decrypts selected fields in files that contain sensitive information, such as passwords and connection strings. It is mainly intended to be used on configuration files.

Once encrypted, the file will still be human-readable, but the sensitive fields will be replaced with encrypted values. 

So, before encryption you would have:
```json
{
    "password_secret": "my plain password here!"
}
```
and after the encryption:
```json
{
    "password_secret": "ENC(bU4hcFtAU1GAIUUHBsu7GAAAAAAAAAAAAAAAAGbuUQ5dVXAKfHKCLXFevZcwWTATBgcqhkjOPQIBBggqhkjOPQMBBwNCAAT3\u002B7HFv4HQAf7hYTovk61ScJz3uUOkgAg7hi4vi34okP62bjJcZHCCTFtmR2hwzhyH1aynaUB2C/W0GdegqYRV)"
}
```

The tool uses asymmetric encryption, which means that you can share the public key with your team members, while keeping the private key secure.

Currently, this tool only works on .json files.
## Installation

To install Sigaba, you can use the following command:
```bash
dotnet tool install --global Sigaba
```

## Usage

1. First, initialize Sigaba in your project or solution directory:
```bash
cd /path/to/your/project
sigaba init
```
It will create a _sigaba.json_, a _public.key_ and a _private.key_ file in the current directory.

Move the _private.key_ file to a secure location and do not commit it to your version control system.

2. To encrypt your configuration files, use the following command:
```bash
sigaba encrypt
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
| version    | number       | Dot not changet it. Keeps track of the configuration file version.                                                                    |
| fieldRegex | string       | The regex pattern used to select the fields to be encrypted/decrypted. The pattern is matched against the field name, not it's value. |
| include    | string array | A glob pattern to filter which file will be encrypted/decrypted.                                                                      |
| exclude    | string array | A glob pattern to filter which file will NOT be encrypted/decrypted.                                                                  |

In the example above, the tool will process all files that ends with _\.secrets.json_ (like appsettings.secrets.json), but will not process files inside a _node_modules_ or _bin_ or _obj_ folder.
Then, on each of the files, the tool will look for any field name that ends with _\_secret_ and encrypt it's value.

