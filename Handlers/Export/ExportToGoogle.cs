namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.HandlerName}.Export")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public abstract class ExportToGoogle : IStreamHandler, INeedVariableId, IContextUses
    {
        public string VariableId { get; set; }
        public IContext Context { get; set; }

        [HandlerParameter(Name = "Файл данных", Default = @"C:\\TSLab\\Balance.csv", NotOptimized = true)]
        [Description("Файл с данными (csv)")]
        public string FileSource { get; set; }

        [HandlerParameter(Name = "Файл google настроек", Default = @"C:\\TSLab\\google_client.json", NotOptimized = true)]
        [Description("Файл настроек google api (json)")]
        public string FileGoogle { get; set; }

        [HandlerParameter(Name = "Id таблицы", Default = @"1_T-ENPjpjhfEiVmoSNpYZDrhfEpiwCFNeuPyNNwL2uI", NotOptimized = true)]
        [Description("Id таблицы")]
        public string TableId { get; set; }

        [HandlerParameter(Name = "Название листа", Default = @"MyData", NotOptimized = true)]
        [Description("Название листа")]
        public string SheetName { get; set; }

        protected abstract string Command { get; }

        public ISecurity Execute(ISecurity sec)
        {
            if (!SystemUtils.CanExecuteByTime(VariableId, 5))
                return sec;

            try
            {
                var fileName = Path.Combine(SystemUtils.FolderHandles, SystemUtils.FolderGoogle, SystemUtils.FileGoogle);
                var args = $@"/cmd:{Command} /fileSource:{FileSource} /table:{TableId} /sheet:{SheetName} /fileGoogle:{FileGoogle}";

                SystemUtils.RunProcess(fileName, args, out var output, out var error);

                if (!string.IsNullOrEmpty(output))
                    Context.Log(output, MessageType.Info);
                if (!string.IsNullOrEmpty(error))
                    Context.Log(error, MessageType.Error, toMessageWindow: true);
            }
            catch (Exception ex)
            {
                Context.Log(ex.ToString(), MessageType.Error);
            }

            return sec;
        }
    }

    [HandlerName("Экспорт в Google таблицу (добавить)")]
    [Description("Добавить в Google таблицу данные из файла")]
    public class ExportToGoogleAppend : ExportToGoogle
    {
        protected override string Command => "append";
    }

    [HandlerName("Экспорт в Google таблицу (обновить)")]
    [Description("Обновить в Google таблице данные из файла.\r\n" +
        "Внимание! Перед записью очищает весь лист!")]
    public class ExportToGoogleUpdate : ExportToGoogle
    {
        protected override string Command => "update";
    }
}