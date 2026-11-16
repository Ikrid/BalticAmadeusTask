## Requirements

.NET 10 SDK (tested with version 10.0.201).
https://dotnet.microsoft.com/download

Run:

```sh
dotnet run
```


## Implemented parts

- Task 1
- Task 2
- Bonus 1
- Bonus 2

## Date handling

Accepted formats are yyyy-MM-dd and dd/MM/yyyy.
Dates are validated using DateTime.TryParseExact with
CultureInfo.InvariantCulture.

Invalid input dates are requested again.
The end date cannot be earlier than the start date.
Both range boundaries are included.

## AI usage

I used ChatGPT to help handle date formats and validation,
suggest code snippets, debug errors, and verify calculation results.