# Design Automation signer

[![Design-Automation](https://img.shields.io/badge/Design%20Automation-v3-green.svg)](http://developer.autodesk.com/)
![.NET Core](https://img.shields.io/badge/.NET%20Core-5.0-blue.svg)

[![License](http://img.shields.io/:license-MIT-blue.svg)](http://opensource.org/licenses/MIT)

#Description

This is a utility to sign the parts of the workitem request message with the right algorithm and parameters.

# Setup

## Prerequisites

1. [.NET runtime](https://dotnet.microsoft.com/download/dotnet)

## Build

At the folder on the `.csproj`, run the following via command line:

```bash
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true
```

## Usage

- Generate a public/private key pair using Das.WorkitemSigner.

```bash
dotnet run generate mykey.json
```

- Export the public key into a json file using Das.WorkItemSigner.

```bash
dotnet run export mykey.json mypublickey.json
```

- Upload public key using PATCH forgeapps/me API (see https://forge.autodesk.com/en/docs/design-automation/v3/reference/http/forgeapps-id-PATCH/). 

```bash  
  curl forgeapps/me -x PATCH `{'publicKey' : <contents of mypublickey.json>}`
```

- Generate digital signature for the `activityId` that it wants to call using Das.WorkitemSigner

```bash
dotnet run sign mykey.json <ForgeAppNickNameOrId>.PlotToPdf+<Alias>
```
## Test

- create `setenv.bat`

```
set CLIENT_ID=<<YOUR CLIENT ID FROM DEVELOPER PORTAL>>
set CLIENT_SECRET=<<YOUR CLIENT SECRET>>
```
- Create a basic activity, lets name our activity `HelloWorld` with alias `prod`.  Note I'm using `adesk` as forgeapp nickname.

  - ```bash
    {
    	"commandLine": "$(engine.path)\\accoreconsole.exe /s \"$(settings[script].path)\"",
    	"engine": "Autodesk.AutoCAD+24",
    	"settings": {
    		"script": "(print \"Hello World!\")\n"
    	},
    	"description": "Send String To Execute Test",
    	"id": "{{ _.activityId }}"
    }
    ```

  - ```bash
    {
    	"version": 1,
    	"id": "{{ _.alias }}"
    }
    ```

- Or you can run `createActivity.ps1`
```bash
  dotnet run generate mykey.json
  dotnet run export mykey.json mypublickey.json
  .\patchPublicKey.ps1
  dotnet run sign mykey.json adesk.HelloWorld+prod
  .\workitem.ps1
```

## Demonstration

[Video demo](https://github.com/Autodesk-Forge/Das.WorkItemSigner/blob/main/demo/demo.mp4)

## License

This sample is licensed under the terms of the **MIT LICENSE**. Please see the [LICENSE](https://github.com/Autodesk-Forge/Das.WorkItemSigner/blob/main/LICENSE) file for full details.
