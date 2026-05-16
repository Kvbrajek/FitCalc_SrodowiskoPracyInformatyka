using System;
using System.Collections.Generic;
using System.IO;

namespace FitCalc
{
   
    public enum CelDietetyczny { Redukcja = 1, Utrzymanie = 2, Masa = 3 }
    public enum CelTreningowy { Sila = 1, Hipertrofia = 2 }

    
    public class Profil
    {
        public decimal Waga;
        public int Wzrost;
        public int Wiek;
        public CelDietetyczny Cel;
    }

    public class Danie
    {
        public string Nazwa;
        public string SkladnikiOpis;
        public int BazoweKcal;
    }

    class Program
    {
        static void Main(string[] args)
        {
            bool dziala = true;

            // glowna petla programu
            while (dziala)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("========================================");
                Console.WriteLine("            WITAJ W FITCALC             ");
                Console.WriteLine("========================================");
                Console.ResetColor();

                Console.WriteLine("1. Oblicz zapotrzebowanie kaloryczne");
                Console.WriteLine("2. Kalkulator 1RM i plan treningowy");
                Console.WriteLine("3. Propozycje dań z proporcjami");
                Console.WriteLine("4. Porady dla początkujących");
                Console.WriteLine("5. Wyjście z programu");
                Console.Write("\nWybierz opcję (1-5): ");

                string wybor = Console.ReadLine();

                switch (wybor)
                {
                    case "1":
                        ModulDietetyczny dieta = new ModulDietetyczny();
                        dieta.Uruchom();
                        break;
                    case "2":
                        ModulTreningowy trening = new ModulTreningowy();
                        trening.Uruchom();
                        break;
                    case "3":
                        GeneratorPosilkowCSV jedzenie = new GeneratorPosilkowCSV();
                        jedzenie.Uruchom();
                        break;
                    case "4":
                        Porady.Uruchom();
                        break;
                    case "5":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nZamykanie programu. Powodzenia na treningu!");
                        Console.ResetColor();
                        dziala = false;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nBłąd: Nie ma takiej opcji.");
                        Console.ResetColor();
                        Console.WriteLine("Wciśnij dowolny klawisz...");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }

    // Klasa z przydatnymi funkcjami
    public class Pomocnik
    {
        // funkcja wczytuje liczbe od usera i sprawdza czy nie wpisal glupot
        public static decimal WczytajLiczbe(string tekst)
        {
            decimal liczba;
            while (true)
            {
                Console.Write(tekst);
                string wejscie = Console.ReadLine();

                try
                {
                    bool czySieUdalo = decimal.TryParse(wejscie, out liczba);
                    if (czySieUdalo && liczba > 0)
                    {
                        return liczba;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Podaj poprawną liczbę większą od zera!\n");
                        Console.ResetColor();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Blad: " + e.Message);
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

            Profil profilUsera = new Profil();
            profilUsera.Waga = Pomocnik.WczytajLiczbe("Podaj swoją wagę (kg): ");
            profilUsera.Wzrost = (int)Pomocnik.WczytajLiczbe("Podaj swój wzrost (cm): ");
            profilUsera.Wiek = (int)Pomocnik.WczytajLiczbe("Podaj swój wiek (lata): ");

            Console.WriteLine("\nJaki masz cel?");
            Console.WriteLine("1. Redukcja");
            Console.WriteLine("2. Utrzymanie wagi");
            Console.WriteLine("3. Masa");

            int wyborCelu = 0;
            while (wyborCelu < 1 || wyborCelu > 3)
            {
                Console.Write("Wybierz opcję (1-3): ");
                string wejscie = Console.ReadLine();
                int.TryParse(wejscie, out wyborCelu);
            }

            
            profilUsera.Cel = (CelDietetyczny)wyborCelu;

            // liczenie zapotrzebowania
            int bmr = ObliczBMR(profilUsera.Waga, profilUsera.Wzrost, profilUsera.Wiek);
            int kalorie = (int)(bmr * 1.6); // mnoznik aktywnosci

            decimal zmianaWagi = 0;

            if (profilUsera.Cel == CelDietetyczny.Redukcja)
            {
                kalorie = kalorie - 400;
                zmianaWagi = -0.4m;
            }
            else if (profilUsera.Cel == CelDietetyczny.Masa)
            {
                kalorie = kalorie + 400;
                zmianaWagi = 0.4m;
            }

            // makrosy
            int bialko = (int)(profilUsera.Waga * 2.2m);
            int tluszcze = (int)(profilUsera.Waga * 1.0m);
            int wegle = (kalorie - (bialko * 4) - (tluszcze * 9)) / 4;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nCałkowite zapotrzebowanie (TDEE): {bmr * 1.6} kcal");
            Console.WriteLine($"ZALECANE SPOŻYCIE ({profilUsera.Cel.ToString().ToUpper()}): {kalorie} kcal");
            Console.ResetColor();

            WypiszMakro(bialko, tluszcze, wegle);

            if (zmianaWagi != 0)
            {
                Console.WriteLine("\n--- PROGNOZA WAGI ---");
                decimal nowaWaga = RekurencjaWaga(profilUsera.Waga, zmianaWagi, 4);
                Console.WriteLine($"Twoja przewidywana waga za 4 tygodnie: {Math.Round(nowaWaga, 1)} kg");
            }

            Console.WriteLine("\nWciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        private int ObliczBMR(decimal waga, int wzrost, int wiek)
        {
            // wzor na podstawowa przemiane materii
            decimal wynik = (10m * waga) + (6.25m * wzrost) - (5m * wiek) + 5m;
            return (int)wynik;
        }

        private void WypiszMakro(int b, int t, int w)
        {
            Console.WriteLine($"- Białko: {b}g | Tłuszcze: {t}g | Węglowodany: {w}g");
        }

        
        private decimal RekurencjaWaga(decimal aktualnaWaga, decimal zmiana, int tygodnie)
        {
            if (tygodnie == 0)
            {
                return aktualnaWaga;
            }
            else
            {
                return RekurencjaWaga(aktualnaWaga + zmiana, zmiana, tygodnie - 1);
            }
        }
    }

    public class ModulTreningowy
    {
        public void Uruchom()
        {
            Console.Clear();
            Console.WriteLine("--- KALKULATOR 1RM (Maksymalne powtórzenie) ---");

            decimal ciezar = Pomocnik.WczytajLiczbe("Podnoszony ciężar (kg): ");
            int powtorzenia = (int)Pomocnik.WczytajLiczbe("Liczba powtórzeń w serii: ");

            decimal maks = ObliczMaks(ciezar, powtorzenia);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nTwój max na jedno powtórzenie (1RM): {maks} kg");
            Console.ResetColor();

            Console.Write("\nCzy chcesz wygenerować 5-tygodniowy plan i zapisać go do pliku? (T/N): ");
            string odpowiedz = Console.ReadLine();

            if (odpowiedz.ToUpper() == "T")
            {
                Console.Write("Podaj nazwę ćwiczenia (np. Wyciskanie): ");
                string nazwaCwiczenia = Console.ReadLine();

                Console.WriteLine("Jaki masz cel treningowy?");
                Console.WriteLine("1. Siła");
                Console.WriteLine("2. Hipertrofia (Masa mięśniowa)");

                string celOdp = Console.ReadLine();
                CelTreningowy wybranyCel = CelTreningowy.Sila;

                if (celOdp == "2")
                {
                    wybranyCel = CelTreningowy.Hipertrofia;
                }

                int dniWtgodniu = 1;
                while (true)
                {
                    Console.Write("Ile razy w tygodniu robisz to ćwiczenie? (1-3): ");
                    string dniStr = Console.ReadLine();
                    if (int.TryParse(dniStr, out dniWtgodniu) && dniWtgodniu >= 1 && dniWtgodniu <= 3)
                    {
                        break;
                    }
                }

                StworzPlan(maks, wybranyCel, dniWtgodniu, nazwaCwiczenia);
            }

            Console.WriteLine("\nWciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        private decimal ObliczMaks(decimal ciezar, int powtorzenia)
        {
            // wzor Epleya
            decimal wynik = ciezar * (1M + (decimal)powtorzenia / 30M);
            // zaokraglamy do 2.5 kg zeby mialo sens na silowni
            return Math.Round(wynik / 2.5m) * 2.5m;
        }

        private void StworzPlan(decimal max1RM, CelTreningowy cel, int dni, string cwiczenie)
        {
            // tablice do logiki planu
            decimal[] procenty;
            if (cel == CelTreningowy.Sila)
            {
                procenty = new decimal[] { 0.75m, 0.80m, 0.85m, 0.90m, 0.95m };
            }
            else
            {
                procenty = new decimal[] { 0.65m, 0.70m, 0.72m, 0.75m, 0.80m };
            }

            int[,] planSerii = new int[5, 2];
            for (int i = 0; i < 5; i++)
            {
                planSerii[i, 0] = 4; // zawsze 4 serie glowne

                if (cel == CelTreningowy.Sila)
                {
                    planSerii[i, 1] = Math.Max(6 - i, 1); // powtorzenia spadaja co tydzien
                }
                else
                {
                    planSerii[i, 1] = Math.Max(12 - i, 1);
                }
            }

            // usuwam z nazwy znaki specjalne, zeby plik sie dalo zapisac
            string nazwaPliku = cel.ToString().ToUpper() + "_" + cwiczenie + "_Periodyzacja.txt";
            nazwaPliku = nazwaPliku.Replace("/", "");
            nazwaPliku = nazwaPliku.Replace("\\", "");
            nazwaPliku = nazwaPliku.Replace("?", "");
            nazwaPliku = nazwaPliku.Replace(" ", "_");

            try
            {
                using (StreamWriter plik = new StreamWriter(nazwaPliku))
                {
                    string tytul = $"\n--- PLAN 5 TYGODNI: {cel.ToString().ToUpper()} | ĆWICZENIE: {cwiczenie.ToUpper()} ---";
                    Console.WriteLine(tytul);
                    plik.WriteLine(tytul);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Tydzień \t Dzień \t Ciężar \t Serie x Powt");
                    Console.WriteLine("-----------------------------------------------------");
                    Console.ResetColor();

                    for (int tydzien = 0; tydzien < 5; tydzien++)
                    {
                        for (int dzien = 0; dzien < dni; dzien++)
                        {
                            // jak ktos robi czesciej to troche zmniejszamy ciezar zeby sie nie zajechal
                            decimal zmniejszenie = 1.0m;
                            if (dzien == 1) zmniejszenie = 0.85m;
                            if (dzien == 2) zmniejszenie = 0.90m;

                            decimal ciezarDoWyliczenia = max1RM * (procenty[tydzien] * zmniejszenie);
                            decimal ciezarNaSztandze = Math.Round(ciezarDoWyliczenia / 2.5m) * 2.5m;

                            int iloscSerii = planSerii[tydzien, 0];
                            int iloscPowt = planSerii[tydzien, 1];

                            if (dzien != 0)
                            {
                                iloscSerii -= 1;
                                iloscPowt += 2;
                            }

                            string linijka = $"Tydzień {tydzien + 1} \t Dzień {dzien + 1} \t {ciezarNaSztandze} kg \t {iloscSerii} x {iloscPowt}";
                            Console.WriteLine(linijka);
                            plik.WriteLine(linijka);
                        }
                    }
                }
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\nPlan zapisany na dysku w pliku: {nazwaPliku}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wystąpił problem z zapisem do pliku: " + ex.Message);
            }
        }
    }

    public class GeneratorPosilkowCSV
    {
        private Dictionary<string, Danie> daniaNaRedukcje = new Dictionary<string, Danie>();
        private Dictionary<string, Danie> daniaNaMase = new Dictionary<string, Danie>();

        public GeneratorPosilkowCSV()
        {
            LadowaniePlikuCSV();
        }

        private void LadowaniePlikuCSV()
        {
            string nazwaPliku = "Dania.csv";

            if (File.Exists(nazwaPliku))
            {
                using (StreamReader czytnik = new StreamReader(nazwaPliku))
                {
                    string tekst;
                    while ((tekst = czytnik.ReadLine()) != null)
                    {
                        string[] dane = tekst.Split(';');

                        if (dane[0] == "Typ")
                        {
                            continue;
                        }

                        Danie posilek = new Danie();
                        posilek.Nazwa = dane[2];
                        posilek.SkladnikiOpis = dane[3];
                        posilek.BazoweKcal = int.Parse(dane[4]);

                        if (dane[0] == "Redukcja")
                        {
                            daniaNaRedukcje.Add(dane[1], posilek);
                        }
                        else if (dane[0] == "Masa")
                        {
                            daniaNaMase.Add(dane[1], posilek);
                        }
                    }
                }
            }
        }

        public void Uruchom()
        {
            Console.Clear();
            Console.WriteLine("--- PROPOZYCJE DAŃ Z PROPORCJAMI ---");

            if (daniaNaRedukcje.Count == 0 && daniaNaMase.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Nie znaleziono bazy potraw! Sprawdź czy plik Dania.csv istnieje.");
                Console.ResetColor();
                Console.ReadKey();
                return; 
            }

            Console.WriteLine("Jaki masz cel?");
            Console.WriteLine("1. Redukcja (posiłki zapychające)");
            Console.WriteLine("2. Masa (więcej kalorii)");

            string odpowiedz = Console.ReadLine();

            Dictionary<string, Danie> pokazaneDania;
            if (odpowiedz == "1")
            {
                pokazaneDania = daniaNaRedukcje;
            }
            else
            {
                pokazaneDania = daniaNaMase;
            }

            Console.WriteLine("\nCo chciałbyś zjeść?");
            foreach (var element in pokazaneDania)
            {
                Console.WriteLine($"{element.Key}. {element.Value.Nazwa} (ok. {element.Value.BazoweKcal} kcal)");
            }

            Console.Write("\nPodaj numer posiłku z listy: ");
            string numerek = Console.ReadLine();

            if (pokazaneDania.ContainsKey(numerek))
            {
                Danie wybrane = pokazaneDania[numerek];
                int kalorieUzytkownika = (int)Pomocnik.WczytajLiczbe("\nIle kalorii chcesz zjeść? (np. 500): ");

                // matematyka proporcji
                decimal mnoznik = (decimal)kalorieUzytkownika / wybrane.BazoweKcal;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n========================================");
                Console.WriteLine($" PRZEPIS DOPASOWANY DO CIEBIE ({kalorieUzytkownika} kcal)");
                Console.WriteLine($"========================================");
                Console.ResetColor();

                Console.WriteLine($"Danie: {wybrane.Nazwa}");
                Console.WriteLine($"\nLista składników:");

                string[] pojedynczeSkladniki = wybrane.SkladnikiOpis.Split(',');

                foreach (string skladnik in pojedynczeSkladniki)
                {
                    string s = skladnik.Trim(); 

                    int nawias1 = s.IndexOf('(');
                    int nawias2 = -1;

                    
                    if (nawias1 != -1)
                    {
                        nawias2 = s.IndexOf('g', nawias1);
                    }

                    if (nawias1 != -1 && nawias2 != -1)
                    {
                        string nazwaZarelka = s.Substring(0, nawias1).Trim();
                        string gramyStr = s.Substring(nawias1 + 1, nawias2 - nawias1 - 1);

                        double wagaCzego = double.Parse(gramyStr);
                        int wyliczoneGramy = (int)Math.Round(wagaCzego * (double)mnoznik);

                        if (wyliczoneGramy == 0 && wagaCzego > 0)
                        {
                            wyliczoneGramy = 1;
                        }

                        Console.WriteLine($"- {nazwaZarelka}: {wyliczoneGramy} g");
                    }
                    else
                    {
                        Console.WriteLine($"- {s}");
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nWybrano zły numer z listy.");
                Console.ResetColor();
            }

            Console.WriteLine("\nWciśnij dowolny klawisz...");
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

            string nazwaPliku = "Porady.csv";

            if (File.Exists(nazwaPliku) == false)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Brakuje pliku Porady.csv w folderze!");
                Console.ResetColor();
                Console.ReadKey();
                return;
            }

            try
            {
                using (StreamReader czytnik = new StreamReader(nazwaPliku))
                {
                    string linijka;
                    while ((linijka = czytnik.ReadLine()) != null)
                    {
                      
                        if (linijka.StartsWith("Kategoria"))
                        {
                            continue;
                        }

                        WyswietlPoradeZPliku(linijka);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cos poszlo nie tak z plikiem porad: " + ex.Message);
            }

            Console.WriteLine("\nWciśnij dowolny klawisz, aby wrócić do menu...");
            Console.ReadKey();
        }

        private static void WyswietlPoradeZPliku(string liniaZPliku)
        {
            string[] kolumny = liniaZPliku.Split(';');

            if (kolumny.Length >= 3)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{kolumny[0].ToUpper()}] {kolumny[1]}");
                Console.ResetColor();
                Console.WriteLine($"{kolumny[2]}\n");
            }
        }
    }
}