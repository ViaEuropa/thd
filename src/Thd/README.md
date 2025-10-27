# Two Headed Dog - thd

## Running thd

### Get Help
```shell
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
