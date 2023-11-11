namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.Handler}.Export")]
    [HandlerName("Экспорт агентов")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportAgents : IStreamHandler
    {
        private const char Delimeter = ';';

        [HandlerParameter(true, @"C:\\Temp\\TSLab\\Agents.csv", NotOptimized = true)]
        public string FileName { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            var service = Locator.Current.GetInstance<IAgentTradingService>();
            if (service == null)
                return sec;

            var sb = new StringBuilder();
            sb.Append("Поставщик").Append(Delimeter);
            sb.Append("Счет").Append(Delimeter);
            sb.Append("Инструмент").Append(Delimeter);
            sb.Append("Чистая ст-ть").Append(Delimeter);
            sb.Append("Позиция").Append(Delimeter);
            sb.Append("Расчетная позиция").Append(Delimeter);
            sb.Append("Вне агентов").Append(Delimeter);
            sb.Append("Расхождение").Append(Delimeter);
            sb.Append("UpdateTime").Append(Delimeter);
            sb.AppendLine();

            foreach (var item in service.Positions.Items.Where(x => x.IsActive))
            {
                var decimals = item.Security.BalanceDecimals;
                var lotDecimals = item.Security.LotTick.GetDecimalPlaces();
                sb.Append(item.DataSource.Name).Append(Delimeter);
                sb.Append(item.Account.Name).Append(Delimeter);
                sb.Append(item.Security.Name).Append(Delimeter);
                sb.Append(GetValue(item.Balance, decimals)).Append(Delimeter);
                sb.Append(GetValue(item.Current, lotDecimals)).Append(Delimeter);
                sb.Append(GetValue(item.CalculatedBalance, lotDecimals)).Append(Delimeter);
                sb.Append(GetValue(item.ManualTradePosition, lotDecimals)).Append(Delimeter);
                sb.Append(GetValue(item.Divergence, lotDecimals)).Append(Delimeter);
                sb.Append(DateTime.Now).Append(Delimeter);
                sb.AppendLine();
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