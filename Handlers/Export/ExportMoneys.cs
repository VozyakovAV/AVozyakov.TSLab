namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.Handler}.Export")]
    [HandlerName("Экспорт денег")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportMoneys : IStreamHandler
    {
        private const char Delimeter = ';';

        [HandlerParameter(true, @"C:\\Temp\\TSLab\\Moneys.csv", NotOptimized = true)]
        public string FileName { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            var ds = sec?.SecurityDescription?.TradePlace?.DataSource as IPortfolioSourceBase;
            if (ds == null)
                return sec;

            var sb = new StringBuilder();
            sb.Append("Поставщик").Append(Delimeter);
            sb.Append("Счет").Append(Delimeter);
            sb.Append("Рынок").Append(Delimeter);
            sb.Append("Инструмент").Append(Delimeter);
            sb.Append("Валюта").Append(Delimeter);
            sb.Append("Входящая").Append(Delimeter);
            sb.Append("Текущая").Append(Delimeter);
            sb.Append("Плановая").Append(Delimeter);
            sb.Append("UpdateTime").Append(Delimeter);
            sb.AppendLine();

            foreach (var account in ds.Accounts)
            {
                var balances = ds.GetBalances(account.Id)
                    .Where(x => x.Security.IsMoney)
                    .OrderBy(x => x.SecurityName)
                    .ToList();

                foreach (var b in balances)
                {
                    var decimals = b.Security.Decimals;
                    sb.Append(ds.Name).Append(Delimeter);
                    sb.Append(b.AccountName).Append(Delimeter);
                    sb.Append(b.TradePlaceName).Append(Delimeter);
                    sb.Append(b.SecurityName).Append(Delimeter);
                    sb.Append(b.Security.Currency).Append(Delimeter);
                    sb.Append(GetValue(b.IncomeRest, decimals)).Append(Delimeter);
                    sb.Append(GetValue(b.RealRest, decimals)).Append(Delimeter);
                    sb.Append(GetValue(b.PlanRest, decimals)).Append(Delimeter);
                    sb.Append(DateTime.Now).Append(Delimeter);
                    sb.AppendLine();
                }
            }

            File.WriteAllText(FileName, sb.ToString(), Encoding.GetEncoding(1251));

            return sec;
        }

        private static decimal? GetValue(double? value, int decimals)
        {
            if (value == null)
                return null;
            return (decimal)Math.Round(value.Value, decimals);
        }
    }
}