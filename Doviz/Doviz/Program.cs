using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using System.ComponentModel.Design;

class Program
{
    static async Task Main(string[] args)
    {
        string url = "https://www.tcmb.gov.tr/kurlar/today.xml";



        while (true)
        {
            try
            {
                HttpClient client = new HttpClient();
                string response = await client.GetStringAsync(url);
                XDocument xmlDoc = XDocument.Parse(response);



                Console.WriteLine("1.Normal Kur");
                Console.WriteLine("2.Çapraz Kur");
                Console.WriteLine("Hangi Kur Üzeridnen İşlem Yapacaksınız?");
                string secim = Console.ReadLine();

                if (secim == "1")
                {
                    var currencies = xmlDoc.Descendants("Currency")
                    .Where(x => x.Element("ForexBuying") != null)
                    .Select(x => new
                    {
                        CurrencyCode = x.Attribute("CurrencyCode").Value,
                        ForexBuying = x.Element("ForexBuying").Value,

                    }).ToList();

                    Console.WriteLine("Lütfen görmek istediğiniz para biriminin numarasını girin:");
                    for (int i = 0; i < currencies.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {currencies[i].CurrencyCode}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= currencies.Count)
                    {
                        var selectedCurrency = currencies[selectedIndex - 1];
                        Console.WriteLine($"Para Birimi: {selectedCurrency.CurrencyCode}, Alış Kuru: {selectedCurrency.ForexBuying}");
                    }
                    else
                    {
                        Console.WriteLine("Geçersiz bir seçim yaptınız.");
                    }

                }
                else if (secim == "2")
                {

                    var currencies = xmlDoc.Descendants("Currency")
                                           .Where(x => x.Element("CrossRateUSD") != null && x.Element("CrossRateOther") != null)
                                           .Select(x => new
                                           {
                                               CurrencyCode = x.Attribute("CurrencyCode").Value,
                                               CrossRateUSD = x.Element("CrossRateUSD").Value,
                                               CrossRateOther = x.Element("CrossRateOther").Value
                                           }).ToList();

                    Console.WriteLine("Lütfen görmek istediğiniz para biriminin numarasını girin:");
                    for (int i = 0; i < currencies.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {currencies[i].CurrencyCode}");
                    }

                    if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= currencies.Count)
                    {
                        var selectedCurrency = currencies[selectedIndex - 1];
                        if (Convert.ToDecimal(selectedCurrency.CrossRateOther) > Convert.ToDecimal(selectedCurrency.CrossRateUSD))
                        {
                            Console.WriteLine($"1 {selectedCurrency.CurrencyCode} = {selectedCurrency.CrossRateOther} Dolardır.");
                        }
                        else if (Convert.ToDecimal(selectedCurrency.CrossRateOther) < Convert.ToDecimal(selectedCurrency.CrossRateUSD))
                        {
                            Console.WriteLine($"1 Dolar = {selectedCurrency.CrossRateOther} {selectedCurrency.CurrencyCode}");
                        }



                        else
                        {
                            Console.WriteLine("Geçersiz bir seçim yaptınız.");
                        }
                    }
                }
            }

            catch (HttpRequestException e)
            {
                Console.WriteLine($"İstek hatası: {e.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Beklenmedik bir hata oluştu: {e.Message}");
            }
        }
    }
}

