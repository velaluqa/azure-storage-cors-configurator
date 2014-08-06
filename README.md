# Azure Storage CORS Configurator

Rough C# tool to update the Azure Storage CORS rules, developed with
Mono.

Thanks to https://github.com/Widen/fine-uploader-server.

## Notice: Using it with FineUploader and Safari

Safari asks explicitly whether "origin" is allowed. So add "origin" to the list of `ALLOWED_CORS_HEADERS` when using FineUploader.
