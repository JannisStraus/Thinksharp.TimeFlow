# Thinksharp.TimeFlow

[![NuGet](https://img.shields.io/nuget/v/Thinksharp.TimeFlow.svg)](https://www.nuget.org/packages/Thinksharp.TimeFlow/) 
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=MSBFDUU5UUQZL)

## Introduction

TimeFlow is a lightweight extendable library for working with time series with support for time zones and daylight saving. The core of the library is the **TimeSeries** type, which holds a list of time point / value pairs. Time points are represented as **DateTimeOffset**s. The time span between two time points (called frequency) is always identical for the whole time series. Values are represented as **nullable decimal**s.

The first and the last time points of a TimeSeries are exposed by the **Start** and **End** properties. The interval between time points is exposed as **Frequency** property.

TimeSeries objects are **immutable**. There are some methods for transforming TimeSeries, like **ReSample**, **Slice** or **Apply**. Each of these methods return a new immutable object.

A collection of named time series with the same frequency can be combined to one **TimeFrame**.

See [jupiter notebook](Notebooks/timeseries.ipynb) for examples (reload if visualization fails for the first time).

## Installation

### Usage in .Net Projects

ThinkSharp.TimeFlow can be installed via [Nuget](https://www.nuget.org/packages/Thinksharp.TimeFlow)

    Install-Package Thinksharp.TimeFlow

### Usage in .Net Interactive (Jupiter Notebooks)

Reference Nuget **Package Thinksharp.TimeFlow** and **Thinksharp.TimeFlow.Interactive** as well as **XPlot.Plotly** if you want to use plotting abilities.

    #r "nuget: Thinksharp.TimeFlow"
    #r "nuget: Thinksharp.TimeFlow.Interactive"
    #r "nuget: XPlot.Plotly"
    #r "nuget: XPlot.Plotly.Interactive"

## License

ThinkSharp.TimeFlow is released under [The MIT license (MIT)](LICENSE)

## Versioning

We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/JanDotNet/ThinkSharp.TimeFlow/tags). 
    
   
## Donation

If you like ThinkSharp.TimeFlow and use it in your project(s), feel free to give me a cup of coffee :) 