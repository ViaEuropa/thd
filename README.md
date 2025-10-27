# Two Headed Dog - thd
[![Build](https://github.com/ViaEuropa/thd/workflows/build/badge.svg)](https://github.com/ViaEuropa/thd/actions?query=workflow:"build")
[![GitHub Release](https://img.shields.io/github/release/ViaEuropa/thd?include_prereleases=&sort=semver&color=blue)](https://github.com/ViaEuropa/thd/releases/)
[![NuGet version](https://img.shields.io/nuget/v/thd.svg?style=flat&label=NuGet)](https://www.nuget.org/packages/thd)
[![License](https://img.shields.io/badge/License-MIT-blue)](#license)


## Getting started

### Installation
```shell
dotnet tool install --global thd

thd -h
```

### Basic Compare
Create a CSV file named `requests.csv`
- The last columns are considered as paths to be appended to the base URL.
- All other columns are considered as variables to be used in the base URL template and will be referenced as `column_1`, `column_2`, etc.

```csv
id,subdomain,path
1,foo,/endpoint1
2,bar,/version
```

```shell
 export THD_EXPECTED_AUTHORIZATION_HEADER="Bearer TokenForTheExpectedService"
 export THD_REQUEST_ACTUAL_AUTHORIZATION_HEADER="Bearer TokenForTheActualService"
 
thd compare requests.csv \
  --expected-base-url "https://{{ column_2 }}.api.localhost:9094" \
  --actual-base-url "https://{{ column_2 }}.api.localhost:9094/v2"
```