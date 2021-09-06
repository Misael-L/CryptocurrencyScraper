using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Threading;

namespace CryptocurrencyScraper
{
    class Program
    {
        public static List<CryptoCurrency> _cryptoCurrencyList = new List<CryptoCurrency>();

        public class CryptoCurrency
        {
            public string CurrencyName { get; set; } = string.Empty;
            public string CurrencyTicker { get; set; } = string.Empty;
            public string Price { get; set; } = string.Empty;
            public string MarketCap { get; set; } = string.Empty;
        }

        public static void Main(string[] args)
        {
            HtmlWeb web = new HtmlWeb();
            var crawlDelay = 1000;
            HtmlDocument urlDoc = web.Load("https://www.coingecko.com/en");
            Thread.Sleep(crawlDelay);

            try
            {
                var currencyTable = urlDoc.DocumentNode.SelectNodes("//tbody/tr");
                double totalPageCount = 9300 / 100.00;

                for (var i = 1; i <= totalPageCount; i++)
                {
                    var url = string.Format("https://www.coingecko.com/en?page={0}", i);
                    urlDoc = web.Load(url);
                    var cryptoList = urlDoc.DocumentNode.SelectNodes("//div/div/div/div/table[@class='sort table mb-0 text-sm text-lg-normal table-scrollable']//tbody/tr");

                    foreach (var currency in cryptoList)
                    {
                        string currencyName = currency.SelectSingleNode(".//div/a[@class='tw-hidden lg:tw-flex font-bold tw-items-center tw-justify-between']") != null 
                                            ? currency.SelectSingleNode(".//div/a[@class='tw-hidden lg:tw-flex font-bold tw-items-center tw-justify-between']").InnerText.Trim() 
                                            : string.Empty;
                        
                        string currencyTicker = currency.SelectSingleNode(".//div/span[@class='tw-hidden d-lg-inline font-normal text-3xs ml-2']") != null 
                                        ? currency.SelectSingleNode(".//div/span[@class='tw-hidden d-lg-inline font-normal text-3xs ml-2']").InnerText.Trim() 
                                        : string.Empty;

                        string price = currency.SelectSingleNode(".//td[@class='td-price price text-right']//span[@class='no-wrap']") != null 
                                     ? currency.SelectSingleNode(".//td[@class='td-price price text-right']//span[@class='no-wrap']").InnerText.Trim() 
                                     : string.Empty;
                        
                        string marketCap = currency.SelectSingleNode(".//td[@class='td-market_cap cap col-market cap-price text-right']//span[@class='no-wrap']") != null 
                                         ? currency.SelectSingleNode(".//td[@class='td-market_cap cap col-market cap-price text-right']//span[@class='no-wrap']").InnerText.Trim() 
                                         : string.Empty;
                       
                        AddToCurrencyList(_cryptoCurrencyList, currencyName, currencyTicker, price, marketCap);
                    }
                }
            }
            catch (Exception e)
            {
            }

            WriteDataToCSV();
        }

        public static void AddToCurrencyList(List<CryptoCurrency> cryptoCurrencyList, string currencyName, string currencyTicker, string price, string marketCap)
        {
            cryptoCurrencyList.Add(new CryptoCurrency
            {
                CurrencyName = currencyName,
                CurrencyTicker = currencyTicker,
                Price = price,
                MarketCap = marketCap
            });
        }

        public static void WriteDataToCSV()
        {
            var csvBuilder = new StringBuilder();

            foreach (var currency in _cryptoCurrencyList)
            {
                csvBuilder.AppendLine(string.Format("{0},\"{1}\",\"{2}\",\"{3}\"", currency.CurrencyName, currency.CurrencyTicker, currency.Price, currency.MarketCap));
            }

            File.WriteAllText("C:\\Users\\Misael Lopez\\Desktop\\Text CSV Files\\Crypto List.csv", csvBuilder.ToString());
        }

    }
}
