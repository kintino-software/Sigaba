## json limitations:
1. In this version cannot encrypt entire json objects
    ```json
    {
        "parent_secret": { "child_secret": "sensitive data" }
    }
    ```
    will become:
    ```json
    {
        "parent_secret": { "child_secret": "ENC(AAAA...)" }
    }
    ```
    and never
    ```json
    {
        "parent_secret": "ENC(AAAA...)"
    }
    ```
    this is to prevent encrypted values nested inside encrypted objects that would cause a mess to decrypt.
