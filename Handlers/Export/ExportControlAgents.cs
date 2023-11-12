namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.HandlerName}.Export")]
    [HandlerName("Экспорт контроля агентов")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportControlAgents : IStreamHandler, INeedVariableId
    {
        private const char Delimeter = ';';
        public string VariableId { get; set; }

        [HandlerParameter(Name = "Файл", Default = @"C:\\TSLab\\ControlAgents.csv", NotOptimized = true)]
        public string FileName { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            if (!SystemUtils.CanExecuteByTime(VariableId, 5))
                return sec;

            var service = Locator.Current.GetInstance<IAgentTradingService>();
            if (service == null)
                return sec;

            var sb = new StringBuilder();
            sb.Append("Время").Append(Delimeter);
            sb.Append("Поставщик").Append(Delimeter);
            sb.Append("Счет").Append(Delimeter);
            sb.Append("Инструмент").Append(Delimeter);

            sb.Append("Чистая ст-ть").Append(Delimeter);
            sb.Append("Позиция").Append(Delimeter);
            sb.Append("Расчетная позиция").Append(Delimeter);
            sb.Append("Вне агентов").Append(Delimeter);
            sb.Append("Расхождение").Append(Delimeter);
            sb.AppendLine();

            foreach (var item in service.Positions.Items.Where(x => x.IsActive))
            {
                var decimals = item.Security.BalanceDecimals;
                var lotDecimals = item.Security.LotTick.GetDecimalPlaces();
                sb.Append(DateTime.Now).Append(Delimeter);
                sb.Append(item.DataSource.Name).Append(Delimeter);
                sb.Append(item.Account.Name).Append(Delimeter);
                sb.Append(item.Security.Name).Append(Delimeter);

                sb.Append(item.Balance.Round(decimals)).Append(Delimeter);
                sb.Append(item.Current.Round(lotDecimals)).Append(Delimeter);
                sb.Append(item.CalculatedBalance.Round(lotDecimals)).Append(Delimeter);
                sb.Append(item.ManualTradePosition.Round(lotDecimals)).Append(Delimeter);
                sb.Append(item.Divergence.Round(lotDecimals)).Append(Delimeter);
                sb.AppendLine();
            }

            File.WriteAllText(FileName, sb.ToString(), Encoding.GetEncoding(1251));
            return sec;
        }
    }
}