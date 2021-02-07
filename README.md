# X2ProjectGenerator

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

This script will make sure that every folder and file in project folder is included in the project and sorted in correct order.
Already existing entries won't be touched - you can run this script as many as you like.