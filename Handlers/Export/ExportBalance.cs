namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.Handler}.Export")]
    [HandlerName("Экспорт счета")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportBalance : IStreamHandler
    {
        private const char Delimeter = ';';

        [HandlerParameter(true, @"C:\\Temp\\TSLab\\Balance.csv", NotOptimized = true)]
        public string FileName { get; set; }

        [HandlerParameter(Name = "Интервал записи (мин)", Default = @"1440", NotOptimized = true)]
        public int IntervalMin { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            var secRt = sec as ISecurityRt;
            if (secRt == null)
                return sec;

            var sb = new StringBuilder();
            sb.Append("Время").Append(Delimeter);
            sb.Append("Баланс").Append(Delimeter);
            sb.Append("Доступно").Append(Delimeter);
            sb.AppendLine();

            var time = DateTime.Now.RoundDown(TimeSpan.FromMinutes(IntervalMin));
            sb.Append(time).Append(Delimeter);
            sb.Append(secRt.EstimatedBalance).Append(Delimeter);
            sb.Append(secRt.CurrencyBalance).Append(Delimeter);
            sb.AppendLine();

            File.WriteAllText(FileName, sb.ToString(), Encoding.GetEncoding(1251));

            return sec;
        }
    }
}