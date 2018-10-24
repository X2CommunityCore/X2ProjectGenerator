# X2ProjectGenerator

### Usage

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