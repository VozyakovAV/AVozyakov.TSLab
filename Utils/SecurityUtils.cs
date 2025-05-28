namespace AVozyakov.Utils
{
    public static class SecurityUtils
    {
        public static IPortfolioSourceBase GetPortfolioSource(this ISecurity sec)
        {
            return sec?.SecurityDescription?.TradePlace?.DataSource as IPortfolioSourceBase;
        }
    }
}
