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
            bool programDziala = true;

            while (programDziala)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine("            WITAJ W FITCALC             ");
                Console.WriteLine("========================================");
                Console.ResetColor();

                Console.WriteLine("1. Oblicz zapotrzebowanie kaloryczne, BMI i nawodnienie");
                Console.WriteLine("2. Kalkulator 1RM i plan treningowy");
                Console.WriteLine("3. Propozycje dań z proporcjami");
                Console.WriteLine("4. Porady dla początkujących");
                Console.WriteLine("5. Wyjście z programu");
                Console.Write("\nWybierz opcję (1-5): ");

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
                        Console.WriteLine("\nZamykanie programu. Do zobaczenia na treningu!");
                        Console.ResetColor();
                        programDziala = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nBłąd: Nieprawidłowy wybór.");
                        Console.ResetColor();
                        Console.WriteLine("Wciśnij dowolny klawisz...");
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
                        throw new FormatException("Wartość musi być liczbą większą od zera.");
                }
                catch (FormatException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Błąd: {ex.Message} Spróbuj ponownie.\n");
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
            Console.WriteLine("--- KALKULATOR ZAPOTRZEBOWANIA ---");

            ProfilUzytkownika profil = new ProfilUzytkownika();
            bool uzytoZapisanego = false;

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

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("Znaleziono zapisany profil użytkownika!");
                            Console.ResetColor();
                            Console.WriteLine($"- Waga: {profil.Waga} kg");
                            Console.WriteLine($"- Wzrost: {profil.Wzrost} cm");
                            Console.WriteLine($"- Wiek: {profil.Wiek} lat");
                            Console.WriteLine($"- Obecny Cel: {profil.Cel}");

                            Console.Write("\nCo robimy? (T - użyj tych danych, C - zmień tylko cel, N - wpisz wszystko od nowa): ");
                            string odp = Console.ReadLine().ToUpper();

                            if (odp == "T")
                            {
                                uzytoZapisanego = true;
                            }
                            else if (odp == "C")
                            {
                                Console.WriteLine("\nWybierz nowy cel dietetyczny:\n1. Redukcja\n2. Utrzymanie\n3. Masa");
                                int nowyCel;
                                while (!int.TryParse(Console.ReadLine(), out nowyCel) || nowyCel < 1 || nowyCel > 3)
                                {
                                    Console.Write("Błąd. Wybierz 1, 2 lub 3: ");
                                }
                                profil.Cel = (CelDietetyczny)nowyCel;

                                // Nadpisanie pliku nowym celem
                                try
                                {
                                    using (StreamWriter sw = new StreamWriter("Profil.csv"))
                                    {
                                        sw.WriteLine($"{profil.Waga};{profil.Wzrost};{profil.Wiek};{(int)profil.Cel}");
                                    }
                                    Console.ForegroundColor = ConsoleColor.Magenta;
                                    Console.WriteLine("Cel został zaktualizowany!");
                                    Console.ResetColor();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Problem z aktualizacją celu: " + ex.Message);
                                }
                                uzytoZapisanego = true; // Ustawiamy na true, żeby pominąć wpisywanie danych od nowa
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd odczytu profilu: " + ex.Message);
                }
            }

            if (uzytoZapisanego == false)
            {
                Console.WriteLine("\n--- WPROWADZANIE DANYCH ---");
                profil.Waga = Narzedzia.PobierzLiczbe("Podaj swoją wagę (kg): ");
                profil.Wzrost = (int)Narzedzia.PobierzLiczbe("Podaj swój wzrost (cm): ");
                profil.Wiek = (int)Narzedzia.PobierzLiczbe("Podaj swój wiek (lata): ");

                Console.WriteLine("\nCel dietetyczny:\n1. Redukcja\n2. Utrzymanie\n3. Masa");
                int wyborCelu;
                while (!int.TryParse(Console.ReadLine(), out wyborCelu) || wyborCelu < 1 || wyborCelu > 3)
                {
                    Console.Write("Błąd. Wybierz 1, 2 lub 3: ");
                }
                profil.Cel = (CelDietetyczny)wyborCelu;

                try
                {
                    using (StreamWriter sw = new StreamWriter("Profil.csv"))
                    {
                        sw.WriteLine($"{profil.Waga};{profil.Wzrost};{profil.Wiek};{(int)profil.Cel}");
                    }
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\nTwój profil został pomyślnie zapisany!");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Problem z zapisem profilu: " + ex.Message);
                }
            }

            int bmr = ObliczBMR(profil.Waga, profil.Wzrost, profil.Wiek);
            int tdee = (int)(bmr * 1.6);
            int doceloweKcal = tdee;
            decimal zmianaWagiTygodniowo = 0;

            if (profil.Cel == CelDietetyczny.Redukcja) { doceloweKcal -= 400; zmianaWagiTygodniowo = -0.4m; }
            else if (profil.Cel == CelDietetyczny.Masa) { doceloweKcal += 400; zmianaWagiTygodniowo = 0.4m; }

            int bialko = (int)(profil.Waga * 2.2m);
            int tluszcze = (int)(profil.Waga * 1.0m);
            int wegle = (doceloweKcal - (bialko * 4) - (tluszcze * 9)) / 4;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nCałkowite zapotrzebowanie: {tdee} kcal");
            Console.WriteLine($"ZALECANE SPOŻYCIE ({profil.Cel.ToString().ToUpper()}): {doceloweKcal} kcal");
            Console.ResetColor();

            WypiszMakro(bialko, tluszcze, wegle);

            if (zmianaWagiTygodniowo != 0)
            {
                Console.WriteLine("\n--- PROGNOZA WAGI ---");
                decimal nowaWaga = PrognozujWageRekurencyjnie(profil.Waga, zmianaWagiTygodniowo, 4);
                Console.WriteLine($"Waga za 4 tyg.: {Math.Round(nowaWaga, 1)} kg");
            }

            Console.WriteLine("\n--- ANALIZA SYLWETKI I NAWODNIENIA ---");
            decimal wzrostMetry = profil.Wzrost / 100m;
            decimal bmi = profil.Waga / (wzrostMetry * wzrostMetry);

            int woda = (int)(profil.Waga * 35);

            Console.WriteLine($"- Twoje BMI: {Math.Round(bmi, 1)}");
            Console.Write("- Status wagowy: ");
            if (bmi < 18.5m) Console.WriteLine("Niedowaga");
            else if (bmi < 25m) Console.WriteLine("Norma (Zdrowa waga)");
            else if (bmi < 30m) Console.WriteLine("Nadwaga");
            else Console.WriteLine("Otyłość");

            Console.WriteLine($"- Zalecane dzienne nawodnienie: {woda} ml (ok. {Math.Round(woda / 250.0, 1)} szklanek wody)");

            Console.WriteLine("\nWciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ObliczBMR(decimal waga, int wzrost, int wiek) => (int)((10m * waga) + (6.25m * wzrost) - (5m * wiek) + 5m);

        private void WypiszMakro(params int[] makroskladniki)
        {
            Console.WriteLine($"- Białko: {makroskladniki[0]}g | Tłuszcze: {makroskladniki[1]}g | Węgle: {makroskladniki[2]}g");
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
            Console.WriteLine("--- KALKULATOR 1RM ---");

            decimal ciezar = Narzedzia.PobierzLiczbe("Podnoszony ciężar (kg): ");
            int powtorzenia = (int)Narzedzia.PobierzLiczbe("Liczba powtórzeń w serii: ");

            decimal max1RM = Oblicz1RM(ciezar, powtorzenia);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nTwój max (1RM): {max1RM} kg");
            Console.ResetColor();

            Console.Write("\nCzy zapisać ten wynik do historii rekordów? (T/N): ");
            if (Console.ReadLine().ToUpper() == "T")
            {
                Console.Write("Podaj nazwę ćwiczenia dla rekordu (np. Wyciskanie): ");
                string rekordCwiczenie = Console.ReadLine().Trim();
                try
                {
                    using (StreamWriter sw = new StreamWriter("HistoriaRekordow.txt", true))
                    {
                        sw.WriteLine($"{DateTime.Now.ToShortDateString()} - {rekordCwiczenie}: {max1RM} kg");
                    }
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Wynik został zapisany w pliku HistoriaRekordow.txt");
                    Console.ResetColor();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Błąd zapisu rekordu: " + ex.Message);
                }
            }

            Console.Write("\nGenerować 5-tygodniowy plan i zapisać do pliku? (T/N): ");
            if (Console.ReadLine().ToUpper() == "T")
            {
                Console.Write("Podaj nazwę ćwiczenia do planu: ");
                string cwiczenie = Console.ReadLine().Trim();

                Console.WriteLine("Cel: 1. Siła | 2. Hipertrofia");
                CelTreningowy cel = Console.ReadLine() == "1" ? CelTreningowy.Sila : CelTreningowy.Hipertrofia;

                int czestotliwosc = 1;
                while (true)
                {
                    Console.Write("Ile razy w tygodniu robisz to ćwiczenie? (1-3): ");
                    if (int.TryParse(Console.ReadLine(), out czestotliwosc) && czestotliwosc >= 1 && czestotliwosc <= 3) break;
                }

                GenerujZapiszPlan(max1RM, cel, czestotliwosc, cwiczenie);
            }

            Console.WriteLine("\nWciśnij dowolny klawisz...");
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
                Console.WriteLine(naglowek);
                sw.WriteLine(naglowek);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"{"Tydzień",-8} | {"Dzień",-7} | {"Ciężar",-9} | {"Serie x Powt.",-15}");
                Console.WriteLine(new string('-', 45));
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
                        Console.WriteLine(wiersz);
                        sw.WriteLine(wiersz);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\nPlan został pomyślnie zapisany w pliku: {sciezkaPliku}");
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
            Console.WriteLine("--- PROPOZYCJE DAŃ Z PROPORCJAMI ---");

            if (menuRedukcyjne.Count == 0 && menuMasowe.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Błąd: Nie znaleziono pliku bazy danych potraw (Dania.csv)!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Wybierz swój obecny cel:");
            Console.WriteLine("1. Redukcja (Czyste, wysokobiałkowe, o niskiej gęstości)");
            Console.WriteLine("2. Masa (Zdrowe, bogatokaloryczne i łatwostrawne)");

            string celInput = Console.ReadLine();
            Dictionary<string, Danie> wybraneMenu = (celInput == "1") ? menuRedukcyjne : menuMasowe;

            Console.WriteLine("\nDostępne propozycje dań:");
            foreach (var danie in wybraneMenu)
            {
                Console.WriteLine($"{danie.Key}. {danie.Value.Nazwa} (Baza: {danie.Value.BazoweKcal} kcal)");
            }

            Console.Write("\nWybierz numer dania: ");
            string numerDania = Console.ReadLine();

            if (wybraneMenu.ContainsKey(numerDania))
            {
                Danie danie = wybraneMenu[numerDania];
                int doceloweKcal = (int)Narzedzia.PobierzLiczbe("\nIle kalorii ma mieć ten posiłek? (np. 300-1500): ");

                decimal przelicznik = (decimal)doceloweKcal / danie.BazoweKcal;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n========================================");
                Console.WriteLine($"   PRZEPIS SKROJONY POD TWÓJ CEL ({doceloweKcal} kcal)");
                Console.WriteLine($"========================================");
                Console.ResetColor();
                Console.WriteLine($"Nazwa: {danie.Nazwa}");
                Console.WriteLine($"\nSkładniki:");

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

                        Console.WriteLine($"- {nazwaSkladnika}: {wyliczonaWaga} g");
                    }
                    else
                    {
                        Console.WriteLine($"- {oczyszczonySkladnik}");
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nBłąd: Nie ma takiego dania na liście.");
                Console.ResetColor();
            }

            Console.WriteLine("\nWciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }
    }

    public class Porady
    {
        public static void Uruchom()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("        PORADY DLA POCZĄTKUJĄCYCH       ");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine();

            string sciezka = "Porady.csv";
            if (!File.Exists(sciezka))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Błąd: Nie znaleziono pliku Porady.csv!");
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

            Console.WriteLine("\nWciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }

        private static void WyswietlPojedynczaPorade(string liniaCSV)
        {
            string[] czesci = liniaCSV.Split(';');
            if (czesci.Length >= 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{czesci[0].ToUpper()}] {czesci[1]}");
                Console.ResetColor();
                Console.WriteLine($"{czesci[2]}\n");
            }
        }
    }
}