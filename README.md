# X2ProjectGenerator

This script will make sure that every folder and file in project folder is included in the project and sorted in correct order.
Already existing entries won't be touched - you can run this script as many as you like.

## Download

This tool is built in C# using .Net Framework 4.7.2. Your options are:

* Clone the project and build it yourself
* Click on the releases (right sidebar on repo home on github web non-mobile) - the latest release has a built `.exe` attached

## Usage

* Create an empty or default mod project
* Close ModBuddy
* Copy the files as you want them to be in the project
* Run this exe with a single argument which points to the directory where `.x2proj` file is located:

```
X2ProjectGenerator.exe "C:\Users\xyman\Documents\Firaxis ModBuddy\XCOM - War of the Chosen\MyAwesomeMod\MyAwesomeMod"
``` 

*Note that the quotes (`"`) are important - otherwise script will break (if there are spaces in folder names)*

**TIP**: to open a command line prompt in the folder where you downloaded the `X2ProjectGenerator.exe`
simply type `cmd` in the file explorer's address bar and hit enter

## Additional flags

### `--verify-only`

Don't write to the project file. Instead throw an exception if entries would be added to the project file.

### `--exclude-contents`

Folder names starting with `Content` will be ignored (commonly `Content` and `ContentForCook`).

## Examples

### Automatically verify project as part of build process

If using a Powershell-based build script (like [X2ModBuildCommon](https://github.com/X2CommunityCore/X2ModBuildCommon)), you
can add this to your `build.ps1` (requires X2ProjectGenerator.exe in `PATH` environment variable):

```ps1
if ($null -ne (Get-Command "X2ProjectGenerator.exe" -ErrorAction SilentlyContinue)) {
    Write-Host "Verifying project file..."
    &"X2ProjectGenerator.exe" "$srcDirectory\YOUR_MOD_NAME_HERE" "--exclude-contents" "--verify-only"
    if ($LASTEXITCODE -ne 0) {
        ThrowFailure "Errors in project file."
    }
}
else {
    Write-Host "Skipping verification of project file."
}
```

### Automatically verify project in GitHub Actions for Pull Requests

File name: `.github/workflows/check_project_file.yaml`

```yaml
name: check-project-file

on:
  push:
    branches:
      - master
  pull_request:
    branches:
      - master

jobs:
  check:
    runs-on: windows-latest
    steps:
    - name: Checkout
      uses: actions/checkout@v2
    - name: Install X2ProjectGenerator
      shell: pwsh
      run: |
        Invoke-WebRequest -UseBasicParsing -Uri https://github.com/Xymanek/X2ProjectGenerator/releases/download/v1.1/X2ProjectGenerator.exe -OutFile X2ProjectGenerator.exe
    - name: Check project file
      shell: pwsh
      run: |
        .\X2ProjectGenerator.exe "YOUR_MOD_NAME_HERE\" --exclude-contents --verify-only
```
