namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.HandlerName}.Export")]
    [HandlerName("Экспорт позиций")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportPositions : IStreamHandler
    {
        private const char Delimeter = ';';

        [HandlerParameter(true, @"C:\\TSLab\\Positions.csv", NotOptimized = true)]
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
            sb.Append("Уч.цена").Append(Delimeter);
            sb.Append("Оцен.цена").Append(Delimeter);
            sb.Append("Чистая ст-ть").Append(Delimeter);
            sb.Append("НП/У").Append(Delimeter);
            sb.Append("UpdateTime").Append(Delimeter);
            sb.AppendLine();

            foreach (var account in ds.Accounts)
            {
                var balances = ds.GetBalances(account.Id)
                    .Where(x => !x.Security.IsMoney)
                    .OrderBy(x => x.SecurityName)
                    .ToList();

                foreach (var b in balances)
                {
                    var decimals = b.Security.BalanceDecimals;
                    var lotDecimals = b.Security.LotTick.GetDecimalPlaces();
                    sb.Append(ds.Name).Append(Delimeter);
                    sb.Append(b.AccountName).Append(Delimeter);
                    sb.Append(b.TradePlaceName).Append(Delimeter);
                    sb.Append(b.SecurityName).Append(Delimeter);
                    sb.Append(b.Security.Currency).Append(Delimeter);
                    sb.Append(b.IncomeRest.Round(lotDecimals)).Append(Delimeter);
                    sb.Append(b.RealRest.Round(lotDecimals)).Append(Delimeter);
                    sb.Append(b.PlanRest.Round(lotDecimals)).Append(Delimeter);
                    sb.Append(b.BalancePrice.Round(decimals)).Append(Delimeter);
                    sb.Append(b.AssessedPrice.Round(decimals)).Append(Delimeter);
                    sb.Append(b.Balance.Round(decimals)).Append(Delimeter);
                    sb.Append(b.ProfitVolume.Round(decimals)).Append(Delimeter);
                    sb.Append(DateTime.Now).Append(Delimeter);
                    sb.AppendLine();
                }
            }

            File.WriteAllText(FileName, sb.ToString(), Encoding.GetEncoding(1251));
            return sec;
        }
    }
}