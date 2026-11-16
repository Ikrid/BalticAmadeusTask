namespace BalticAmadeusTask;

public static class PricingRules
{
    public const decimal LargeVolumeThreshold = 20000m;
    public const decimal MediumVolumeThreshold = 5000m;
    public const decimal LargeVolumeDiscount = 0.10m;
    public const decimal MediumVolumeDiscount = 0.05m;

    public const decimal DeliveryPrice = 1.80m;
    public const decimal SmallLoadThreshold = 1000m;
    public const decimal SmallLoadFee = 150m;

    public const decimal DieselExcise = 0.33m;
    public const decimal GasolineExcise = 0.47m;
    public const decimal LpgExcise = 0.18m;

    public const decimal OldVATRate = 0.21m;
    public const decimal NewVATRate = 0.23m;

    public static readonly DateTime VATChangeDate = new(2024, 1, 1);
    
    
    public const int PaymentTermDays = 30;
    public const decimal OldInterestRate = 0.12m;
    public const decimal NewInterestRate = 0.09m;
    public const decimal DaysPerYear = 365m;

    public static readonly DateTime InterestChangeDate = new(2024, 7, 1);
    public static readonly DateTime ReportDate = new(2024, 7, 20);
}