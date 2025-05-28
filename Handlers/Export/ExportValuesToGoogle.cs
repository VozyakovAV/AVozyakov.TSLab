namespace AVozyakov.Handlers.Export
{
    [HandlerCategory("AVozyakov.Export")]
    [HandlerName("Экспорт значений в Google таблицу")]
    [Description("Экспорт значений в Google таблицу (только добавление).\r\n" +
        "Время округляет до указанного значения.\r\n" +
        "Например, сейчас 12:05, округление времени 1440 минут (1 день), тогда в таблицу запишется 00:00.\r\n" +
        "Работает только при подключенном поставщике.")]
    [InputsCount(2, 50)]
    [Input(0, TemplateTypes.SECURITY)]
    [Input(1, TemplateTypes.DOUBLE)]
    [OutputsCount(0)]
    public class ExportValuesToGoogle : IValuesHandlerWithNumber, IValuesHandler, IHandler, IContextUses, INeedVariableId
    {
        private const char Delimeter = ';';

        public IContext Context { get; set; }

        public string VariableId { get; set; }

        [HandlerParameter(Name = "Файл google настроек", Default = "C:\\\\TSLab\\\\google_client.json", NotOptimized = true)]
        [Description("Файл настроек google api (json)")]
        public string FileGoogle { get; set; }

        [HandlerParameter(Name = "Id таблицы", Default = "1_T-ENPjpjhfEiVmoSNpYZDrhfEpiwCFNeuPyNNwL2uI", NotOptimized = true)]
        [Description("Id таблицы")]
        public string TableId { get; set; }

        [HandlerParameter(Name = "Название листа", Default = "MyData", NotOptimized = true)]
        [Description("Название листа")]
        public string SheetName { get; set; }

        [HandlerParameter(Name = "Округление времени (мин)", Default = "1440", NotOptimized = true)]
        public int IntervalMin { get; set; }

        public void Execute(ISecurity sec, params double[] values)
        {
            int index = Convert.ToInt32(values.Last());
            if (!IsLastIndex(index) || !SystemUtils.CanExecuteByTime(VariableId, 5))
            {
                return;
            }

            var portfolioSource = sec.GetPortfolioSource();
            if (portfolioSource != null && portfolioSource.ConnectionState == DSConnectionState.Connected)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.Append(DateTime.Now.RoundDown(TimeSpan.FromMinutes(IntervalMin))).Append(Delimeter);
                for (int i = 0; i < values.Length - 1; i++)
                {
                    stringBuilder.Append(values[i]).Append(Delimeter);
                }
                stringBuilder.AppendLine();

                var text = Path.Combine(AppPath.TempFolder, FileUtil.GetValidFilePath("ExportValuesToGoogle_" + VariableId + ".txt"));
                FileUtils.WriteAllText(text, stringBuilder.ToString(), Encoding.GetEncoding(1251));

                var exportToGoogleAppend = new ExportToGoogleAppend
                {
                    VariableId = VariableId + "0",
                    Context = Context,
                    FileGoogle = FileGoogle,
                    TableId = TableId,
                    SheetName = SheetName,
                    FileSource = text
                };
                exportToGoogleAppend.Execute(sec);
            }
        }

        private bool IsLastIndex(int index)
        {
            int num = Context.BarsCount;
            if (!Context.IsLastBarUsed)
            {
                num--;
            }
            return index >= num - 1;
        }
    }
}
