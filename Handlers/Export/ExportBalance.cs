namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.HandlerName}.Export")]
    [HandlerName("Экспорт баланса")]
    [Description("Экспорт баланса в Google таблицу.\r\n" +
        "Время округляет до указанного значения.\r\n" +
        "Например, сейчас 12:05, округление времени 1440 минут (1 день), тогда в таблицу запишется 00:00.\r\n" +
        "Работает только при подключенном поставщике.")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportBalance : IStreamHandler
    {
        private const char Delimeter = ';';

        [HandlerParameter(Name = "Файл", Default = @"C:\\TSLab\\Balance.csv", NotOptimized = true)]
        public string FileName { get; set; }

        [HandlerParameter(Name = "Округление времени (мин)", Default = @"1440", NotOptimized = true)]
        public int IntervalMin { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            var ds = sec.GetPortfolioSource();
            if (ds == null || ds.ConnectionState != DSConnectionState.Connected)
                return sec;
            
            var accountId = ds?.Accounts?.FirstOrDefault()?.Id;
            if (accountId == null)
                return sec;

            var accountInfo = ds.GetAccountInfo(accountId);
            
            var sb = new StringBuilder();
            sb.Append("Время").Append(Delimeter);
            sb.Append("Баланс").Append(Delimeter);
            sb.Append("Доступно").Append(Delimeter);
            sb.AppendLine();

            sb.Append(DateTime.Now.RoundDown(TimeSpan.FromMinutes(IntervalMin))).Append(Delimeter);
            sb.Append(accountInfo.FullBalance).Append(Delimeter);
            sb.Append(accountInfo.AvailableBalance).Append(Delimeter);
            sb.AppendLine();

            File.WriteAllText(FileName, sb.ToString(), Encoding.GetEncoding(1251));
            return sec;
        }
    }
}