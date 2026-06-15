using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace FitCalc
{
    #region Struktury i Enumy
    public enum CelDietetyczny { Redukcja = 1, Utrzymanie = 2, Masa = 3 }
    public enum CelTreningowy { Sila = 1, Hipertrofia = 2 }

    public struct ProfilUzytkownika
    {
        public decimal Waga;
        public int Wzrost;
        public int Wiek;
        public CelDietetyczny Cel;
    }
    #endregion

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

                Console.WriteLine("1. Oblicz zapotrzebowanie (Kalkulator BMR/Kcal)");
                Console.WriteLine("2. Kalkulator 1RM i Generator Planu (zapis do pliku)");
                Console.WriteLine("3. Porady dla początkujących (Kompendium)");
                Console.WriteLine("4. Wyjście z programu");
                Console.Write("\nWybierz opcję (1-4): ");

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
                        Kompendium.Wyswietl();
                        Console.WriteLine("\nWciśnij dowolny klawisz, aby wrócić do menu...");
                        Console.ReadKey();
                        break;
                    case "4":
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

            Console.Write("\nGenerować 5-tygodniowy plan i zapisać do pliku? (T/N): ");
            if (Console.ReadLine().ToUpper() == "T")
            {
                Console.WriteLine("Cel: 1. Siła | 2. Hipertrofia");
                CelTreningowy cel = Console.ReadLine() == "1" ? CelTreningowy.Sila : CelTreningowy.Hipertrofia;

                int czestotliwosc = 1;
                while (true)
                {
                    Console.Write("Ile razy w tygodniu robisz to ćwiczenie? (1-3): ");
                    if (int.TryParse(Console.ReadLine(), out czestotliwosc) && czestotliwosc >= 1 && czestotliwosc <= 3) break;
                }

                GenerujZapiszPlan(max1RM, cel, czestotliwosc);
            }

            Console.WriteLine("\nWciśnij dowolny klawisz...");
            Console.ReadKey();
        }

        static decimal Oblicz1RM(decimal ciezar, int powtorzenia)
        {
            decimal wynik = ciezar * (1M + (decimal)powtorzenia / 30M);
            return Math.Round(wynik / 2.5m) * 2.5m;
        }

        private void GenerujZapiszPlan(decimal max1RM, CelTreningowy cel, int czestotliwosc)
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

            string sciezkaPliku = "PlanTreningowy.txt";
            using (StreamWriter sw = new StreamWriter(sciezkaPliku))
            {
                string naglowek = $"\n--- 5-TYGODNIOWA PERIODYZACJA: {cel.ToString().ToUpper()} ---";
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
                        decimal roboczy = Math.Round((max1RM * (intensywnoscBaza[t] * modyfikator)) / 2.5m) * 2.5m;
                        int serie = (d == 0) ? schemat[t, 0] : schemat[t, 0] - 1;
                        int powt = (d == 0) ? schemat[t, 1] : schemat[t, 1] + 2;
                        string wiersz = $"Tydzień {t + 1,-1} | Dzień {d + 1,-1} | {roboczy,-6} kg | {serie} x {powt}";
                        Console.WriteLine(wiersz);
                        sw.WriteLine(wiersz);
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n[SUKCES] Plan zapisany w pliku: {sciezkaPliku}");
            Console.ResetColor();
        }
    }

    public class Kompendium
    {
        public static void Wyswietl()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("   PORADY DLA POCZĄTKUJĄCYCH (KOMPENDIUM)");
            Console.WriteLine("========================================");
            Console.ResetColor();

            string surowyTekst =
                "[ODPOCZYNEK] Czas odpoczynku między seriami: Trening siłowy – 2-5 minut przerwy. Trening hipertroficzny – 30-90 sekund. Trening wytrzymałościowy – 15-45 sekund.|" +
                "[PROGRESJA] Brak progresji prowadzi do stagnacji, dlatego kluczowe jest stopniowe zwiększanie obciążenia.|" +
                "[POJĘCIA] Upadek mięśniowy: punkt, w którym nie wykonasz kolejnego powtórzenia poprawnie.|" +
                "[POJĘCIA] RPE: skala wysiłku (1-10). RIR: powtórzenia w zapasie do upadku.|" +
                "[POJĘCIA] Objętość: łączna liczba serii i powtórzeń w sesji. Intensywność: ciężar względem 1RM.|" +
                "[DIETA] Białko: 1.6-2.2g/kg masy ciała. Źródła: mięso, ryby, jaja, strączkowe.|" +
                "[DIETA] Węglowodany: główne źródło energii. Proste (owoce) i złożone (ryż, kasze).|" +
                "[DIETA] Tłuszcze: 20-30% kalorii, regulacja hormonów. Orzechy, oliwa, tłuste ryby.|" +
                "[SUPLEMENTY] Mikroskładniki: Witamina D, Magnez, Żelazo, Cynk – niezbędne dla regeneracji.";

            string[] porady = surowyTekst.Split('|');
            foreach (string porada in porady)
            {
                string s = porada.Replace("[ODPOCZYNEK]", "\n>>> REGENERACJA:\n").Replace("[PROGRESJA]", "\n>>> FUNDAMENT:\n").Replace("[POJĘCIA]", "\n>>> POJĘCIA:\n").Replace("[DIETA]", "\n>>> DIETA:\n").Replace("[SUPLEMENTY]", "\n>>> MIKROSKŁADNIKI:\n");
                if (s.Contains(">>>")) Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }
    }
}