# FitCalc to aplikacja konsolowa napisana w C#, będąca osobistym asystentem treningowo-żywieniowym.

##   Główne funkcje:
1. **Część Dietetyczna:** 
   - Obliczanie wskaźnika BMI oraz BMR (podstawowej przemiany materii).
   - Wyliczanie zapotrzebowania kalorycznego z podziałem na makroskładniki (Białko/Tłuszcz/Węglowodany) w zależności od wybranego celu (Masa, Redukcja lub Utrzymanie).
   - Możliwość wyboru tempa zmiany wagi (kg/tydzień).
   - Prognoza wagi po miesiącu.
   - System zapisywania profilu użytkownika do pliku CSV, co pozwala na pominięcie wpisywania danych przy kolejnych uruchomieniach.

2. **Część Treningowa:**
   - Obliczanie szacowanego ciężaru maksymalnego (1RM).
   - Możliwość zapisu rekordów w danym ćwiczeniu do pliku TXT.
   - Generowanie gotowego, 5-tygodniowego planu treningowego z uwzględnieniem progresji ciężaru (pod siłę lub hipertrofię) i zapis do pliku TXT.

3. **Cześć z Posiłkami:**
   - Odczyt dań z pliku CSV.
   - Dostosowywanie gramatury składników tak, aby zmieścić się w założonej liczbie kalorii wybranej przez użytkownika.
   - Dodatkowe obliczanie makroskładników (Białko/Tłuszcz/Węglowodany) w zależności od wybranego celu.

4. **Część z Poradami Treningowo-Dietetyczne:**
   - Zakładka z bardzo pomocnymi, sensownymi tipami dla osób początkujacych na siłowni.

##   Jak odpalić projekt?

Aplikacja opiera się na plikach CSV. Aby program zadziałał poprawnie i nie wyrzucił błędu, pliki Dania.csv oraz Porady.csv muszą znajdować się w tym samym folderze co plik  .exe.

Wszystkie nowe pliki generowane przez aplikację (np. zapisane plany treningowe czy historia rekordów) również będą zapisywać się w tym samym folderze roboczym.

## Na czym opiera się projekt:
- Napisany w C#
- Konsola jako interfejs
- Odczyt i zapis plików tekstowych oraz CSV 
- Instrukcje warunkowe, pętle, rekurencja, obsługa wyjątków, kolekcje
