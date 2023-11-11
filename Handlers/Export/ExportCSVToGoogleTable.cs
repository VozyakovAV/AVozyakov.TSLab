﻿namespace AVozyakov
{
    [HandlerCategory($"{SystemUtils.HandlerName}.Export")]
    [HandlerName("Экспорт в Google таблицу")]
    [Description("Экспорт в Google таблицу из CSV файла")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(1)]
    [OutputType(TemplateTypes.SECURITY)]
    public class ExportCSVToGoogleTable : IStreamHandler, IContextUses
    {
        [HandlerParameter(Name = "Файл CSV", Default = @"C:\\TSLab\\Balance.csv", NotOptimized = true)]
        [Description("Файл CSV с данными")]
        public string FileSource { get; set; }

        [HandlerParameter(Name = "Файл google настроек", Default = @"C:\\TSLab\\google_client.json", NotOptimized = true)]
        [Description("Файл настроек google api")]
        public string FileGoogle { get; set; }

        [HandlerParameter(Name = "Id таблицы", Default = @"1_T-ENPjpjhfEiVmoSNpYZDrhfEpiwCFNeuPyNNwL2uI", NotOptimized = true)]
        [Description("Id таблицы")]
        public string TableId { get; set; }

        [HandlerParameter(Name = "Название листа", Default = @"MyData", NotOptimized = true)]
        [Description("Название листа")]
        public string SheetName { get; set; }

        public IContext Context { get; set; }

        public ISecurity Execute(ISecurity sec)
        {
            try
            {
                var fileName = Path.Combine(SystemUtils.FolderHandles, SystemUtils.FolderGoogle, SystemUtils.FileGoogle);
                var args = $@"/cmd:append /fileSource:{FileSource} /table:{TableId} /sheet:{SheetName} /fileGoogle:{FileGoogle}";

                SystemUtils.RunProcess(fileName, args, out var output, out var error);

                if (!string.IsNullOrEmpty(output))
                    Context.Log(output, MessageType.Info);
                if (!string.IsNullOrEmpty(error))
                    Context.Log(error, MessageType.Error);
            }
            catch (Exception ex)
            {
                Context.Log(ex.ToString(), MessageType.Error);
            }

            return sec;
        }
    }
}