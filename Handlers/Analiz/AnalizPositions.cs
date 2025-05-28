namespace AVozyakov.Handlers.Analiz
{
    [HandlerCategory("AVozyakov.Analiz")]
    [HandlerName("Карта ситуаций")]
    [Description("Анализ входов позиций по методу Кургузкина (карта ситуаций)")]
    [InputsCount(1)]
    [Input(0, TemplateTypes.SECURITY)]
    [OutputsCount(0)]
    public class AnalizPositions : IValuesHandlerWithNumber, IValuesHandler, IHandler, IContextUses
    {
        [HandlerParameter(Name = "Период", Default = "100", NotOptimized = true)]
        public int Period { get; set; }

        public IContext Context { get; set; }

        public void Execute(ISecurity sec, int barNum)
        {
            if (Context.IsOptimization)
            {
                return;
            }
            int num = (Context.IsLastBarUsed ? (sec.Bars.Count - 1) : (sec.Bars.Count - 2));
            if (barNum < num)
            {
                return;
            }
            IEnumerable<IGrouping<string, IPosition>> enumerable = from x in sec.Positions
                                                                   group x by x.EntrySignalName;
            foreach (IGrouping<string, IPosition> item in enumerable)
            {
                IList<double> values = AnalizEntries(item, Period);
                Save(values, "C:\\Temp\\Analiz\\" + item.Key + ".txt");
            }
            IEnumerable<IGrouping<string, IPosition>> enumerable2 = from x in sec.Positions
                                                                    group x by x.ExitSignalName;
            foreach (IGrouping<string, IPosition> item2 in enumerable2)
            {
                IList<double> values2 = AnalizExits(item2, Period);
                Save(values2, "C:\\Temp\\Analiz\\" + item2.Key + ".txt");
            }
        }

        public static IList<double> AnalizEntries(IEnumerable<IPosition> positions, int countBars = 100)
        {
            if (positions == null || !positions.Any())
            {
                return new List<double>();
            }
            IList<double> closePrices = positions.First().Security.ClosePrices;
            int num = 0;
            double[] array = new double[countBars];
            foreach (IPosition position in positions)
            {
                double entryPrice = position.EntryPrice;
                if (entryPrice != 0.0)
                {
                    int num2 = Math.Min(countBars, closePrices.Count - position.EntryBarNum);
                    for (int i = 0; i < num2; i++)
                    {
                        double num3 = closePrices[position.EntryBarNum + i];
                        double num4 = (position.IsLong ? ((num3 - entryPrice) / entryPrice * 100.0) : ((entryPrice - num3) / num3 * 100.0));
                        array[i] += num4;
                    }
                    num++;
                }
            }
            for (int j = 0; j < countBars; j++)
            {
                array[j] = Math.Round(array[j] / (double)num, 6);
            }
            return array;
        }

        public static IList<double> AnalizExits(IEnumerable<IPosition> positions, int countBars = 100)
        {
            if (positions == null || !positions.Any())
            {
                return new List<double>();
            }
            IList<double> closePrices = positions.First().Security.ClosePrices;
            int num = 0;
            double[] array = new double[countBars];
            foreach (IPosition position in positions)
            {
                double exitPrice = position.ExitPrice;
                if (exitPrice != 0.0)
                {
                    int num2 = Math.Min(countBars, closePrices.Count - position.ExitBarNum);
                    for (int i = 0; i < num2; i++)
                    {
                        double num3 = closePrices[position.ExitBarNum + i];
                        double num4 = (position.IsLong ? ((num3 - exitPrice) / exitPrice * 100.0) : ((exitPrice - num3) / num3 * 100.0));
                        array[i] += num4;
                    }
                    num++;
                }
            }
            for (int j = 0; j < countBars; j++)
            {
                array[j] = Math.Round(array[j] / (double)num, 6);
            }
            return array;
        }

        public static void Save(IList<double> values, string filePath)
        {
            List<string> lines = values.Select((double x) => x.ToString()).ToList();
            FileUtils.WriteAllLines(filePath, lines);
        }
    }
}
