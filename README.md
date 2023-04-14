# FanControl.AcerPO3630

Plugin for [FanControl](https://github.com/Rem0o/FanControl.Releases) providing access to Acer PO3-630 sensors using Acer PredatorSense Named Pipe methods.

After becoming incresingly frustrated with Acer seemingly not allowing 3rd party access to the fan controls, I used dotPeek to open up PredatorSense.exe and reverse engineered the speed controls from there. The plugin currently relies on the Predator Service installed alongside the Predator Sense app, I hope to remove this dependancy sometime down the road.

## Installation

1. Install the Acer Predator Sense application. This plugin was tested using PredatorSense 3.0.3044.0. This step is required to to install the PredatorSense Service on which this plugin relies.
2. Download the latest binaries from [Releases](https://github.com/millerm1993/FanControl.Acer_PO3630/releases)
3. Copy the FanControl.AcerPO3630.dll into FanControl's "Plugins" folder.

## Compatibility 

I tested the plugin with my Acer PO3-630 PC. Looking through the code in PredatorSense.exe it may work on other machines (PO5 & PO9) but these are untested.

This plugin was tested using PredatorSense 3.0.3044.0. At the time of writing, PredatorSense 4 was released but was incompatible with my machine/os, therefore I have no idea if it works with this plugin.
