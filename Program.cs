
using System.Globalization;
using System.Text.Json;
using BalticAmadeusTask.Models;
using BalticAmadeusTask;


public class Program
{
public static void Main()
{
    string inputJson = File.ReadAllText("shipments.json");

    using JsonDocument document = JsonDocument.Parse(inputJson);

    List<Result> results = new();
    
    List<Shipment> validShipments = new();
    
    HashSet<string> seenIds = new(StringComparer.Ordinal);

    Directory.CreateDirectory("output");

    string errorsPath = Path.Combine("output", "errors.csv");

    using (StreamWriter errorWriter = new StreamWriter(errorsPath))
    {
        errorWriter.WriteLine("Adam_Nekrašas");
        errorWriter.WriteLine("ShipmentID,ErrorCode");

        foreach (JsonElement record in document.RootElement.EnumerateArray())
        {
            string id = "";

            if (record.ValueKind == JsonValueKind.Object
                && record.TryGetProperty("id", out JsonElement idElement)
                && idElement.ValueKind == JsonValueKind.String)
            {
                id = idElement.GetString() ?? "";
            }

            string errorCode;

          
            if (record.ValueKind == JsonValueKind.Object
                && record.TryGetProperty("id", out JsonElement existingId)
                && existingId.ValueKind == JsonValueKind.String
                && !seenIds.Add(id))
            {
                errorCode = "DUPLICATE_ID";
            }
            else
            {
                errorCode = GetErrorCode(record);
            }

            if (errorCode != "")
            {
                errorWriter.WriteLine(
                    EscapeCsv(id) + "," + errorCode
                );

                continue;
            }

            Shipment shipment = record.Deserialize<Shipment>()!;
            validShipments.Add(shipment);
            results.Add(CalculateShipment(shipment));
        }
    }

    string outputPath = Path.Combine("output", "task1.csv");

    using (StreamWriter writer = new StreamWriter(outputPath))
    {
        writer.WriteLine("Adam_Nekrašas");
        writer.WriteLine(
            "ShipmentID,NetFuelPrice,LogisticsCost,TotalTax,GrandTotal"
        );

        foreach (Result result in results)
        {
            writer.WriteLine(
                EscapeCsv(result.Id) + "," +
                result.NetFuelPrice.ToString("F2", CultureInfo.InvariantCulture) + "," +
                result.LogisticsCost.ToString("F2", CultureInfo.InvariantCulture) + "," +
                result.TotalTax.ToString("F2", CultureInfo.InvariantCulture) + "," +
                result.GrandTotal.ToString("F2", CultureInfo.InvariantCulture)
            );
        }
    }

    decimal VATrate;
    

    while (true)
    {
        Console.Write("Enter custom VAT rate for Private clients (%): ");

        string? input = Console.ReadLine();

        if (input == null)
        {
            Console.Error.WriteLine("Not enough input.");
            return;
        }

        bool isNumber = decimal.TryParse(
            input,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out VATrate
        );

        if (!isNumber)
        {
            Console.WriteLine("Please enter a number.");
            continue;
        }

        if (VATrate < 0m)
        {
            Console.WriteLine("Value cannot be negative.");
            continue;
        }

        break;
    }

    VATrate = VATrate / 100m;
    
    decimal GasolineExcise;

    while (true)
    {
        Console.Write("Enter custom Excise rate for Gasoline: ");

        string? input = Console.ReadLine();

        if (input == null)
        {
            Console.Error.WriteLine("Not enough input.");
            return;
        }

        bool isNumber = decimal.TryParse(
            input,
            NumberStyles.Float,
            CultureInfo.InvariantCulture,
            out GasolineExcise
        );
    
        if (!isNumber)
        {
            Console.WriteLine("Please enter a number.");
            continue;
        }

        if (GasolineExcise < 0m)
        {
            Console.WriteLine("Value cannot be negative.");
            continue;
        }

        break;
    }
    
    
    
    List<Result> task2Results = new();

    foreach (Shipment shipment in validShipments)
    {
        Result result = CalculateShipment(
            shipment,
            VATrate,
            GasolineExcise
        );

        task2Results.Add(result);
    }

    string task2Path = Path.Combine("output", "task2.csv");

    using (StreamWriter writer = new StreamWriter(task2Path))
    {
        writer.WriteLine("Adam_Nekrašas");
        writer.WriteLine(
            "ShipmentID,NetFuelPrice,LogisticsCost,TotalTax,GrandTotal"
        );

        foreach (Result result in task2Results)
        {
            writer.WriteLine(
                EscapeCsv(result.Id) + "," +
                result.NetFuelPrice.ToString("F2", CultureInfo.InvariantCulture) + "," +
                result.LogisticsCost.ToString("F2", CultureInfo.InvariantCulture) + "," +
                result.TotalTax.ToString("F2", CultureInfo.InvariantCulture) + "," +
                result.GrandTotal.ToString("F2", CultureInfo.InvariantCulture)
            );
        }
    }

    DateTime StartDate;

    while (true)
    {
        Console.Write("Choose date range, Start Date: ");

        string? input = Console.ReadLine();

        if (input == null)
        {
            Console.Error.WriteLine("Not enough input.");
            return;
        }

        bool isDate = DateTime.TryParseExact(
            input,
            new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out StartDate
        );
        

    if (!isDate)
        {
            Console.WriteLine("Please enter a date.");
            continue;
        }
        break;
    }

    DateTime EndDate;

    while (true)
    {
        Console.Write("End date: ");
        string? input = Console.ReadLine();

        if (input == null)
        {
            Console.Error.WriteLine("Not enough input.");
            return;
        }

        bool isDate = DateTime.TryParseExact(
            input,
            new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out EndDate
        );

        if (!isDate)
        {
            Console.WriteLine("Please enter a date");
            continue;
        }
        
        if (EndDate < StartDate)
        {
            Console.WriteLine("End date cannot be before Start date.");
            continue;
        }
        break;
    }

    int shipmentCount = 0;
    string mostExpensiveOne = "N/A";
    decimal highestGrandTotal = 0m;
    decimal totalVAT = 0m;

    for (int i = 0; i < validShipments.Count; i++)
    {
        Shipment shipment = validShipments[i];
        Result result = task2Results[i];

        DateTime shipmentDate = DateTime.ParseExact(
            shipment.Date,
            new[] { "yyyy-MM-dd", "dd/MM/yyyy" , "dd-MM-yyyy", "yyyy/MM/dd"},
            CultureInfo.InvariantCulture,
            DateTimeStyles.None
        );

        if (shipmentDate >= StartDate && shipmentDate <= EndDate)
        {
            shipmentCount++;
            totalVAT += result.VAT;

            if (shipmentCount == 1 || result.GrandTotal > highestGrandTotal)
            {
                highestGrandTotal = result.GrandTotal;
                mostExpensiveOne = result.Id;
            }
        }
    }

    
    string bonus1Path = Path.Combine("output", "bonus1.csv");

    using (StreamWriter writer = new StreamWriter(bonus1Path))
    {
        writer.WriteLine("Adam_Nekrašas");
        writer.WriteLine("ShipmentCount,MostExpensiveID,TotalVAT");

        writer.WriteLine(
            shipmentCount + "," +
            EscapeCsv(mostExpensiveOne) + "," +
            totalVAT.ToString("F2", CultureInfo.InvariantCulture)
        );
    }

    string paymentsJson = File.ReadAllText("payments.json");

using JsonDocument paymentsDocument = JsonDocument.Parse(paymentsJson);

Dictionary<string, DateTime> paymentDates = new(StringComparer.Ordinal);

HashSet<string> validIds = new(StringComparer.Ordinal);

foreach (Shipment shipment in validShipments)
{
    validIds.Add(shipment.Id);
}

foreach (JsonElement payment in paymentsDocument.RootElement.EnumerateArray())
{
    if (payment.ValueKind != JsonValueKind.Object)
    {
        continue;
    }

    if (!payment.TryGetProperty("id", out JsonElement idElement)
        || idElement.ValueKind != JsonValueKind.String)
    {
        continue;
    }

    string id = idElement.GetString()!;

    if (!validIds.Contains(id))
    {
        continue;
    }

    if (paymentDates.ContainsKey(id))
    {
        continue;
    }

    if (!payment.TryGetProperty("paidDate", out JsonElement dateElement)
        || dateElement.ValueKind != JsonValueKind.String)
    {
        continue;
    }

    bool isDate = DateTime.TryParseExact(
        dateElement.GetString(),
        new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
        CultureInfo.InvariantCulture,
        DateTimeStyles.None,
        out DateTime paidDate
    );

    if (!isDate)
    {
        continue;
    }

    paymentDates.Add(id, paidDate);
}

string bonus2Path = Path.Combine("output", "bonus2.csv");

using (StreamWriter writer = new StreamWriter(bonus2Path))
{
    writer.WriteLine("Adam_Nekrašas");
    writer.WriteLine("ShipmentID,DueDate,DaysLate,LateInterest");

    for (int i = 0; i < validShipments.Count; i++)
    {
        Shipment shipment = validShipments[i];
        Result result = results[i]; 

        DateTime shipmentDate = DateTime.ParseExact(
            shipment.Date,
            new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None
        );

        DateTime dueDate = shipmentDate.AddDays(
            PricingRules.PaymentTermDays
        );

        DateTime endDate = PricingRules.ReportDate;

        if (paymentDates.TryGetValue(shipment.Id, out DateTime paidDate))
        {
            endDate = paidDate;
        }

        int daysLate = 0;

        if (endDate > dueDate)
        {
            daysLate = (endDate - dueDate).Days;
        }

        decimal lateInterest = CalculateLateInterest(
            result.GrandTotal,
            dueDate,
            daysLate
        );

        writer.WriteLine(
            EscapeCsv(shipment.Id) + "," +
            dueDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "," +
            daysLate.ToString(CultureInfo.InvariantCulture) + "," +
            lateInterest.ToString("F2", CultureInfo.InvariantCulture)
        );
    }
}



}
    
    
private static string GetErrorCode(JsonElement record)
{
    if (record.ValueKind != JsonValueKind.Object)
    {
        return "MISSING_FIELD";
    }

    string[] requiredFields =
    {
        "id", "type", "client", "date",
        "volume", "distance", "basePrice"
    };

    foreach (string field in requiredFields)
    {
        if (!record.TryGetProperty(field, out JsonElement value)
            || value.ValueKind == JsonValueKind.Null)
        {
            return "MISSING_FIELD";
        }
    }

    JsonElement typeElement = record.GetProperty("type");

    if (typeElement.ValueKind != JsonValueKind.String)
    {
        return "UNKNOWN_TYPE";
    }

    string fuelType = typeElement.GetString()!.Trim().ToUpperInvariant();

    if (fuelType != "DIESEL"
        && fuelType != "GASOLINE"
        && fuelType != "LPG")
    {
        return "UNKNOWN_TYPE";
    }

    JsonElement clientElement = record.GetProperty("client");

    if (clientElement.ValueKind != JsonValueKind.String)
    {
        return "UNKNOWN_CLIENT";
    }

    string client = clientElement.GetString()!.Trim().ToUpperInvariant();

    if (client != "PRIVATE" && client != "EXEMPT")
    {
        return "UNKNOWN_CLIENT";
    }

    JsonElement dateElement = record.GetProperty("date");

    if (dateElement.ValueKind != JsonValueKind.String)
    {
        return "INVALID_DATE";
    }

    if (!DateTime.TryParseExact(
            dateElement.GetString(),
            new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _))
    {
        return "INVALID_DATE";
    }

    string[] numericFields = { "volume", "distance", "basePrice" };

    foreach (string field in numericFields)
    {
        JsonElement value = record.GetProperty(field);

        if (value.ValueKind != JsonValueKind.Number)
        {
            return "INVALID_VALUE";
        }

        if (!value.TryGetDecimal(out decimal number))
        {
            return "INVALID_VALUE";
        }

        if (field == "volume" && number <= 0m)
        {
            return "INVALID_VALUE";
        }

        if (field != "volume" && number < 0m)
        {
            return "INVALID_VALUE";
        }
    }

  
    if (record.GetProperty("id").ValueKind != JsonValueKind.String)
    {
        return "INVALID_VALUE";
    }

    return "";
}

private static string EscapeCsv(string value)
{
    if (value.Contains(",")
        || value.Contains("\"")
        || value.Contains("\n")
        || value.Contains("\r"))
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }

    return value;
}



    public static Result CalculateShipment(Shipment shipment, decimal? customVATrate = null, decimal? customGasolineExcise = null)
    {
        decimal discount = 0m;

        if (shipment.Volume > PricingRules.LargeVolumeThreshold)
        {
            discount = PricingRules.LargeVolumeDiscount;
        }
        else if (shipment.Volume > PricingRules.MediumVolumeThreshold)
        {
            discount = PricingRules.MediumVolumeDiscount;
        }

        decimal netFuelPrice = RoundMoney(
            shipment.Volume * shipment.BasePrice * (1m - discount)
        );

        decimal logisticsCost;

        if (shipment.Volume < PricingRules.SmallLoadThreshold)
        { 
            logisticsCost = RoundMoney(
                shipment.Distance * PricingRules.DeliveryPrice + PricingRules.SmallLoadFee
            );
        }
        else
        {
            logisticsCost = RoundMoney(
                shipment.Distance * PricingRules.DeliveryPrice
            );
        }

        string fuelType = shipment.Type.Trim().ToUpperInvariant();

        decimal exciseRate;

        if (fuelType == "DIESEL")
        {
            exciseRate = PricingRules.DieselExcise;
        }
        else if (fuelType == "GASOLINE")
        {
            if (customGasolineExcise.HasValue)
            {
                exciseRate = customGasolineExcise.Value;
            }
            else
            {
                exciseRate = PricingRules.GasolineExcise;
            }
        }
        
        else if (fuelType == "LPG")
        {
            exciseRate = PricingRules.LpgExcise;
        }
        else
        {
            throw new ArgumentException("UNKNOWN FUEL_TYPE.");
        }

        decimal excise = RoundMoney(shipment.Volume * exciseRate);

        DateTime date = DateTime.ParseExact(
            shipment.Date,
            new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None
        );

        string client = shipment.Client.Trim().ToUpperInvariant();

        decimal vatRate;

        if (client == "EXEMPT")
        {
            vatRate = 0m;
        }
        else if (client == "PRIVATE")
        {
            if (customVATrate.HasValue)
            {
                vatRate = customVATrate.Value;
            }
            else if (date < PricingRules.VATChangeDate)
            {
                vatRate = PricingRules.OldVATRate;
            }
            else
            {
                vatRate = PricingRules.NewVATRate;
            }
        }
        else
        {
            throw new ArgumentException("unknown CLIENT_TYPE.");
        }

        decimal vat = RoundMoney(
            (netFuelPrice + logisticsCost + excise) * vatRate
        );
        
        decimal totalTax = RoundMoney(excise + vat);

        decimal grandTotal = RoundMoney(
            netFuelPrice + logisticsCost + totalTax
        );

        return new Result
        {
            Id = shipment.Id,
            NetFuelPrice = netFuelPrice,
            LogisticsCost = logisticsCost,
            Excise = excise,
            VAT = vat,
            TotalTax = totalTax,
            GrandTotal = grandTotal
        };
    }

    private static decimal RoundMoney(decimal value)
    {
        return Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
    
    private static decimal CalculateLateInterest(
        decimal grandTotal,
        DateTime dueDate,
        int daysLate)
    {
        if (daysLate == 0)
        {
            return 0m;
        }

        int oldRateDays = 0;
        
        if (dueDate < PricingRules.InterestChangeDate)
        {
            int daysBeforeChange =
                (PricingRules.InterestChangeDate - dueDate).Days - 1;

            if (daysBeforeChange > daysLate)
            {
                oldRateDays = daysLate;
            }
            else
            {
                oldRateDays = daysBeforeChange;
            }
        }

        int newRateDays = daysLate - oldRateDays;

        decimal oldInterest = RoundMoney(
            grandTotal
            * PricingRules.OldInterestRate
            * oldRateDays
            / PricingRules.DaysPerYear
        );

        decimal newInterest = RoundMoney(
            grandTotal
            * PricingRules.NewInterestRate
            * newRateDays
            / PricingRules.DaysPerYear
        );

        return oldInterest + newInterest;
    }
}