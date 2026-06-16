using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace FitCalc
{
    public enum CelDietetyczny { Redukcja = 1, Utrzymanie = 2, Masa = 3 }
    public enum CelTreningowy { Sila = 1, Hipertrofia = 2 }

    public struct ProfilUzytkownika
    {
        public decimal Waga;
        public int Wzrost;
        public int Wiek;
        public CelDietetyczny Cel;
    }

    public struct Danie
    {
        public string Nazwa;
        public string SkladnikiOpis;
        public int BazoweKcal;
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "FitCalc";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            bool programDziala = true;

            while (programDziala)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("              ╔═════════════════════════════════════════════════════╗");
                Console.WriteLine("              ║                  WITAJ W FITCALC                    ║");
                Console.WriteLine("              ╚═════════════════════════════════════════════════════╝");
                Console.ResetColor();

                Console.WriteLine("\n              Co dzisiaj robimy:");
                Console.WriteLine("\n              [1] Kalkulator zapotrzebowania kalorycznego, BMI oraz zalecana ilość wody");
                Console.WriteLine("              [2] Kalkulator ciężaru maksymalnego 1RM oraz układanie planu periodyzacji");
                Console.WriteLine("              [3] Szamka - przepisy pod makro");
                Console.WriteLine("              [4] Porady dla początkujących");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n              [5] Wychodzę");
                Console.ResetColor();
                Console.Write("\n              Podaj numer [1-5]: ");

                string wybor = Console.ReadLine();

                switch (wybor)
                {
                    case "1":
                        new ModulDietetyczny().Uruchom();
                        break;
                    case "2":
                        new ModulTreningowy().Uruchom();
                        break;
                    case "3":
                        new GeneratorPosilkowCSV().Uruchom();
                        break;
                    case "4":
                        Porady.Uruchom();
                        break;
                    case "5":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n              Zamykamy. Do zobaczenia na treningu!");
                        Console.ResetColor();
                        programDziala = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n              Spróbuj jeszcze raz.");
                        Console.ResetColor();
                        Console.WriteLine("              Wciśnij cokolwiek, aby wrócić...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    public static class Narzedzia
    {
        public static decimal PobierzLiczbe(string zacheta)
        {
            decimal wynik;
            while (true)
            {
                Console.Write(zacheta);
                try
                {
                    if (decimal.TryParse(Console.ReadLine(), out wynik) && wynik > 0)
                        return wynik;
                    else
                        throw new FormatException("To musi być liczba większa od zera.");
                }
                catch (FormatException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"              [BŁĄD] {ex.Message}\n");
                    Console.ResetColor();
                }
            }
        }
    }

    public class ModulDietetyczny
    {
        public void Uruchom()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("              ╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("              ║             KALKULATOR ZAPOTRZEBOWANIA              ║");
            Console.WriteLine("              ╚═════════════════════════════════════════════════════╝");
            Console.ResetColor();

            ProfilUzytkownika profil = new ProfilUzytkownika();
            bool uzytoZapisanego = false;
            bool odczytanoPoprawnie = false;

            if (File.Exists("Profil.csv"))
            {
                try
                {
                    using (StreamReader sr = new StreamReader("Profil.csv"))
                    {
                        string linia = sr.ReadLine();
                        if (linia != null)
                        {
                            string[] dane = linia.Split(';');
                            profil.Waga = decimal.Parse(dane[0]);
                            profil.Wzrost = int.Parse(dane[1]);
                            profil.Wiek = int.Parse(dane[2]);

                            int wczytanyCel = int.Parse(dane[3]);
                            profil.Cel = (CelDietetyczny)wczytanyCel;
                            odczytanoPoprawnie = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("              Coś poszło nie tak przy czytaniu profilu: " + ex.Message);
                }
            }

            if (odczytanoPoprawnie)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n              [Mamy Twój zapisany profil!]");
                Console.ResetColor();
                Console.WriteLine($"              Waga: {profil.Waga} kg | Wzrost: {profil.Wzrost} cm | Wiek: {profil.Wiek} lat | Cel: {profil.Cel}");

                Console.Write("\n              Co robimy? (T - lecimy z tym, C - zmieniam tylko cel, N - wpisuję od nowa): ");
                string odp = Console.ReadLine().ToUpper();

                if (odp == "T")
                {
                    uzytoZapisanego = true;
                }
                else if (odp == "C")
                {
                    Console.WriteLine("\n              Wybierz nowy cel:\n              [1] Redukcja\n              [2] Utrzymanie\n              [3] Masa");
                    int nowyCel;
                    while (!int.TryParse(Console.ReadLine(), out nowyCel) || nowyCel < 1 || nowyCel > 3)
                    {
                        Console.Write("              Wybierz 1, 2 lub 3: ");
                    }
                    profil.Cel = (CelDietetyczny)nowyCel;

                    try
                    {
                        using (StreamWriter sw = new StreamWriter("Profil.csv"))
                        {
                            sw.WriteLine($"{profil.Waga};{profil.Wzrost};{profil.Wiek};{(int)profil.Cel}");
                        }
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("              Cel zaaktualizowany!");
                        Console.ResetColor();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("              Nie udało się zaktualizować celu: " + ex.Message);
                    }
                    uzytoZapisanego = true;
                }
            }

            if (uzytoZapisanego == false)
            {
                Console.WriteLine("\n              --- NOWE DANE ---");
                profil.Waga = Narzedzia.PobierzLiczbe("              Podaj swoją wagę (kg): ");
                profil.Wzrost = (int)Narzedzia.PobierzLiczbe("              Podaj swój wzrost (cm): ");
                profil.Wiek = (int)Narzedzia.PobierzLiczbe("              Podaj swój wiek (lata): ");

                Console.WriteLine("\n              Cel:\n              [1] Redukcja\n              [2] Utrzymanie\n              [3] Masa");
                int wyborCelu;
                while (!int.TryParse(Console.ReadLine(), out wyborCelu) || wyborCelu < 1 || wyborCelu > 3)
                {
                    Console.Write("              Wybierz 1, 2 lub 3: ");
                }
                profil.Cel = (CelDietetyczny)wyborCelu;

                try
                {
                    using (StreamWriter sw = new StreamWriter("Profil.csv"))
                    {
                        sw.WriteLine($"{profil.Waga};{profil.Wzrost};{profil.Wiek};{(int)profil.Cel}");
                    }
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("              Profil zapisany elegancko!");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("              Problem z zapisem: " + ex.Message);
                }
            }

            int bmr = ObliczBMR(profil.Waga, profil.Wzrost, profil.Wiek);
            int tdee = (int)(bmr * 1.6);
            int doceloweKcal = tdee;
            decimal zmianaWagiTygodniowo = 0;

            if (profil.Cel == CelDietetyczny.Redukcja || profil.Cel == CelDietetyczny.Masa)
            {
                string akcja = profil.Cel == CelDietetyczny.Redukcja ? "zrzucać" : "łapać";
                Console.WriteLine($"\n              W jakim tempie chcesz {akcja} wagę? (kg / tydzień)");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("              Zdrowe tempo to od 0,2 do 0,5 kg.");
                Console.ResetColor();

                decimal tempo;
                while (true)
                {
                    tempo = Narzedzia.PobierzLiczbe("              > Podaj wartość (np. 0,3): ");
                    if (tempo <= 1.5m)
                    {
                        break;
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("              Trochę za szybko! Podaj bezpieczniejszą wartość (max 1.5).");
                    Console.ResetColor();
                }

                int kalorieZeZmiany = (int)(tempo * 1000m);

                if (profil.Cel == CelDietetyczny.Redukcja)
                {
                    doceloweKcal -= kalorieZeZmiany;
                    zmianaWagiTygodniowo = -tempo;
                }
                else if (profil.Cel == CelDietetyczny.Masa)
                {
                    doceloweKcal += kalorieZeZmiany;
                    zmianaWagiTygodniowo = tempo;
                }
            }

            int bialko = (int)(profil.Waga * 2.2m);
            int tluszcze = (int)(profil.Waga * 1.0m);
            int wegle = (doceloweKcal - (bialko * 4) - (tluszcze * 9)) / 4;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n              Całkowite zapotrzebowanie: {tdee} kcal");
            Console.WriteLine($"              ZALECANE SPOŻYCIE ({profil.Cel.ToString().ToUpper()}):   {doceloweKcal} kcal");
            Console.ResetColor();

            WypiszMakro(bialko, tluszcze, wegle);

            if (zmianaWagiTygodniowo != 0)
            {
                decimal nowaWaga = PrognozujWageRekurencyjnie(profil.Waga, zmianaWagiTygodniowo, 4);
                Console.WriteLine($"\n              > Twoja prognozowana waga za 4 tyg.: {Math.Round(nowaWaga, 1)} kg");
            }

            Console.WriteLine("\n              --- ANALIZA SYLWETKI I NAWODNIENIA ---");
            decimal wzrostMetry = profil.Wzrost / 100m;
            decimal bmi = profil.Waga / (wzrostMetry * wzrostMetry);

            int woda = (int)(profil.Waga * 35);

            Console.WriteLine($"              * Twoje BMI: {Math.Round(bmi, 1)}");
            Console.Write("              * Status wagowy: ");
            if (bmi < 18.5m) Console.WriteLine("Niedowaga");
            else if (bmi < 25m) Console.WriteLine("Norma (Zdrowa waga)");
            else if (bmi < 30m) Console.WriteLine("Nadwaga");
            else Console.WriteLine("Otyłość");

            Console.WriteLine($"              * Zalecane picie: {woda} ml");

            Console.WriteLine("\n              Wciśnij cokolwiek, aby wrócić...");
            Console.ReadKey();
        }

        
        private int ObliczBMR(decimal waga, int wzrost, int wiek) => (int)((10m * waga) + (6.25m * wzrost) - (5m * wiek) + 5m);

        private void WypiszMakro(params int[] makroskladniki)
        {
            Console.WriteLine($"              Białko: {makroskladniki[0]}g | Tłuszcze: {makroskladniki[1]}g | Węgle: {makroskladniki[2]}g");
        }

        private decimal PrognozujWageRekurencyjnie(decimal waga, decimal zmianaTygodniowa, int tygodnieZostaly)
        {
            if (tygodnieZostaly == 0) return waga;
            return PrognozujWageRekurencyjnie(waga + zmianaTygodniowa, zmianaTygodniowa, tygodnieZostaly - 1);
        }
    }
     
    public class ModulTreningowy
    {
        public void Uruchom()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("              ╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("              ║               KALKULATOR 1RM I PLANY                ║");
            Console.WriteLine("              ╚═════════════════════════════════════════════════════╝");
            Console.ResetColor();

            decimal ciezar = Narzedzia.PobierzLiczbe("              Ile najwięcej podniosłeś (kg): ");
            int powtorzenia = (int)Narzedzia.PobierzLiczbe("              Na ile powtórzeń to poszło: ");

            decimal max1RM = Oblicz1RM(ciezar, powtorzenia);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n              » Twój szacowany max (1RM): {max1RM} kg «");
            Console.ResetColor();

            Console.Write("\n              Zapisujemy to do historii rekordów? (T/N): ");
            if (Console.ReadLine().ToUpper() == "T")
            {
                Console.Write("              > Jakie to było ćwiczenie (np. Wyciskanie): ");
                string rekordCwiczenie = Console.ReadLine().Trim();
                try
                {
                    using (StreamWriter sw = new StreamWriter("HistoriaRekordow.txt", true))
                    {
                        sw.WriteLine($"{DateTime.Now.ToShortDateString()} - {rekordCwiczenie}: {max1RM} kg");
                    }
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("              [ Zapisano! Sprawdź plik HistoriaRekordow.txt ]");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("              Błąd przy zapisywaniu rekordu: " + ex.Message);
                }
            }

            Console.Write("\n              Robimy z tego 5-tygodniowy plan i zrzucamy do pliku? (T/N): ");
            if (Console.ReadLine().ToUpper() == "T")
            {
                Console.Write("              > Podaj nazwę ćwiczenia do planu: ");
                string cwiczenie = Console.ReadLine().Trim();

                Console.WriteLine("\n              Cel planu:\n              [1] Czysta siła\n              [2] Hipertrofia (Masa)");
                Console.Write("              Wybierz (1-2): ");
                CelTreningowy cel = Console.ReadLine() == "1" ? CelTreningowy.Sila : CelTreningowy.Hipertrofia;

                int czestotliwosc = 1;
                while (true)
                {
                    Console.Write("              Ile razy w tygodniu to robisz? (1-3): ");
                    if (int.TryParse(Console.ReadLine(), out czestotliwosc) && czestotliwosc >= 1 && czestotliwosc <= 3) break;
                }

                GenerujZapiszPlan(max1RM, cel, czestotliwosc, cwiczenie);
            }

            Console.WriteLine("\n              Wciśnij cokolwiek, aby wrócić...");
            Console.ReadKey();
        }

        static decimal Oblicz1RM(decimal ciezar, int powtorzenia)
        {
            decimal wynik = ciezar * (1M + (decimal)powtorzenia / 30M);
            return Math.Round(wynik / 2.5m) * 2.5m;
        }

        private void GenerujZapiszPlan(decimal max1RM, CelTreningowy cel, int czestotliwosc, string cwiczenie)
        {
            decimal[] intensywnoscBaza = (cel == CelTreningowy.Sila)
                ? new decimal[] { 0.75m, 0.80m, 0.85m, 0.90m, 0.95m }
                : new decimal[] { 0.65m, 0.70m, 0.72m, 0.75m, 0.80m };

            int[,] schemat = new int[5, 2];
            for (int t = 0; t < 5; t++)
            {
                schemat[t, 0] = 4;
                schemat[t, 1] = (cel == CelTreningowy.Sila) ? Math.Max(6 - t, 1) : Math.Max(12 - t, 1);
            }

            string czystaNazwaCwiczenia = cwiczenie.Replace("/", "").Replace("\\", "").Replace(":", "").Replace("*", "").Replace("?", "").Replace("\"", "").Replace("<", "").Replace(">", "").Replace("|", "");
            string sciezkaPliku = $"{cel.ToString().ToUpper()}_{czystaNazwaCwiczenia}_Periodyzacja.txt";

            using (StreamWriter sw = new StreamWriter(sciezkaPliku))
            {
                string naglowek = $"\n--- 5-TYGODNIOWA PERIODYZACJA: {cel.ToString().ToUpper()} | ĆWICZENIE: {cwiczenie.ToUpper()} ---";
                Console.WriteLine(naglowek.Replace("\n", "\n              "));
                sw.WriteLine(naglowek);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("              " + $"{"Tydzień",-8} | {"Dzień",-7} | {"Ciężar",-9} | {"Serie x Powt.",-15}");
                Console.WriteLine("              " + new string('-', 45));
                Console.ResetColor();

                for (int t = 0; t < 5; t++)
                {
                    for (int d = 0; d < czestotliwosc; d++)
                    {
                        decimal modyfikator = (d == 0) ? 1.0m : (d == 1) ? 0.85m : 0.90m;
                        decimal surowyCiezar = max1RM * (intensywnoscBaza[t] * modyfikator);

                        decimal roboczy = Math.Round(surowyCiezar / 2.5m) * 2.5m;

                        int serie = (d == 0) ? schemat[t, 0] : schemat[t, 0] - 1;
                        int powt = (d == 0) ? schemat[t, 1] : schemat[t, 1] + 2;

                        string wiersz = $"Tydzień {t + 1,-1} | Dzień {d + 1,-1} | {roboczy,-6} kg | {serie} x {powt}";
                        Console.WriteLine("              " + wiersz);
                        sw.WriteLine(wiersz);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n              [ Plik gotowy! Nazwa: {sciezkaPliku} ]");
            Console.ResetColor();
        }
    }

    public class GeneratorPosilkowCSV
    {
        private Dictionary<string, Danie> menuRedukcyjne = new Dictionary<string, Danie>();
        private Dictionary<string, Danie> menuMasowe = new Dictionary<string, Danie>();

        public GeneratorPosilkowCSV()
        {
            WczytajBazeZPlikuCSV();
        }

        private void WczytajBazeZPlikuCSV()
        {
            string sciezka = "Dania.csv";
            if (!File.Exists(sciezka))
            {
                return;
            }

            using (StreamReader sr = new StreamReader(sciezka))
            {
                string linia;
                while ((linia = sr.ReadLine()) != null)
                {
                    string[] dane = linia.Split(';');
                    if (dane[0] == "Typ") continue;

                    Danie noweDanie = new Danie
                    {
                        Nazwa = dane[2],
                        SkladnikiOpis = dane[3],
                        BazoweKcal = int.Parse(dane[4])
                    };

                    if (dane[0] == "Redukcja")
                        menuRedukcyjne.Add(dane[1], noweDanie);
                    else if (dane[0] == "Masa")
                        menuMasowe.Add(dane[1], noweDanie);
                }
            }
        }

        public void Uruchom()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("              ╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("              ║                  CO DZISIAJ JEMY?                   ║");
            Console.WriteLine("              ╚═════════════════════════════════════════════════════╝");
            Console.ResetColor();

            if (menuRedukcyjne.Count == 0 && menuMasowe.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("              [BŁĄD] Nie widzę pliku Dania.csv!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\n              Jaki masz teraz cel:");
            Console.WriteLine("              [1] Redukcja (zapychające i dużo białka)");
            Console.WriteLine("              [2] Masa (łatwostrawne kalorie)");
            Console.Write("\n              Wybieraj: ");
            string celInput = Console.ReadLine();
            Dictionary<string, Danie> wybraneMenu = (celInput == "1") ? menuRedukcyjne : menuMasowe;

            Console.WriteLine("\n              Oto co mamy w bazie:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("              -----------------------------------------------------");
            Console.ResetColor();
            foreach (var danie in wybraneMenu)
            {
                Console.WriteLine($"              [{danie.Key}] {danie.Value.Nazwa} (Baza: {danie.Value.BazoweKcal} kcal)");
            }
            Console.Write("\n              > Podaj numer z listy: ");
            string numerDania = Console.ReadLine();

            if (wybraneMenu.ContainsKey(numerDania))
            {
                Danie danie = wybraneMenu[numerDania];
                int doceloweKcal = (int)Narzedzia.PobierzLiczbe("\n              Ile kalorii ma mieć ten posiłek? (np. 500): ");
                decimal przelicznik = (decimal)doceloweKcal / danie.BazoweKcal;

                string tekstWRamce = $"  PRZEPIS DOSTOSOWANY POD TWÓJ CEL KALORII ({doceloweKcal} kcal)  ";
                string liniaKreskowa = new string('═', tekstWRamce.Length);

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\n              ╔{liniaKreskowa}╗");
                Console.WriteLine($"              ║{tekstWRamce}║");
                Console.WriteLine($"              ╚{liniaKreskowa}╝");
                Console.ResetColor();
                Console.WriteLine($"              » Danie: {danie.Nazwa}");
                Console.WriteLine("\n              Składniki do zważenia:");

                string[] skladniki = danie.SkladnikiOpis.Split(',');
                foreach (string skladnik in skladniki)
                {
                    string oczyszczonySkladnik = skladnik.Trim();
                    int startNawiasu = oczyszczonySkladnik.IndexOf('(');
                    int stopNawiasu = startNawiasu != -1 ? oczyszczonySkladnik.IndexOf('g', startNawiasu) : -1;
                    if (startNawiasu != -1 && stopNawiasu != -1)
                    {
                        string nazwaSkladnika = oczyszczonySkladnik.Substring(0, startNawiasu).Trim();
                        string wagaStr = oczyszczonySkladnik.Substring(startNawiasu + 1, stopNawiasu - startNawiasu - 1);
                        double bazowaWaga = double.Parse(wagaStr);
                        int wyliczonaWaga = (int)Math.Round(bazowaWaga * (double)przelicznik);
                        if (wyliczonaWaga == 0 && bazowaWaga > 0) wyliczonaWaga = 1;
                        Console.WriteLine($"              - {nazwaSkladnika}: {wyliczonaWaga} g");
                    }
                    else
                    {
                        Console.WriteLine($"              - {oczyszczonySkladnik}");
                    }
                }

                int bialko = celInput == "1" ? (int)((doceloweKcal * 0.40) / 4) : (int)((doceloweKcal * 0.25) / 4);
                int tluszcze = (int)((doceloweKcal * 0.25) / 9);
                int wegle = celInput == "1" ? (int)((doceloweKcal * 0.35) / 4) : (int)((doceloweKcal * 0.50) / 4);

                Console.WriteLine("\n              » Makro dla tej porcji:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine($"                Białko: ~{bialko}g | Tłuszcze: ~{tluszcze}g | Węgle: ~{wegle}g");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n              Nie ma takiego dania na liście.");
                Console.ResetColor();
            }
            Console.WriteLine("\n              Wciśnij cokolwiek, aby wrócić...");
            Console.ReadKey();
        }
    }

    public class Porady
    {
        public static void Uruchom()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("              ╔═════════════════════════════════════════════════════╗");
            Console.WriteLine("              ║              PORADY DLA POCZĄTKUJĄCYCH              ║");
            Console.WriteLine("              ╚═════════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();

            string sciezka = "Porady.csv";
            if (!File.Exists(sciezka))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("              Nie widzę pliku Porady.csv!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            using (StreamReader sr = new StreamReader(sciezka))
            {
                string linia;
                while ((linia = sr.ReadLine()) != null)
                {
                    if (linia.StartsWith("Kategoria")) continue;
                    WyswietlPojedynczaPorade(linia);
                }
            }
            Console.WriteLine("\n              Wciśnij cokolwiek, aby wrócić...");
            Console.ReadKey();
        }

        private static void WyswietlPojedynczaPorade(string liniaCSV)
        {
            string[] czesci = liniaCSV.Split(';');
            if (czesci.Length >= 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"              [{czesci[0].ToUpper()}] {czesci[1]}");
                Console.ResetColor();
                Console.WriteLine($"              {czesci[2]}\n");
            }
        }
    }
}