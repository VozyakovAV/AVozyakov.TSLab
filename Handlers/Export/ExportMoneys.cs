namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.HandlerName}.Export")]
    [HandlerName("Экспорт денег")]
    [Description("Экспорт денег из таблицы Позиции.\r\n" +
        "Работает только при подключенном поставщике.")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportMoneys : IStreamHandler, INeedVariableId
    {
        private const char Delimeter = ';';
        public string VariableId { get; set; }

        [HandlerParameter(Name = "Файл", Default = @"C:\\TSLab\\Moneys.csv", NotOptimized = true)]
        public string FileName { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            if (!SystemUtils.CanExecuteByTime(VariableId, 5))
                return sec;

            var ds = sec.GetPortfolioSource();
            if (ds == null || ds.ConnectionState != DSConnectionState.Connected)
                return sec;

            var sb = new StringBuilder();
            sb.Append("Время").Append(Delimeter);
            sb.Append("Поставщик").Append(Delimeter);
            sb.Append("Валюта").Append(Delimeter);
            sb.Append("Счет").Append(Delimeter);

            sb.Append("Входящая").Append(Delimeter);
            sb.Append("Текущая").Append(Delimeter);
            sb.Append("Плановая").Append(Delimeter);
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
                    sb.Append(DateTime.Now).Append(Delimeter);
                    sb.Append(ds.Name).Append(Delimeter);
                    sb.Append(b.Security.Currency).Append(Delimeter);
                    sb.Append(b.AccountName).Append(Delimeter);

                    sb.Append(b.IncomeRest.Round(decimals)).Append(Delimeter);
                    sb.Append(b.RealRest.Round(decimals)).Append(Delimeter);
                    sb.Append(b.PlanRest.Round(decimals)).Append(Delimeter);
                    sb.AppendLine();
                }
            }

            File.WriteAllText(FileName, sb.ToString(), Encoding.GetEncoding(1251));
            return sec;
        }
    }
}